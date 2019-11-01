using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Tortuga.Anchor.Metadata;

namespace Tortuga.Anchor.Modeling.Internals
{
    /// <summary>
    /// This is the template for a model's backing store.
    /// </summary>
    public abstract class PropertyBagBase : INotifyPropertyChanged
    {
        /// <summary>
        /// This is the template for a model's backing store.
        /// </summary>
        /// <param name="owner">Owning model, used to fetch metadata</param>
        /// <exception cref="ArgumentNullException">owner;owner is null.</exception>
        protected PropertyBagBase(object owner)
        {
            if (owner == null)
                throw new ArgumentNullException(nameof(owner), $"{nameof(owner)} is null.");

            Metadata = MetadataCache.GetMetadata(owner.GetType());
        }

        /// <summary>
        /// This fires for each changed property.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Occurs just before a property value is changed.
        /// </summary>
        public event PropertyChangingEventHandler? PropertyChanging;

        /// <summary>
        /// This fires after one or more properties have been changed and their corresponding RevalidateProperty events have been handled.
        /// </summary>
        public event EventHandler? RevalidateObject;

        /// <summary>
        /// This fires after one or more properties have been changed.
        /// </summary>
        public event PropertyChangedEventHandler? RevalidateProperty;

        /// <summary>
        /// Cached metadata for the associated model.
        /// </summary>
        /// <value>
        /// The metadata for the associated model.
        /// </value>
        public ClassMetadata Metadata { get; }

        /// <summary>
        /// Fetches a value, returning Default(T) if it doesn't exist.
        /// </summary>
        /// <typeparam name="T">Expected type</typeparam>
        /// <param name="propertyName">Case-sensitive property name</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">propertyName;propertyName is null</exception>
        /// <exception cref="ArgumentException">propertyName is empty;propertyName</exception>
        /// <exception cref="InvalidOperationException">Property  + propertyName +  contains a null</exception>
        /// <remarks>
        /// Storing the default value will trigger validation but not a property-changed event.
        /// </remarks>
#nullable disable

        public T Get<T>([CallerMemberName] string propertyName = "")
        {
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentException($"{nameof(propertyName)} is null or empty.", nameof(propertyName));

            var result = GetValue(propertyName);
            if (result == NotSet.Value)
            {
                result = default(T);
                Set(result, PropertySetModes.SetAsOriginal, propertyName);
            }
            try
            {
                return (T)result;
            }
            catch (NullReferenceException ex)
            {
                //This can occur if T is a non-nullable value type and the parent class is mal-formed.
                throw new InvalidOperationException("Property " + propertyName + " contains a null", ex);
            }
        }

#nullable restore

        /// <summary>
        /// Fetches a value, using the default value it if it doesn't exist.
        /// </summary>
        /// <typeparam name="T">Expected type</typeparam>
        /// <param name="defaultValue">Default value to use</param>
        /// <param name="propertyName">Case-sensitive property name</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">propertyName;propertyName is null</exception>
        /// <exception cref="ArgumentException">propertyName is empty;propertyName</exception>
        /// <exception cref="InvalidOperationException">Property  + propertyName +  contains a null</exception>
        /// <remarks>
        /// Storing the default value will trigger validation but not a property-changed event.
        /// </remarks>
#nullable disable

        public T GetDefault<T>(T defaultValue, [CallerMemberName] string propertyName = "")
        {
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentException($"{nameof(propertyName)} is null or empty.", nameof(propertyName));

            var result = GetValue(propertyName);
            if (result == NotSet.Value)
            {
                result = defaultValue;
                Set(result, PropertySetModes.SetAsOriginal, propertyName);
            }
            try
            {
                return (T)result;
            }
            catch (NullReferenceException ex)
            {
                //This can occur if T is a non-nullable value type and the parent class is mal-formed.
                throw new InvalidOperationException("Property " + propertyName + " contains a null", ex);
            }
        }



#nullable restore

        /// <summary>
        /// Access to the values dictionary for sub-classes. Extreme care must be taken when working this dictionary directly, as events will not be automatically fired.
        /// </summary>
        /// <value>
        /// The values.
        /// </value>
        protected internal Dictionary<string, object?> Values { get; } = new Dictionary<string, object?>(StringComparer.Ordinal);

        /// <summary>
        /// Overrides the value dictionary.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <remarks>This should only be called on newly created objects</remarks>
        internal void SetValues(Dictionary<string, object?> source)
        {
            Values.Clear();
            foreach (var item in source)
                Set(item.Value, item.Key);
        }

        /// <summary>
        /// Fetches a value, creating it if it doesn't exist.
        /// </summary>
        /// <typeparam name="T">Expected type</typeparam>
        /// <param name="creationFunction">Function to execute if the property doesn't already exist.</param>
        /// <param name="propertyName">Case-sensitive property name</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// creationFunction;creationFunction is null.
        /// or
        /// propertyName;propertyName is null
        /// </exception>
        /// <exception cref="ArgumentException">propertyName is empty.;propertyName</exception>
        /// <exception cref="InvalidOperationException">Property  + propertyName +  contains a null</exception>
        /// <remarks>
        /// Executing the default function will trigger validation but not a property-changed event.
        /// </remarks>
#nullable disable

        public T GetNew<T>(Func<T> creationFunction, [CallerMemberName] string propertyName = "")
        {
            if (creationFunction == null)
                throw new ArgumentNullException(nameof(creationFunction), $"{nameof(creationFunction)} is null.");
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentException($"{nameof(propertyName)} is null or empty.", nameof(propertyName));

            var result = GetValue(propertyName);
            if (result == NotSet.Value)
            {
                result = creationFunction();
                Set(result, PropertySetModes.SetAsOriginal, propertyName);
            }
            try
            {
                return (T)result;
            }
            catch (NullReferenceException ex)
            {
                //This can occur if T is a non-nullable value type and the parent class is mal-formed.
                throw new InvalidOperationException("Property " + propertyName + " contains a null", ex);
            }
        }

#nullable restore

        /// <summary>
        /// Fetches a value, creating it if it doesn't exist.
        /// </summary>
        /// <typeparam name="T">Expected type that has a parameterless constructor</typeparam>
        /// <param name="propertyName">Case-sensitive property name</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">propertyName;propertyName is null</exception>
        /// <exception cref="ArgumentException">propertyName is empty;propertyName</exception>
        /// <exception cref="InvalidOperationException">Property  + propertyName +  contains a null</exception>
        /// <remarks>
        /// Executing the constructor will trigger validation but not a property-changed event.
        /// </remarks>
#nullable disable

        public T GetNew<T>([CallerMemberName] string propertyName = "")
                        where T : new()
        {
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentException($"{nameof(propertyName)} is null or empty.", nameof(propertyName));

            var result = GetValue(propertyName);
            if (result == NotSet.Value)
            {
                result = Activator.CreateInstance<T>();
                Set(result, PropertySetModes.SetAsOriginal, propertyName);
            }
            try
            {
                return (T)result;
            }
            catch (NullReferenceException ex)
            {
                //This can occur if T is a non-nullable value type and the parent class is mal-formed.
                throw new InvalidOperationException("Property " + propertyName + " contains a null", ex);
            }
        }

#nullable restore

        /// <summary>
        /// Implementors need to override this to return the indicated value.
        /// </summary>
        /// <param name="propertyName">Name of the property to fetch.</param>
        /// <returns>
        /// The indicated value or System.Reflection.Missing if the value isn't defined.
        /// </returns>

        public abstract object? GetValue([CallerMemberName] string propertyName = "");

        /// <summary>
        /// This property indicates whether or not the associated property was created.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>

        public abstract bool IsDefined([CallerMemberName] string propertyName = "");

        /// <summary>
        /// Uses the property metadata to trigger validation on all model properties. Then requests validation on the object itself.
        /// </summary>
        public void RevalidateAll()
        {
            foreach (var property in Metadata.Properties)
            {
                OnRevalidateProperty(property);
            }
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
        /// <remarks>
        /// This will create the property if it doesn't already exist
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "<Pending>")]
        public abstract bool Set(object? value, PropertySetModes mode, string propertyName, out object? oldValue);

        /// <summary>
        /// Set the indicated property to the value.
        /// If the value doesn't match the previous value, or if there is no previous value, raise a property changed notification.
        /// </summary>
        /// <param name="value">Value to be saved.</param>
        /// <param name="mode">Indicates special handling for the action.</param>
        /// <param name="propertyName">Name of property to be created/updated</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">propertyName;propertyName is null</exception>
        /// <exception cref="ArgumentException">propertyName is empty.;propertyName</exception>
        public bool Set(object? value, PropertySetModes mode, [CallerMemberName] string propertyName = "")
        {
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentException($"{nameof(propertyName)} is null or empty.", nameof(propertyName));

            return Set(value, mode, propertyName, out var _);
        }

        /// <summary>
        /// Set the indicated property to the value.
        /// If the value doesn't match the previous value, or if there is no previous value, raise a property changed notification.
        /// </summary>
        /// <param name="value">Value to be saved.</param>
        /// <param name="propertyName">Name of property to be created/updated</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">propertyName;propertyName is null</exception>
        /// <exception cref="ArgumentException">propertyName is empty.;propertyName</exception>
        public bool Set(object? value, [CallerMemberName] string propertyName = "")
        {
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentException($"{nameof(propertyName)} is null or empty.", nameof(propertyName));

            return Set(value, PropertySetModes.RaiseChangedEvent | PropertySetModes.ValidateProperty | PropertySetModes.ValidateObject | PropertySetModes.UpdateIsChangedProperty, propertyName, out var _);
        }

        /// <summary>
        /// Set the indicated property to the value.
        /// If the value doesn't match the previous value, or if there is no previous value, raise a property changed notification.
        /// </summary>
        /// <param name="value">Value to be saved.</param>
        /// <param name="valueChanged">Delegate that will be invoked if the value of this property changes.</param>
        /// <param name="propertyName">Name of property to be created/updated</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">propertyName;propertyName is null</exception>
        /// <exception cref="ArgumentException">valueChanged is empty.;valueChanged</exception>
        /// <exception cref="ArgumentException">propertyName is empty.;propertyName</exception>
#nullable disable

        public bool Set<T>(T value, ValueChanged<T> valueChanged, [CallerMemberName] string propertyName = "")
        {
            if (valueChanged == null)
                throw new ArgumentNullException(nameof(valueChanged), $"{nameof(valueChanged)} is null.");
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentException($"{nameof(propertyName)} is null or empty.", nameof(propertyName));

            var result = Set(value, PropertySetModes.RaiseChangedEvent | PropertySetModes.ValidateProperty | PropertySetModes.ValidateObject | PropertySetModes.UpdateIsChangedProperty, propertyName, out var oldValue);

            if (result)
                if (oldValue == NotSet.Value)
                    valueChanged(default, value);
                else
                    valueChanged((T)oldValue, value);

            return result;
        }

#nullable restore

        /// <summary>
        /// Set the indicated property to the value.
        /// If the value doesn't match the previous value, or if there is no previous value, raise a property changed notification.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">Value to be saved.</param>
        /// <param name="propertyChanged">A property changed event handler to be attached to the new value. If an old value exists, the event handler will be removed from it.</param>
        /// <param name="propertyName">Name of property to be created/updated</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// propertyName;propertyName is null
        /// or
        /// propertyChanged;propertyChanged is null.
        /// </exception>
        /// <exception cref="ArgumentException">propertyName is empty.;propertyName</exception>
        public bool Set<T>(T value, PropertyChangedEventHandler propertyChanged, [CallerMemberName] string propertyName = "")
            where T : INotifyPropertyChanged
        {
            if (propertyChanged == null)
                throw new ArgumentNullException(nameof(propertyChanged), $"{nameof(propertyChanged)} is null.");
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentException($"{nameof(propertyName)} is null or empty.", nameof(propertyName));

            var result = Set(value, PropertySetModes.RaiseChangedEvent | PropertySetModes.ValidateProperty | PropertySetModes.ValidateObject | PropertySetModes.UpdateIsChangedProperty, propertyName, out var oldValue);

            if (oldValue is INotifyPropertyChanged notifyPropertyChanged)
                notifyPropertyChanged.PropertyChanged -= propertyChanged;

            if (value != null)
                value.PropertyChanged += propertyChanged;

            return result;
        }

        /// <summary>
        /// Set the indicated property to the value.
        /// If the value doesn't match the previous value, or if there is no previous value, raise a property changed notification.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">Value to be saved.</param>
        /// <param name="collectionChanged">A collection changed event handler to be attached to the new value. If an old value exists, the event handler will be removed from it.</param>
        /// <param name="propertyName">Name of property to be created/updated</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// propertyName;propertyName is null
        /// or
        /// collectionChanged;collectionChanged is null.
        /// </exception>
        /// <exception cref="ArgumentException">propertyName is empty.;propertyName</exception>
        public bool Set<T>(T value, NotifyCollectionChangedEventHandler collectionChanged, [CallerMemberName] string propertyName = "")
            where T : INotifyCollectionChanged
        {
            if (collectionChanged == null)
                throw new ArgumentNullException(nameof(collectionChanged), $"{nameof(collectionChanged)} is null.");
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentException($"{nameof(propertyName)} is null or empty.", nameof(propertyName));

            var result = Set(value, PropertySetModes.RaiseChangedEvent | PropertySetModes.ValidateProperty | PropertySetModes.ValidateObject | PropertySetModes.UpdateIsChangedProperty, propertyName, out var oldValue);

            if (oldValue is INotifyCollectionChanged notifyCollectionChanged)
                notifyCollectionChanged.CollectionChanged -= collectionChanged;

            if (value != null)
                value.CollectionChanged += collectionChanged;

            return result;
        }

        /// <summary>
        /// Triggers the PropertyChanged event.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <exception cref="ArgumentNullException">property;property is null.</exception>
        protected internal void OnPropertyChanged(PropertyMetadata property)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property), $"{nameof(property)} is null.");

            if (PropertyChanged == null)
                return;

            PropertyChanged.Invoke(this, property.PropertyChangedEventArgs);

            if (property.AffectsCalculatedFields)
                foreach (var affectedProperty in property.CalculatedFields)
                {
                    OnPropertyChanged(affectedProperty);
                }
        }

        /// <summary>
        /// Triggers the PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected internal void OnPropertyChanged(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                PropertyChanged?.Invoke(this, CommonProperties.Empty);
            else
                OnPropertyChanged(Metadata.Properties[propertyName]);
        }

        /// <summary>
        /// Triggers the PropertyChanging event.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <exception cref="ArgumentNullException">property;property is null.</exception>
        protected internal void OnPropertyChanging(PropertyMetadata property)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property), $"{nameof(property)} is null.");

            if (PropertyChanging == null)
                return;

            PropertyChanging.Invoke(this, property.PropertyChangingEventArgs);

            if (property.AffectsCalculatedFields)
                foreach (var affectedProperty in property.CalculatedFields)
                {
                    OnPropertyChanging(affectedProperty);
                }
        }

        /// <summary>
        /// Triggers the PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected internal void OnPropertyChanging(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentException($"{nameof(propertyName)} is null or empty.", nameof(propertyName));

            OnPropertyChanging(Metadata.Properties[propertyName]);
        }

        /// <summary>
        /// Triggers the RevalidateObject event.
        /// </summary>
        protected virtual void OnRevalidateObject()
        {
            RevalidateObject?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Triggers the RevalidateProperty event.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <exception cref="ArgumentNullException">property;property is null.</exception>
        protected void OnRevalidateProperty(PropertyMetadata property)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property), $"{nameof(property)} is null.");

            RevalidateProperty?.Invoke(this, property.PropertyChangedEventArgs);
        }
    }
}
