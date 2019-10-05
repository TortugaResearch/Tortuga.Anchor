using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Tortuga.Anchor.Eventing;

namespace Tortuga.Anchor.Modeling.Internals
{
    /// <summary>
    /// Property bag with basic change tracking capabilities.
    /// </summary>
    public class ChangeTrackingPropertyBag : PropertyBagBase
    {
        readonly IListener<PropertyChangedEventArgs> m_ChildIsChangedPropertyListener;
        readonly Dictionary<string, object?> m_OriginalValues = new Dictionary<string, object?>(StringComparer.Ordinal);
        bool m_IsChangedLocal;

        /// <summary>
        /// Property bag with basic change tracking capabilities.
        /// </summary>
        /// <param name="owner">Owning model, used to fetch metadata</param>
        public ChangeTrackingPropertyBag(object owner)
    : base(owner)
        {
            m_ChildIsChangedPropertyListener = new Listener<PropertyChangedEventArgs>(OnChildIsChangedPropertyChanged);
        }

        /// <summary>
        /// Returns True if any fields were modified since the last call to AcceptChanges. This does not walk the object graph.
        /// </summary>
        /// <returns>true if the object’s content has changed since the last call to <see cref="System.ComponentModel.IChangeTracking.AcceptChanges" />; otherwise, false.</returns>
        public bool IsChangedLocal
        {
            get { return m_IsChangedLocal; }
            set
            {
                if (m_IsChangedLocal == value)
                    return;
                m_IsChangedLocal = value;

                OnPropertyChanged(Metadata.Properties[nameof(IsChangedLocal)]);
                OnPropertyChanged(Metadata.Properties["IsChanged"]);
            }
        }

        /// <summary>
        /// Access to the values dictionary for sub-classes. Extreme care must be taken when working this dictionary directly, as events will not be automatically fired.
        /// </summary>
        /// <value>
        /// The values.
        /// </value>
        protected Dictionary<string, object?> Values { get; } = new Dictionary<string, object?>(StringComparer.Ordinal);

        /// <summary>
        /// Marks all fields as unchanged by storing them in the original values collection.
        /// </summary>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        public virtual void AcceptChanges(bool recursive)
        {
            m_OriginalValues.Clear();

            foreach (var item in Values)
            {
                m_OriginalValues[item.Key] = item.Value;
                if (recursive && item.Value is IChangeTracking)
                    ((IChangeTracking)item.Value).AcceptChanges();
            }

            IsChangedLocal = false;
        }

        /// <summary>Implementors need to override this to return the indicated value.</summary>
        /// <returns>The indicated value or System.Reflection.Missing if the value isn't defined.</returns>
        /// <exception cref="ArgumentNullException">propertyName;propertyName is null</exception>
        /// <exception cref="ArgumentException">propertyName is empty.;propertyName</exception>
        public IReadOnlyList<string> ChangedProperties()
        {
            var result = new List<string>();

            foreach (var item in Values)
            {
                if (m_OriginalValues.TryGetValue(item.Key, out var old))
                {
                    if (!Equals(old, item.Value))
                        result.Add(item.Key);
                }
                else
                {
                    result.Add(item.Key);
                }
            }

            return new ReadOnlyCollection<string>(result);
        }

        /// <summary>
        /// Gets the previous value for the indicated property.
        /// </summary>
        /// <param name="propertyName">
        /// Name of the property.
        /// </param>
        public object? GetPreviousValue(string propertyName)
        {
            if (m_OriginalValues.TryGetValue(propertyName, out var result))
                return result;

            return NotSet.Value;
        }

        /// <summary>Implementors need to override this to return the indicated value.</summary>
        /// <param name="propertyName">Name of the property to fetch.</param>
        /// <returns>The indicated value or System.Reflection.Missing if the value isn't defined.</returns>
        /// <exception cref="ArgumentException">propertyName</exception>
        public override object? GetValue([CallerMemberName] string propertyName = "")
        {
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentException($"{nameof(propertyName)} is null or empty.", nameof(propertyName));

            if (Values.ContainsKey(propertyName))
                return Values[propertyName];

            return NotSet.Value;
        }

        /// <summary>
        /// Determines whether any objects have unsaved changed in the graph.
        /// </summary>
        /// <returns></returns>
        public bool IsChangedGraph()
        {
            foreach (var item in Values.Values.OfType<IChangeTracking>())
            {
                if (item.IsChanged)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// This property indicates whether or not the associated property was created.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">propertyName;propertyName is null</exception>
        /// <exception cref="ArgumentException">propertyName is empty.;propertyName</exception>
        public override bool IsDefined([CallerMemberName] string propertyName = "")
        {
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentException($"{nameof(propertyName)} is null or empty.", nameof(propertyName));

            return Values.ContainsKey(propertyName);
        }

        /// <summary>
        /// Replaces the current values collection with the original values collection.
        /// </summary>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        public virtual void RejectChanges(bool recursive)
        {
            var needsIsChangedEvent = IsChangedGraph() && !IsChangedLocal;

            //remove properties that no longer exist
            foreach (var item in Values.ToList())
            {
                if (!m_OriginalValues.ContainsKey(item.Key))
                {
                    var property = Metadata.Properties[item.Key];
                    OnPropertyChanging(property);

                    Values.Remove(item.Key);

                    UpdateChangeTrackingEventHandlers(item.Value, null);

                    OnPropertyChanged(property);
                    OnRevalidateProperty(property);
                }
            }

            //update remaining properties
            foreach (var item in m_OriginalValues)
            {
                var currentValue = GetValue(item.Key);
                if (!Equals(currentValue, item.Value))
                {
                    var property = Metadata.Properties[item.Key];
                    OnPropertyChanging(property);

                    Values[item.Key] = item.Value;

                    UpdateChangeTrackingEventHandlers(currentValue, item.Value);

                    OnPropertyChanged(property);
                    OnRevalidateProperty(property);
                }
            }

            if (recursive)
            {
                foreach (var item in Values)
                {
                    if (item.Value is IRevertibleChangeTracking)
                        ((IRevertibleChangeTracking)item.Value).RejectChanges();
                }
            }

            IsChangedLocal = false;
            if (needsIsChangedEvent)
                OnPropertyChanged(Metadata.Properties["IsChanged"]);

            OnRevalidateObject();
        }

        /// <summary>
        /// Implementors need to override this to save the indicated value.
        /// </summary>
        /// <param name="value">The value to be saved. A null will set the value to null.</param>
        /// <param name="mode">Indicates special handling for the action.</param>
        /// <param name="propertyName">Name of property to update</param>
        /// <param name="oldValue">The previously stored value. If the property was uninitialized, this will return NotSet.Value</param>
        /// <returns>
        /// True if the value actually changed
        /// </returns>
        /// <exception cref="ArgumentNullException">propertyName;propertyName is null</exception>
        /// <exception cref="ArgumentException">propertyName is empty.;propertyName</exception>
        /// <remarks>
        /// This will create the property if it doesn't already exist
        /// </remarks>
        public override bool Set(object? value, PropertySetModes mode, string propertyName, out object? oldValue)
        {
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentException($"{nameof(propertyName)} is null or empty.", nameof(propertyName));

            var property = Metadata.Properties[propertyName];

            if ((mode & PropertySetModes.FixCasing) != 0)
                propertyName = property.Name;

            oldValue = GetValue(propertyName);

            if (Equals(oldValue, value))
                return false;

            if ((mode & PropertySetModes.SetAsOriginal) != 0)
                m_OriginalValues[propertyName] = value;

            if ((mode & PropertySetModes.RaiseChangedEvent) != 0)
                OnPropertyChanging(property);

            Values[propertyName] = value;

#pragma warning disable CA1062 // Compiler error, value is allowed to be null
            UpdateChangeTrackingEventHandlers(oldValue, value);
#pragma warning restore CA1062 // Validate arguments of public methods

            if ((mode & PropertySetModes.RaiseChangedEvent) != 0)
                OnPropertyChanged(property);

            if ((mode & PropertySetModes.UpdateIsChangedProperty) != 0)
                IsChangedLocal = true;

            if ((mode & PropertySetModes.ValidateProperty) != 0)
                OnRevalidateProperty(property);

            if ((mode & PropertySetModes.ValidateObject) != 0)
                OnRevalidateObject();

            return true;
        }

        void OnChildIsChangedPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == "IsChanged")
                OnPropertyChanged(Metadata.Properties["IsChanged"]);
        }

        void UpdateChangeTrackingEventHandlers(object? oldValue, object? newValue)
        {
            if (oldValue is IChangeTracking && oldValue is INotifyPropertyChanged)
                if (oldValue is INotifyPropertyChangedWeak notifyPropertyChangedWeak)
                    notifyPropertyChangedWeak.RemoveHandler(m_ChildIsChangedPropertyListener);
                else
                    ((INotifyPropertyChanged)oldValue).PropertyChanged -= OnChildIsChangedPropertyChanged;

            if (newValue is IChangeTracking && newValue is INotifyPropertyChanged)
                if (newValue is INotifyPropertyChangedWeak notifyPropertyChangedWeak2)
                    (notifyPropertyChangedWeak2).AddHandler(m_ChildIsChangedPropertyListener);
                else
                    ((INotifyPropertyChanged)newValue).PropertyChanged += OnChildIsChangedPropertyChanged;
        }
    }
}
