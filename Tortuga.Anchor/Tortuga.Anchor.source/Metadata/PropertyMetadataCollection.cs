using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Tortuga.Anchor.Metadata
{
    /// <summary>
    /// Immutable collection of PropertyMetadata
    /// </summary>
    sealed public class PropertyMetadataCollection : ICollection<PropertyMetadata>
    {
        private readonly Dictionary<string, PropertyMetadata> m_Base = new Dictionary<string, PropertyMetadata>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, PropertyMetadata> m_Int32IndexedProperties = new Dictionary<string, PropertyMetadata>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, PropertyMetadata> m_StringIndexedProperties = new Dictionary<string, PropertyMetadata>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Returns the number of known properties
        /// </summary>
        public int Count
        {
            get { return m_Base.Count; }
        }

        bool ICollection<PropertyMetadata>.IsReadOnly
        {
            get { return true; }
        }

        /// <summary>
        /// Returns a copy of the list of known property names.
        /// </summary>

        public ReadOnlyCollection<string> PropertyNames
        {
            get { return new ReadOnlyCollection<string>(m_Base.Keys.ToList()); }
        }

        /// <summary>
        /// Attempts to fetch property metadata for the indicated property. Will throw an error if not found.
        /// </summary>
        /// <param name="propertyName">
        /// Case insensitive property name.
        /// For indexed properties the parameter types should appear inside brackets. For example, "Item [Int32]".
        /// Note: "Item[]" will be mapped to "Item [Int32]"
        /// </param>
        /// <returns></returns>
        public PropertyMetadata this[string propertyName]
        {
            get
            {
                if (string.IsNullOrEmpty(propertyName))
                    throw new ArgumentException($"{nameof(propertyName)} is null or empty.", nameof(propertyName));

                PropertyMetadata result;
                if (m_Base.TryGetValue(propertyName, out result))
                {
                    return result;
                }
                if (m_Int32IndexedProperties.TryGetValue(propertyName, out result))
                {
                    return result;
                }
                if (m_StringIndexedProperties.TryGetValue(propertyName, out result))
                {
                    return result;
                }

                throw new ArgumentOutOfRangeException("propertyName", propertyName, "The property " + propertyName + " was not found");
            }
        }

        /// <summary>
        /// Returns true if the property is known
        /// </summary>
        ///<param name="item">item to look for</param>
        /// <returns></returns>        
        public bool Contains(PropertyMetadata item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), $"{nameof(item)} is null.");
            if (Count == 0)
                return false;
            return Contains(item.Name);
        }

        /// <summary>
        /// Returns true if the property is known
        /// </summary>
        /// <param name="propertyName">case insensitive property name</param>
        /// <returns></returns>
        public bool Contains(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentException($"{nameof(propertyName)} is null or empty.", nameof(propertyName));

            return m_Base.ContainsKey(propertyName) || m_Int32IndexedProperties.ContainsKey(propertyName) || m_StringIndexedProperties.ContainsKey(propertyName);
        }

        /// <summary>
        ///  Copies the collection elements to an existing array
        /// </summary>
        /// <param name="array">The one-dimensional System.Array that is the destination of the elements</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>

        public void CopyTo(PropertyMetadata[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array), $"{nameof(array)} is null");
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), $"{nameof(arrayIndex)} cannot be less than zero");
            if (arrayIndex > array.Length)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), $"{nameof(arrayIndex)} is greater than the array's length");
            if (Count + arrayIndex > array.Length)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), $"{nameof(Count)} + {nameof(arrayIndex)} is greater than the array's length");


            m_Base.Values.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns></returns>

        public IEnumerator<PropertyMetadata> GetEnumerator()
        {
            return m_Base.Values.GetEnumerator();
        }

        void ICollection<PropertyMetadata>.Clear()
        {
            throw new NotSupportedException("This collection is read-only.");
        }

        bool ICollection<PropertyMetadata>.Remove(PropertyMetadata item)
        {
            throw new NotSupportedException("This collection is read-only.");
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_Base.Values.GetEnumerator();
        }

        /// <summary>
        /// Attempts to fetch property metadata for the indicated property. Will not throw an error if not found.
        /// </summary>
        /// <param name="propertyName">case insensitive property name</param>
        /// <param name="value"></param>
        /// <returns></returns>

        public bool TryGetValue(string propertyName, out PropertyMetadata value)
        {
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentException($"{nameof(propertyName)} is null or empty.", nameof(propertyName));

            if (m_Base.TryGetValue(propertyName, out value))
                return true;
            if (m_Int32IndexedProperties.TryGetValue(propertyName, out value))
                return true;
            if (m_StringIndexedProperties.TryGetValue(propertyName, out value))
                return true;

            value = null;
            return false;
        }

        void ICollection<PropertyMetadata>.Add(PropertyMetadata item)
        {
            throw new NotSupportedException("This collection is read-only.");
        }

        /// <summary>
        /// Adds a property to the collection
        /// </summary>
        /// <param name="value"></param>
        internal void Add(PropertyMetadata value)
        {
            m_Base.Add(value.Name, value);

            if (value.IsIndexed)
            {
                if (value.Name.EndsWith(" [Int32]", StringComparison.Ordinal))
                    m_Int32IndexedProperties.Add(value.PropertyChangedEventArgs.PropertyName, value);
                else if (value.Name.EndsWith(" [System.String]", StringComparison.Ordinal))
                    m_StringIndexedProperties.Add(value.PropertyChangedEventArgs.PropertyName, value);
            }
        }
    }
}
