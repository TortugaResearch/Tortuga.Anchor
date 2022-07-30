using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace Tortuga.Anchor.Collections;

/// <summary>
/// This is a read-only wrapper around a MultiValueDictionary.
/// Implements the <see cref="IReadOnlyDictionary{TKey, TValue}" />
/// </summary>
/// <typeparam name="TKey">The type of the key.</typeparam>
/// <typeparam name="TValue">The type of the value.</typeparam>
/// <seealso cref="IReadOnlyDictionary{TKey, TValue}" />
public class ReadOnlyMultiValueDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, ReadOnlyCollection<TValue>>
	where TKey : notnull
{
	readonly MultiValueDictionary<TKey, TValue> m_Dictionary;

	/// <summary>
	/// Initializes a new instance of the <see cref="MultiValueDictionary{TKey, TValue}" /> class.
	/// </summary>
	/// <param name="dictionary">The dictionary to wrap.</param>
	public ReadOnlyMultiValueDictionary(MultiValueDictionary<TKey, TValue> dictionary)
	{
		m_Dictionary = dictionary;
	}

	/// <summary>
	/// Gets the number of elements in the collection.
	/// </summary>
	/// <value>The count.</value>
	public int Count => m_Dictionary.Count;

	/// <summary>
	/// Returns a flattened view of the MultiValueDictionary.
	/// </summary>
	/// <value>This object as a IReadOnlyCollectionas of key-value pairs.</value>
	public MultiValueDictionary<TKey, TValue>.FlattenedMultiValueDictionary Flatten => m_Dictionary.Flatten;

	/// <summary>
	/// Gets an enumerable collection that contains the keys in the read-only dictionary.
	/// </summary>
	/// <value>The keys.</value>
	public IEnumerable<TKey> Keys => m_Dictionary.Keys;

	/// <summary>
	/// Gets an enumerable collection that contains the values in the read-only dictionary.
	/// </summary>
	/// <value>The values.</value>
	public IEnumerable<ReadOnlyCollection<TValue>> Values => m_Dictionary.Values;

	/// <summary>
	/// Gets the <see cref="ReadOnlyCollection{TValue}"/> with the specified key.
	/// </summary>
	/// <param name="key">The key.</param>
	/// <returns>ReadOnlyCollection&lt;TValue&gt;.</returns>
	public ReadOnlyCollection<TValue> this[TKey key] => m_Dictionary[key];

	/// <summary>
	/// Determines whether the dictionary contains the key/value pair.
	/// </summary>
	/// <param name="item">The object to locate in the dictionary.</param>
	/// <returns>true if <paramref name="item" /> is found in the dictionary; otherwise, false.</returns>
	public bool Contains(KeyValuePair<TKey, TValue> item) => m_Dictionary.Contains(item);

	/// <summary>
	/// Determines whether this instance contains the key/value pair.
	/// </summary>
	/// <param name="key">The key.</param>
	/// <param name="value">The value.</param>
	/// <returns><c>true</c> if the dictionary contains the specified key and value; otherwise, <c>false</c>.</returns>
	public bool Contains(TKey key, TValue value) => m_Dictionary.Contains(key, value);

	/// <summary>
	/// Determines whether the read-only dictionary contains an element that has the specified key.
	/// </summary>
	/// <param name="key">The key to locate.</param>
	/// <returns>true if the read-only dictionary contains an element that has the specified key; otherwise, false.</returns>
	public bool ContainsKey(TKey key) => m_Dictionary.ContainsKey(key);

	/// <summary>
	/// Returns an enumerator that iterates through the collection.
	/// </summary>
	/// <returns>An enumerator that can be used to iterate through the collection.</returns>
	public IEnumerator<KeyValuePair<TKey, ReadOnlyCollection<TValue>>> GetEnumerator() => m_Dictionary.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	/// <summary>
	/// Gets the value that is associated with the specified key.
	/// </summary>
	/// <param name="key">The key to locate.</param>
	/// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value" /> parameter. This parameter is passed uninitialized.</param>
	/// <returns>true if the object that implements the <see cref="IReadOnlyDictionary{TKey, TValue}" /> interface contains an element that has the specified key; otherwise, false.</returns>
	public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out ReadOnlyCollection<TValue> value) => m_Dictionary.TryGetValue(key, out value);
}
