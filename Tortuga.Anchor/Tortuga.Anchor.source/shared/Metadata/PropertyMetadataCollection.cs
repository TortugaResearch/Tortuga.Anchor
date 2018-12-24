using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;

namespace Tortuga.Anchor.Metadata
{
    /// <summary>
    /// Immutable collection of PropertyMetadata
    /// </summary>
    sealed public class PropertyMetadataCollection : IList<PropertyMetadata>
    {
        readonly Dictionary<string, PropertyMetadata> m_Base = new Dictionary<string, PropertyMetadata>(StringComparer.OrdinalIgnoreCase);
        readonly Dictionary<string, PropertyMetadata> m_Int32IndexedProperties = new Dictionary<string, PropertyMetadata>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// This is used when we need to iterate over all of the properties as quickly as possible.
        /// </summary>
        readonly ImmutableArray<PropertyMetadata> m_QuickList;

        readonly Dictionary<string, PropertyMetadata> m_StringIndexedProperties = new Dictionary<string, PropertyMetadata>(StringComparer.OrdinalIgnoreCase);

        internal PropertyMetadataCollection(IEnumerable<PropertyMetadata> properties)
        {
            foreach (var value in properties)
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
            m_QuickList = ImmutableArray.CreateRange(m_Base.Values);
        }

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

        PropertyMetadata IList<PropertyMetadata>.this[int index]
        {
            get { return m_QuickList[index]; }

            set { throw new NotSupportedException("This collection is read-only."); }
        }

        /// <summary>
        /// Gets the <see cref="PropertyMetadata"/> at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>PropertyMetadata.</returns>
        public PropertyMetadata this[int index]
        {
            get { return m_QuickList[index]; }
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

                if (m_Base.TryGetValue(propertyName, out PropertyMetadata result))
                    return result;

                if (m_Int32IndexedProperties.TryGetValue(propertyName, out result))
                    return result;

                if (m_StringIndexedProperties.TryGetValue(propertyName, out result))
                    return result;

                throw new ArgumentOutOfRangeException(nameof(propertyName), propertyName, $"The property {propertyName} was not found");
            }
        }

        void ICollection<PropertyMetadata>.Add(PropertyMetadata item)
        {
            throw new NotSupportedException("This collection is read-only.");
        }

        void ICollection<PropertyMetadata>.Clear()
        {
            throw new NotSupportedException("This collection is read-only.");
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

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_Base.Values.GetEnumerator();
        }

        int IList<PropertyMetadata>.IndexOf(PropertyMetadata item)
        {
            return m_QuickList.IndexOf(item);
        }

        void IList<PropertyMetadata>.Insert(int index, PropertyMetadata item)
        {
            throw new NotSupportedException("This collection is read-only.");
        }

        bool ICollection<PropertyMetadata>.Remove(PropertyMetadata item)
        {
            throw new NotSupportedException("This collection is read-only.");
        }

        void IList<PropertyMetadata>.RemoveAt(int index)
        {
            throw new NotSupportedException("This collection is read-only.");
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
    }
}