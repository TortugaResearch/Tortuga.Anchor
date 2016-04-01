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
        private IListener<PropertyChangedEventArgs> m_ChildIsChangedPropertyListener;
        private readonly Dictionary<string, object> m_OriginalValues = new Dictionary<string, object>(StringComparer.Ordinal);
        private readonly Dictionary<string, object> m_Values = new Dictionary<string, object>(StringComparer.Ordinal);
        private bool m_IsChangedLocal;

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
        /// Marks all fields as unchanged by storing them in the original values collection.
        /// </summary>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        public virtual void AcceptChanges(bool recursive)
        {
            m_OriginalValues.Clear();

            foreach (var item in m_Values)
            {
                m_OriginalValues[item.Key] = item.Value;
                if (recursive && item.Value is IChangeTracking)
                    ((IChangeTracking)item.Value).AcceptChanges();
            }

            IsChangedLocal = false;
        }

        /// <summary>
        /// Implementors need to override this to return the indicated value.
        /// </summary>
        /// <param name="propertyName">Name of the property to fetch.</param>
        /// <returns>
        /// The indicated value or System.Reflection.Missing if the value isn't defined.
        /// </returns>
        /// <exception cref="ArgumentNullException">propertyName;propertyName is null</exception>
        /// <exception cref="ArgumentException">propertyName is empty.;propertyName</exception>

        public override object GetValue([CallerMemberName] string propertyName = null)
        {
            if (propertyName == null)
                throw new ArgumentNullException("propertyName", "propertyName is null");
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentException("propertyName is empty.", "propertyName");

            if (m_Values.ContainsKey(propertyName))
                return m_Values[propertyName];

            return NotSet.Value;
        }

        /// <summary>
        /// This property indicates whether or not the associated property was created.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">propertyName;propertyName is null</exception>
        /// <exception cref="ArgumentException">propertyName is empty.;propertyName</exception>

        public override bool IsDefined([CallerMemberName] string propertyName = null)
        {
            if (propertyName == null)
                throw new ArgumentNullException("propertyName", "propertyName is null");
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentException("propertyName is empty.", "propertyName");

            return m_Values.ContainsKey(propertyName);
        }

        /// <summary>
        /// Replaces the current values collection with the original values collection.
        /// </summary>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        public virtual void RejectChanges(bool recursive)
        {
            var needsIsChangedEvent = IsChangedGraph() && !IsChangedLocal;

            //remove properties that no longer exist
            foreach (var item in m_Values.ToList())
            {
                if (!m_OriginalValues.ContainsKey(item.Key))
                {
                    m_Values.Remove(item.Key);

                    UpdateChangeTrackingEventHandlers(item.Value, null);

                    var property = Metadata.Properties[item.Key];
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
                    m_Values[item.Key] = item.Value;

                    UpdateChangeTrackingEventHandlers(currentValue, item.Value);

                    var property = Metadata.Properties[item.Key];
                    OnPropertyChanged(property);
                    OnRevalidateProperty(property);
                }
            }

            if (recursive)
            {
                foreach (var item in m_Values)
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

        private void UpdateChangeTrackingEventHandlers(object oldValue, object newValue)
        {
            if (oldValue is IChangeTracking && oldValue is INotifyPropertyChanged)
                if (oldValue is INotifyPropertyChangedWeak)
                    ((INotifyPropertyChangedWeak)oldValue).RemoveHandler(m_ChildIsChangedPropertyListener);
                else
                    ((INotifyPropertyChanged)oldValue).PropertyChanged -= OnChildIsChangedPropertyChanged;

            if (newValue is IChangeTracking && newValue is INotifyPropertyChanged)
                if (newValue is INotifyPropertyChangedWeak)
                    ((INotifyPropertyChangedWeak)newValue).AddHandler(m_ChildIsChangedPropertyListener);
                else
                    ((INotifyPropertyChanged)newValue).PropertyChanged += OnChildIsChangedPropertyChanged;
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
        public override bool Set(object value, PropertySetModes mode, string propertyName, out object oldValue)
        {
            if (propertyName == null)
                throw new ArgumentNullException("propertyName", "propertyName is null");
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentException("propertyName is empty.", "propertyName");

            var property = Metadata.Properties[propertyName];

            if (mode.HasFlag(PropertySetModes.FixCasing))
                propertyName = property.Name;

            oldValue = GetValue(propertyName);

            if (Equals(oldValue, value))
                return false;

            if (mode.HasFlag(PropertySetModes.SetAsOriginal))
                m_OriginalValues[propertyName] = value;

            m_Values[propertyName] = value;

            UpdateChangeTrackingEventHandlers(oldValue, value);

            if (mode.HasFlag(PropertySetModes.RaiseChangedEvent))
                OnPropertyChanged(property);

            if (mode.HasFlag(PropertySetModes.UpdateIsChangedProperty))
                IsChangedLocal = true;

            if (mode.HasFlag(PropertySetModes.ValidateProperty))
                OnRevalidateProperty(property);

            if (mode.HasFlag(PropertySetModes.ValidateObject))
                OnRevalidateObject();

            return true;
        }

        private void OnChildIsChangedPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == "IsChanged")
                OnPropertyChanged(Metadata.Properties["IsChanged"]);
        }

        /// <summary>
        /// Access to the values dictionary for sub-classes. Extreme care must be taken when working this this dictionary directly, as events will not be automatically fired.
        /// </summary>
        /// <value>
        /// The values.
        /// </value>
        protected Dictionary<string, object> Values
        {
            get { return m_Values; }
        }

        /// <summary>
        /// Determines whether any objects have unsaved changed in the graph.
        /// </summary>
        /// <returns></returns>

        internal bool IsChangedGraph()
        {
            foreach (var item in m_Values.Values.OfType<IChangeTracking>())
            {
                if (item.IsChanged)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Returns True if any fields were modified since the last call to AcceptChanges. This does not walk the object graph.
        /// </summary>
        /// <returns>true if the object’s content has changed since the last call to <see cref="M:System.ComponentModel.IChangeTracking.AcceptChanges" />; otherwise, false.</returns>
        public bool IsChangedLocal
        {
            get { return m_IsChangedLocal; }
            set
            {
                if (m_IsChangedLocal == value)
                    return;
                m_IsChangedLocal = value;

                OnPropertyChanged(Metadata.Properties["IsChangedLocal"]);
                OnPropertyChanged(Metadata.Properties["IsChanged"]);
            }
        }


        /// <summary>
        /// Gets the previous value for the indicated property.
        /// </summary>
        /// <param name="propertyName">
        /// Name of the property.
        /// </param>
        public object GetPreviousValue(string propertyName)
        {
            object result;
            if (m_OriginalValues.TryGetValue(propertyName, out result))
                return result;

            return NotSet.Value;
        }

        /// <summary>
        /// List of changed properties.
        /// </summary>
        public IReadOnlyList<string> ChangedProperties()
        {
            var result = new List<string>();
            //update remaining properties
            foreach (var item in m_OriginalValues)
            {
                var currentValue = GetValue(item.Key);
                if (!Equals(currentValue, item.Value))
                    result.Add(item.Key);
            }
            return new ReadOnlyCollection<string>(result);
        }
    }


}

