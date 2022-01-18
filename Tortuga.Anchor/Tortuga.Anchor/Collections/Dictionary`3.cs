using System;
using System.Collections;
using System.Collections.Generic;

namespace Tortuga.Anchor.Collections
{
    /// <summary>
    /// A dictionary that uses a compound key.
    /// </summary>
    /// <typeparam name="TKey1">The type of the first key.</typeparam>
    /// <typeparam name="TKey2">The type of the second key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public class Dictionary<TKey1, TKey2, TValue> : IDictionary<ValueTuple<TKey1, TKey2>, TValue>, IReadOnlyDictionary<ValueTuple<TKey1, TKey2>, TValue>
        where TKey1 : IEquatable<TKey1>
        where TKey2 : IEquatable<TKey2>
    {
        readonly Dictionary<ValueTuple<TKey1, TKey2>, TValue> m_Base;
        readonly IDictionary<ValueTuple<TKey1, TKey2>, TValue> m_Interface;

        /// <summary>
        /// Initializes a new instance of the <see cref="Dictionary{TKey1, TKey2, TValue}"/> class.
        /// </summary>
        public Dictionary()
        {
            m_Base = new Dictionary<ValueTuple<TKey1, TKey2>, TValue>();
            m_Interface = m_Base;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Dictionary{TKey1, TKey2, TValue}" /> class.
        /// </summary>
        /// <param name="dictionary">The dictionary to copy.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public Dictionary(IDictionary<ValueTuple<TKey1, TKey2>, TValue> dictionary)
            : this()
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary), $"{nameof(dictionary)} is null.");

            foreach (var item in dictionary)
                m_Interface.Add(item);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Dictionary{TKey1, TKey2, TValue}" /> class.
        /// </summary>
        /// <param name="dictionary">The dictionary to copy.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public Dictionary(Dictionary<ValueTuple<TKey1, TKey2>, TValue> dictionary)
            : this()
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary), $"{nameof(dictionary)} is null.");

            foreach (var item in dictionary)
                m_Interface.Add(item);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Dictionary{TKey1, TKey2, TValue}" /> class.
        /// </summary>
        /// <param name="comparer">The comparer.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public Dictionary(IEqualityComparer<ValueTuple<TKey1, TKey2>> comparer)
        {
            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer), $"{nameof(comparer)} is null.");

            m_Base = new Dictionary<ValueTuple<TKey1, TKey2>, TValue>(comparer);
            m_Interface = m_Base;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Dictionary{TKey1, TKey2, TValue}" /> class.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public Dictionary(int capacity)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity), capacity, $"{nameof(capacity)} cannot be less than zero");

            m_Base = new Dictionary<ValueTuple<TKey1, TKey2>, TValue>(capacity);
            m_Interface = m_Base;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Dictionary{TKey1, TKey2, TValue}" /> class.
        /// </summary>
        /// <param name="dictionary">The dictionary to copy.</param>
        /// <param name="comparer">The comparer.</param>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public Dictionary(IDictionary<ValueTuple<TKey1, TKey2>, TValue> dictionary, IEqualityComparer<ValueTuple<TKey1, TKey2>> comparer)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary), $"{nameof(dictionary)} is null.");
            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer), $"{nameof(comparer)} is null.");

            m_Base = new Dictionary<ValueTuple<TKey1, TKey2>, TValue>(dictionary.Count, comparer);
            m_Interface = m_Base;

            foreach (var item in dictionary)
                m_Interface.Add(item);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Dictionary{TKey1, TKey2, TValue}" /> class.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        /// <param name="comparer">The comparer.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public Dictionary(int capacity, IEqualityComparer<ValueTuple<TKey1, TKey2>> comparer)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity), capacity, $"{nameof(capacity)} cannot be less than zero");
            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer), $"{nameof(comparer)} is null.");

            m_Base = new Dictionary<ValueTuple<TKey1, TKey2>, TValue>(capacity, comparer);
            m_Interface = m_Base;
        }

        /// <summary>
        /// Gets the comparer.
        /// </summary>
        /// <value>The comparer.</value>
        public IEqualityComparer<ValueTuple<TKey1, TKey2>> Comparer
        {
            get { return m_Base.Comparer; }
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="ICollection{T}" />.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the <see cref="ICollection{T}" />.
        /// </returns>
        public int Count
        {
            get { return m_Base.Count; }
        }

#pragma warning disable CA1033 // Interface methods should be callable by child types
        bool ICollection<KeyValuePair<ValueTuple<TKey1, TKey2>, TValue>>.IsReadOnly => false;
#pragma warning restore CA1033 // Interface methods should be callable by child types

        IEnumerable<ValueTuple<TKey1, TKey2>> IReadOnlyDictionary<ValueTuple<TKey1, TKey2>, TValue>.Keys => m_Base.Keys;

        /// <summary>
        /// Gets an <see cref="ICollection{T}" /> containing the keys of the <see cref="IDictionary{TKey,TValue}" />.
        /// </summary>
        /// <returns>
        /// An <see cref="ICollection{T}" /> containing the keys of the object that implements <see cref="IDictionary{TKey,TValue}" />.
        /// </returns>
        public ICollection<ValueTuple<TKey1, TKey2>> Keys => m_Base.Keys;

        IEnumerable<TValue> IReadOnlyDictionary<ValueTuple<TKey1, TKey2>, TValue>.Values => m_Base.Values;

        /// <summary>
        /// Gets an <see cref="ICollection{T}" /> containing the values in the <see cref="IDictionary{TKey,TValue}" />.
        /// </summary>
        /// <returns>
        /// An <see cref="ICollection{T}" /> containing the values in the object that implements <see cref="IDictionary{TKey,TValue}" />.
        /// </returns>
        public ICollection<TValue> Values => m_Base.Values;

#pragma warning disable CA1043 // Use Integral Or String Argument For Indexers

        /// <summary>Gets or sets the element with the specified key.</summary>
        /// <param name="key">The key.</param>
        /// <returns>TValue.</returns>
        public TValue this[ValueTuple<TKey1, TKey2> key]
        {
            get { return m_Base[key]; }
            set { m_Base[key] = value; }
        }

#pragma warning restore CA1043 // Use Integral Or String Argument For Indexers

        /// <summary>
        /// Gets or sets the value with the specified keys.
        /// </summary>
        /// <value>
        ///
        /// </value>
        /// <param name="key1">The first key.</param>
        /// <param name="key2">The second key.</param>
        /// <returns></returns>
        public TValue this[TKey1 key1, TKey2 key2]
        {
            get { return m_Base[(key1, key2)]; }
            set { m_Base[(key1, key2)] = value; }
        }

        /// <summary>
        /// Adds an element with the provided key and value to the <see cref="IDictionary{TKey,TValue}" />.
        /// </summary>
        /// <param name="key">
        /// The object to use as the key of the element to add.
        /// </param>
        /// <param name="value">
        /// The object to use as the value of the element to add.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// An element with the same key already exists in the <see cref="IDictionary{TKey,TValue}" />.
        /// </exception>
        public void Add(ValueTuple<TKey1, TKey2> key, TValue value) => m_Base.Add(key, value);

        /// <summary>
        /// Adds an element with the provided key and value to the <see cref="IDictionary{TKey,TValue}" />.
        /// </summary>
        /// <param name="key1">The first key.</param>
        /// <param name="key2">The second key.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="IDictionary{TKey,TValue}" />.</exception>
        public void Add(TKey1 key1, TKey2 key2, TValue value) => m_Base.Add((key1, key2), value);

        /// <summary>
        /// Adds an item to the <see cref="ICollection{T}" />.
        /// </summary>
        /// <param name="item">
        /// The object to add to the <see cref="ICollection{T}" />.
        /// </param>
        /// The <see cref="ICollection{T}" /> is read-only.
        public void Add(KeyValuePair<ValueTuple<TKey1, TKey2>, TValue> item) => m_Interface.Add(item);

        /// <summary>
        /// Removes all items from the <see cref="ICollection{T}" />.
        /// </summary>
        /// The <see cref="ICollection{T}" /> is read-only.
        public void Clear() => m_Base.Clear();

#pragma warning disable CA1033 // Interface methods should be callable by child types

        bool ICollection<KeyValuePair<ValueTuple<TKey1, TKey2>, TValue>>.Contains(KeyValuePair<ValueTuple<TKey1, TKey2>, TValue> item) => m_Interface.Contains(item);

#pragma warning restore CA1033 // Interface methods should be callable by child types

        /// <summary>
        /// Determines whether the <see cref="IDictionary{TKey,TValue}" /> contains an element with the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="IDictionary{TKey,TValue}" />.</param>
        /// <returns>
        /// true if the <see cref="IDictionary{TKey,TValue}" /> contains an element with the key; otherwise, false.
        /// </returns>
        public bool ContainsKey(ValueTuple<TKey1, TKey2> key) => m_Base.ContainsKey(key);

        /// <summary>
        /// Determines whether the <see cref="IDictionary{TKey,TValue}" /> contains an element with the specified key.
        /// </summary>
        /// <param name="key1">The first key.</param>
        /// <param name="key2">The second key.</param>
        /// <returns>
        /// true if the <see cref="IDictionary{TKey,TValue}" /> contains an element with the key; otherwise, false.
        /// </returns>
        public bool ContainsKey(TKey1 key1, TKey2 key2) => m_Base.ContainsKey((key1, key2));

        /// <summary>Copies the elements of the <see cref="ICollection{T}" /> to an <see cref="Array" />, starting at a particular <see cref="Array" /> index.</summary>
        /// <param name="array">The one-dimensional <see cref="Array" /> that is the destination of the elements copied from <see cref="ICollection{T}" />. The <see cref="Array" /> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="array" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="arrayIndex" /> is less than 0.</exception>
        /// <exception cref="ArgumentException">The number of elements in the source <see cref="ICollection{T}" /> is greater than the available space from <paramref name="arrayIndex" /> to the end of the destination <paramref name="array" />.</exception>
        public void CopyTo(KeyValuePair<ValueTuple<TKey1, TKey2>, TValue>[] array, int arrayIndex) => m_Interface.CopyTo(array, arrayIndex);

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="IEnumerator{T}" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<KeyValuePair<ValueTuple<TKey1, TKey2>, TValue>> GetEnumerator() => m_Base.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => m_Base.GetEnumerator();

        /// <summary>
        /// Removes the element with the specified key from the <see cref="IDictionary{TKey,TValue}" />.
        /// </summary>
        /// <returns>
        /// true if the element is successfully removed; otherwise, false.  This method also returns false if <paramref name="key" /> was not found in the original <see cref="IDictionary{TKey,TValue}" />.
        /// </returns>
        /// <param name="key">
        /// The key of the element to remove.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key" /> is null.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="IDictionary{TKey,TValue}" /> is read-only.
        /// </exception>
        public bool Remove(ValueTuple<TKey1, TKey2> key) => m_Base.Remove(key);

        /// <summary>
        /// Removes the element with the specified key from the <see cref="IDictionary{TKey,TValue}" />.
        /// </summary>
        /// <param name="key1">The first key.</param>
        /// <param name="key2">The second key.</param>
        /// <returns>
        /// true if the element is successfully removed; otherwise, false.  This method also returns false if <paramref name="key1" />+<paramref name="key2" /> was not found in the original <see cref="IDictionary{TKey,TValue}" />
        /// </returns>
        public bool Remove(TKey1 key1, TKey2 key2) => m_Base.Remove((key1, key2));

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="ICollection{T}" />.
        /// </summary>
        /// <returns>
        /// true if <paramref name="item" /> was successfully removed from the <see cref="ICollection{T}" />; otherwise, false. This method also returns false if <paramref name="item" /> is not found in the original <see cref="ICollection{T}" />.
        /// </returns>
        /// <param name="item">
        /// The object to remove from the <see cref="ICollection{T}" />.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// The <see cref="ICollection{T}" /> is read-only.
        /// </exception>
        public bool Remove(KeyValuePair<ValueTuple<TKey1, TKey2>, TValue> item) => m_Interface.Remove(item);

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key whose value to get.</param>
        /// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value" /> parameter. This parameter is passed uninitialized.</param>
        /// <returns>
        /// true if the object that implements <see cref="IDictionary{TKey,TValue}" /> contains an element with the specified key; otherwise, false.
        /// </returns>
#pragma warning disable CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
        public bool TryGetValue(ValueTuple<TKey1, TKey2> key, out TValue? value) => m_Base.TryGetValue(key, out value);
#pragma warning restore CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key1">The first key.</param>
        /// <param name="key2">The second key.</param>
        /// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value" /> parameter. This parameter is passed uninitialized.</param>
        /// <returns> true if the object that implements <see cref="IDictionary{TKey,TValue}" /> contains an element with the specified key; otherwise, false.</returns>
        public bool TryGetValue(TKey1 key1, TKey2 key2, out TValue? value) => m_Base.TryGetValue((key1, key2), out value);
    }
}
