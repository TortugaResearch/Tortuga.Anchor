using System.Collections;
using System.Collections.Immutable;
using System.Reflection;

namespace Tortuga.Anchor.Metadata;

/// <summary>
/// Class ConstructorMetadataCollection.
/// </summary>
/// <seealso cref="ICollection{PropertyMetadata}" />
public sealed class ConstructorMetadataCollection : IList<ConstructorMetadata>, IReadOnlyList<ConstructorMetadata>
{
	readonly ImmutableArray<ConstructorMetadata> m_Constructors;

	internal ConstructorMetadataCollection(IEnumerable<ConstructorInfo> constructors)
	{
		m_Constructors = ImmutableArray.CreateRange(constructors.Where(c => c.IsPublic).Select(c => new ConstructorMetadata(c)));
		DefaultConstructor = m_Constructors.SingleOrDefault(c => c.Signature.Length == 0);
		try
		{
			PreferredConstructor = m_Constructors.SingleOrDefault(c => c.IsPreferred);
		}
		catch (InvalidOperationException)
		{
			throw new InvalidOperationException("More than one constructor has the PreferredConstructorAttribute");
		}
	}

	/// <summary>
	/// Gets the number of elements contained in the <see cref="ICollection{T}" />.
	/// </summary>
	/// <value>The count.</value>
	public int Count => m_Constructors.Length;

	/// <summary>
	/// Gets a the default constructor, if it has one..
	/// </summary>
	public ConstructorMetadata? DefaultConstructor { get; }

	/// <summary>
	/// Gets a value indicating whether this instance has a default constructor.
	/// </summary>
	/// <value><c>true</c> if this instance has a default constructor; otherwise, <c>false</c>.</value>
	public bool HasDefaultConstructor => DefaultConstructor != null;

	/// <summary>
	/// Gets a value indicating whether the <see cref="ICollection{T}" /> is read-only.
	/// </summary>
	/// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
	public bool IsReadOnly => true;

	/// <summary>
	/// Gets a the preferred constructor, if it has one..
	/// </summary>
	public ConstructorMetadata? PreferredConstructor { get; }

	/// <summary>
	/// Gets the <see cref="ConstructorMetadata"/> at the specified index.
	/// </summary>
	/// <param name="index">The index.</param>
	/// <returns>ConstructorMetadata.</returns>
	public ConstructorMetadata this[int index] => m_Constructors[index];

	ConstructorMetadata IList<ConstructorMetadata>.this[int index] { get => m_Constructors[index]; set => throw new NotImplementedException(); }

	void ICollection<ConstructorMetadata>.Add(ConstructorMetadata item) => throw new NotSupportedException();

	void ICollection<ConstructorMetadata>.Clear() => throw new NotSupportedException();

	/// <summary>
	/// Determines whether the <see cref="System.Collections.Generic.ICollection{T}" /> contains a specific value.
	/// </summary>
	/// <param name="item">The object to locate in the <see cref="System.Collections.Generic.ICollection{T}" />.</param>
	/// <returns>true if <paramref name="item" /> is found in the <see cref="ICollection{T}" />; otherwise, false.</returns>
	public bool Contains(ConstructorMetadata item)
	{
		if (item == null)
			throw new ArgumentNullException(nameof(item), $"{nameof(item)} is null.");

		return m_Constructors.Contains(item);
	}

	/// <summary>
	/// Determines whether [contains] [the specified signature].
	/// </summary>
	/// <param name="signature">The signature.</param>
	/// <returns><c>true</c> if [contains] [the specified signature]; otherwise, <c>false</c>.</returns>
	public bool Contains(IReadOnlyList<Type> signature)
	{
		if (signature == null)
			throw new ArgumentNullException(nameof(signature), $"{nameof(signature)} is null");

		return Find(signature) != null;
	}

	/// <summary>
	/// Determines whether [contains] [the specified signature].
	/// </summary>
	/// <param name="signature">The signature.</param>
	/// <returns><c>true</c> if [contains] [the specified signature]; otherwise, <c>false</c>.</returns>
	public bool Contains(params Type[] signature) => Find(signature) != null;

	/// <summary>
	/// Copies the elements of the <see cref="ICollection{T}" /> to an <see cref="System.Array" />, starting at a particular <see cref="System.Array" /> index.
	/// </summary>
	/// <param name="array">The one-dimensional <see cref="Array" /> that is the destination of the elements copied from <see cref="ICollection{T}" />. The <see cref="Array" /> must have zero-based indexing.</param>
	/// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
	public void CopyTo(ConstructorMetadata[] array, int arrayIndex)
	{
		if (array == null)
			throw new ArgumentNullException(nameof(array), $"{nameof(array)} is null");

		m_Constructors.CopyTo(array, arrayIndex);
	}

	/// <summary>
	/// Attempts to find the specified constructor. Returns null if no match was found.
	/// </summary>
	/// <param name="signature">The signature.</param>
	/// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
	public ConstructorMetadata? Find(IReadOnlyList<Type> signature)
	{
		if (signature == null)
			throw new ArgumentNullException(nameof(signature), $"{nameof(signature)} is null");

		foreach (var item in m_Constructors)
		{
			if (item.Signature.Length != signature.Count)
				continue;

			var isMatch = true;
			for (var i = 0; i < item.Signature.Length; i++)
			{
				if (item.Signature[i] != signature[i])
				{
					isMatch = false;
					break;
				}
			}
			if (isMatch)
				return item;
		}
		return null;
	}

	/// <summary>
	/// Attempts to find the specified constructor. Returns null if no match was found.
	/// </summary>
	/// <param name="signature">The signature.</param>
	public ConstructorMetadata? Find(params Type[] signature)
	{
		if (signature == null)
			throw new ArgumentNullException(nameof(signature), $"{nameof(signature)} is null");

		foreach (var item in m_Constructors)
		{
			if (item.Signature.Length != signature.Length)
				continue;

			var isMatch = true;
			for (var i = 0; i < item.Signature.Length; i++)
			{
				if (item.Signature[i] != signature[i])
				{
					isMatch = false;
					break;
				}
			}
			if (isMatch)
				return item;
		}
		return null;
	}

	/// <summary>
	/// Returns an enumerator that iterates through the collection.
	/// </summary>
	/// <returns>A <see cref="System.Collections.Generic.IEnumerator{T}" /> that can be used to iterate through the collection.</returns>
	public IEnumerator<ConstructorMetadata> GetEnumerator()
	{
		return ((ICollection<ConstructorMetadata>)m_Constructors).GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return ((ICollection<ConstructorMetadata>)m_Constructors).GetEnumerator();
	}

	/// <summary>
	/// Determines the index of a specific item in the collection. />.
	/// </summary>
	/// <param name="item">The object to locate in the collection />.</param>
	/// <returns>The index of <paramref name="item" /> if found in the list; otherwise, -1.</returns>
	public int IndexOf(ConstructorMetadata item) => m_Constructors.IndexOf(item);

	void IList<ConstructorMetadata>.Insert(int index, ConstructorMetadata item) => throw new NotImplementedException();

	bool ICollection<ConstructorMetadata>.Remove(ConstructorMetadata item) => throw new NotSupportedException();

	void IList<ConstructorMetadata>.RemoveAt(int index) => throw new NotImplementedException();
}
