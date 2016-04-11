using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Tortuga.Anchor.Modeling.Internals
{

    /// <summary>
    /// This is the simplest implementation of PropertyBagBase. It supports normal property-change notifications and validation events.
    /// </summary>
    public class PropertyBag : PropertyBagBase
    {
        private readonly Dictionary<string, object> m_Values = new Dictionary<string, object>(StringComparer.Ordinal);

        /// <summary>
        /// This is the simplest implementation of PropertyBagBase. It supports normal property-change notifications and validation events.
        /// </summary>
        /// <param name="owner">Pass-through to PropertyBagBase</param>

        public PropertyBag(object owner)
            : base(owner)
        { }

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
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentException($"{nameof(propertyName)} is null or empty.", nameof(propertyName));

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
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentException($"{nameof(propertyName)} is null or empty.", nameof(propertyName));

            return m_Values.ContainsKey(propertyName);
        }

        /// <summary>
        /// Implementors need to override this to save the indicated value.
        /// </summary>
        /// <param name="value">The value to be saved. A null will set the value to null. </param>
        /// <param name="mode">Indicates special handling for the action. Ignores SetAsOriginal.</param>
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
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentException($"{nameof(propertyName)} is null or empty.", nameof(propertyName));

            var property = Metadata.Properties[propertyName];

            if (mode.HasFlag(PropertySetModes.FixCasing))
                propertyName = property.Name;

            oldValue = GetValue(propertyName);

            if (Equals(oldValue, value))
                return false;

            m_Values[propertyName] = value;

            if (mode.HasFlag(PropertySetModes.RaiseChangedEvent))
                OnPropertyChanged(property);

            if (mode.HasFlag(PropertySetModes.ValidateProperty))
                OnRevalidateProperty(property);

            if (mode.HasFlag(PropertySetModes.ValidateObject))
                OnRevalidateObject();

            return true;
        }

    }

}
