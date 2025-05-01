using System.Collections;

namespace Tortuga.Anchor.Collections;

/// <summary>
/// A flattened representation of a MultiValueDictionary.
/// Implements the <see cref="IReadOnlyCollection{T}" />
/// </summary>
/// <seealso cref="IReadOnlyCollection{T}" />

public class FlattenedMultiValueDictionary<TKey, TValue> : IReadOnlyCollection<KeyValuePair<TKey, TValue>>
	where TKey : notnull
{
	readonly MultiValueDictionary<TKey, TValue> m_Parent;

	internal FlattenedMultiValueDictionary(MultiValueDictionary<TKey, TValue> parent)
	{
		m_Parent = parent;
	}

	/// <summary>
	/// Gets the number of values in the dictionary.
	/// </summary>
	public int Count => m_Parent.ItemCount;

	/// <summary>
	/// Gets the values across all keys.
	/// </summary>
	public IEnumerable<TValue> Values => m_Parent.SelectMany(x => x.Value, (a, b) => b);

	/// <summary>
	/// Returns an enumerator that iterates through the collection.
	/// </summary>
	/// <returns>An enumerator that can be used to iterate through the collection.</returns>
	public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
	{
		foreach (var item in m_Parent.InternalDictionary)
			foreach (var value in item.Value)
				yield return new KeyValuePair<TKey, TValue>(item.Key, value);
	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
