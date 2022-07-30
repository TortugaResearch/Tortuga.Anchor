using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace Tortuga.Anchor.Collections;

/// <summary>
/// This collection allows muiltuiple values to be associated with eack key.
/// Implements the <see cref="IReadOnlyDictionary{TKey, TValue}" />
/// </summary>
/// <typeparam name="TKey">The type of the t key.</typeparam>
/// <typeparam name="TValue">The type of the t value.</typeparam>
/// <seealso cref="IReadOnlyDictionary{TKey, TValue}" />
public class MultiValueDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, ReadOnlyCollection<TValue>>
	where TKey : notnull
{
	readonly Dictionary<TKey, ListWithWrapper<TValue>> m_Dictionary;

	/// <summary>
	/// The count of indiviaul values.
	/// </summary>
	int m_Count;

	IReadOnlyCollection<KeyValuePair<TKey, TValue>>? m_FlatWrapper;

	/// <summary>
	/// Initializes a new instance of the <see cref="MultiValueDictionary{TKey, TValue}"/> class.
	/// </summary>
	public MultiValueDictionary()
	{
		m_Dictionary = new();
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="MultiValueDictionary{TKey, TValue}"/> class.
	/// </summary>
	/// <param name="capacity">The initial capacity.</param>
	public MultiValueDictionary(int capacity)
	{
		m_Dictionary = new(capacity);
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="MultiValueDictionary{TKey, TValue}"/> class.
	/// </summary>
	/// <param name="comparer">The comparer to use when comparing keys.</param>
	public MultiValueDictionary(IEqualityComparer<TKey> comparer)
	{
		m_Dictionary = new(comparer);
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="MultiValueDictionary{TKey, TValue}"/> class.
	/// </summary>
	/// <param name="capacity">The initial capacity.</param>
	/// <param name="comparer">The comparer to use when comparing keys.</param>
	public MultiValueDictionary(int capacity, IEqualityComparer<TKey> comparer)
	{
		m_Dictionary = new(capacity, comparer);
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
	public IReadOnlyCollection<KeyValuePair<TKey, TValue>> Flatten
	{
		get
		{
			if (m_FlatWrapper == null)
				m_FlatWrapper = new FlatWrapper(this);

			return m_FlatWrapper;
		}
	}

	/// <summary>
	/// Gets an enumerable collection that contains the keys in the read-only dictionary.
	/// </summary>
	/// <value>The keys.</value>
	public IEnumerable<TKey> Keys => m_Dictionary.Keys;

	/// <summary>
	/// Gets an enumerable collection that contains the values in the read-only dictionary.
	/// </summary>
	/// <value>The values.</value>
	public IEnumerable<ReadOnlyCollection<TValue>> Values => m_Dictionary.Values.Select(x => x.ReadOnlyWrapper);

	/// <summary>
	/// Gets the <see cref="ReadOnlyCollection{TValue}"/> with the specified key.
	/// </summary>
	/// <param name="key">The key.</param>
	/// <returns>ReadOnlyCollection&lt;TValue&gt;.</returns>
	public ReadOnlyCollection<TValue> this[TKey key] => m_Dictionary[key].ReadOnlyWrapper;

	/// <summary>
	/// Adds the value to the specified key.
	/// </summary>
	/// <param name="key">The key.</param>
	/// <param name="value">The value.</param>
	public void Add(TKey key, TValue value)
	{
		if (key == null)
			throw new ArgumentNullException(nameof(key), $"{nameof(key)} is null.");

		if (!m_Dictionary.TryGetValue(key, out var list))
		{
			list = new();
			m_Dictionary[key] = list;
		}
		list.Add(value);
		m_Count += 1;
	}

	/// <summary>
	/// Adds an item to the dictionary. />.
	/// </summary>
	/// <param name="item">The object to add to the dictionary.</param>
	public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);

	/// <summary>
	/// Adds the range of values to the specified key.
	/// </summary>
	/// <param name="key">The key.</param>
	/// <param name="values">The values.</param>
	/// <exception cref="ArgumentNullException">key</exception>
	/// <exception cref="ArgumentNullException">values</exception>
	public void AddRange(TKey key, IEnumerable<TValue> values)
	{
		if (key == null)
			throw new ArgumentNullException(nameof(key), $"{nameof(key)} is null.");

		if (values == null)
			throw new ArgumentNullException(nameof(values), $"{nameof(values)} is null.");

		if (!m_Dictionary.TryGetValue(key, out var list))
		{
			list = new();
			m_Dictionary[key] = list;
		}
		var oldCount = list.Count;
		list.AddRange(values);
		m_Count += list.Count - oldCount;
	}

	/// <summary>
	/// Adds the range of values to the specified key.
	/// </summary>
	/// <param name="key">The key.</param>
	/// <param name="values">The values.</param>
	/// <exception cref="ArgumentNullException">key</exception>
	/// <exception cref="ArgumentNullException">values</exception>
	public void AddRange(TKey key, params TValue[] values)
	{
		AddRange(key, (IEnumerable<TValue>)values);
	}

	/// <summary>
	/// Removes all items from the dictionary.
	/// </summary>
	/// <exception cref="System.NotImplementedException"></exception>
	public void Clear()
	{
		m_Dictionary.Clear();
		m_Count = 0;
	}

	/// <summary>
	/// Determines whether the dictionary contains the key/value pair.
	/// </summary>
	/// <param name="item">The object to locate in the dictionary.</param>
	/// <returns>true if <paramref name="item" /> is found in the dictionary; otherwise, false.</returns>
	public bool Contains(KeyValuePair<TKey, TValue> item) => Contains(item.Key, item.Value);

	/// <summary>
	/// Determines whether this instance contains the key/value pair.
	/// </summary>
	/// <param name="key">The key.</param>
	/// <param name="value">The value.</param>
	/// <returns><c>true</c> if the dictionary contains the specified key and value; otherwise, <c>false</c>.</returns>
	public bool Contains(TKey key, TValue value)
	{
		if (!TryGetValue(key, out var temp))
			return false;
		return temp.Contains(value);
	}

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
	public IEnumerator<KeyValuePair<TKey, ReadOnlyCollection<TValue>>> GetEnumerator()
	{
		foreach (var item in m_Dictionary)
			yield return new(item.Key, item.Value.ReadOnlyWrapper);
	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	/// <summary>
	/// Removes a value from the specified key.
	/// </summary>
	/// <param name="key">The key.</param>
	/// <param name="value">The value.</param>
	/// <returns><c>true</c> if the value was found and removed, <c>false</c> otherwise.</returns>
	public bool Remove(TKey key, TValue value)
	{
		if (!m_Dictionary.TryGetValue(key, out var temp))
			return false;
		var result = temp.Remove(value);
		if (temp.Count == 0)
			m_Dictionary.Remove(key);
		if (result)
			m_Count -= 1;
		return result;
	}

	/// <summary>
	/// Removes the first occurrence of a specific object from the dictionary.
	/// </summary>
	/// <param name="item">The object to remove from the dictionary.</param>
	/// <returns>true if <paramref name="item" /> was successfully removed from the dictionary; otherwise, false. This method also returns false if <paramref name="item" /> is not found in the original dictionary.</returns>
	public bool Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key, item.Value);

	/// <summary>
	/// Gets the value that is associated with the specified key.
	/// </summary>
	/// <param name="key">The key to locate.</param>
	/// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value" /> parameter. This parameter is passed uninitialized.</param>
	/// <returns>true if the object that implements the <see cref="IReadOnlyDictionary{TKey, TValue}" /> interface contains an element that has the specified key; otherwise, false.</returns>
	public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out ReadOnlyCollection<TValue> value)

	{
		var result = m_Dictionary.TryGetValue(key, out var temp);
		value = temp?.ReadOnlyWrapper!; //null is allowed here.
		return result;
	}

	class FlatWrapper : IReadOnlyCollection<KeyValuePair<TKey, TValue>>
	{
		readonly MultiValueDictionary<TKey, TValue> m_Parent;

		public FlatWrapper(MultiValueDictionary<TKey, TValue> parent)
		{
			m_Parent = parent;
		}

		public int Count => m_Parent.m_Count;

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			foreach (var item in m_Parent.m_Dictionary)
				foreach (var value in item.Value)
					yield return new KeyValuePair<TKey, TValue>(item.Key, value);
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

	class ListWithWrapper<T> : List<T>
	{
		ReadOnlyCollection<T>? m_ReadOnlyWrapper;

		public ReadOnlyCollection<T> ReadOnlyWrapper
		{
			get
			{
				if (m_ReadOnlyWrapper == null)
					m_ReadOnlyWrapper = new ReadOnlyCollection<T>(this);

				return m_ReadOnlyWrapper;
			}
		}
	}
}
