using System.Collections;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Reflection;
using Tortuga.Anchor.Collections;

namespace Tortuga.Anchor.Metadata;

/// <summary>
/// Class MethodMetadataCollection.
/// Implements the <see cref="IList{MethodMetadata}" />
/// Implements the <see cref="IReadOnlyList{MethodMetadata}" />
/// </summary>
/// <seealso cref="IList{MethodMetadata}" />
/// <seealso cref="IReadOnlyList{MethodMetadata}" />
public class MethodMetadataCollection : IList<MethodMetadata>, IReadOnlyList<MethodMetadata>
{
	readonly MultiValueDictionary<string, MethodMetadata> m_Methods = new(StringComparer.OrdinalIgnoreCase);
	readonly ImmutableArray<MethodMetadata> m_QuickList;

	internal MethodMetadataCollection(IEnumerable<MethodInfo> methods)
	{
		foreach (var item in methods.Where(x => !x.IsSpecialName))
			m_Methods.Add(item.Name, new MethodMetadata(item));

		m_QuickList = [.. m_Methods.Flatten.Values];
		PublicMethods = [.. m_QuickList.Where(x => x.IsPublic)];
		ProtectedMethods = [.. m_QuickList.Where(x => x.IsProtected)];
	}

	/// <summary>
	/// Gets the number of elements contained in the <see cref="ICollection{T}" />.
	/// </summary>
	/// <value>The count.</value>
	public int Count => m_QuickList.Length;

#pragma warning disable CA1033 // Interface methods should be callable by child types
	bool ICollection<MethodMetadata>.IsReadOnly => true;
#pragma warning restore CA1033 // Interface methods should be callable by child types

	MethodMetadata IList<MethodMetadata>.this[int index] { get => m_QuickList[index]; set => throw new NotSupportedException(); }

	/// <summary>
	/// Gets the <see cref="MethodMetadata"/> at the specified index.
	/// </summary>
	/// <param name="index">The index.</param>
	/// <returns>MethodMetadata.</returns>
	public MethodMetadata this[int index] => m_QuickList[index];

	/// <summary>
	/// Gets the <see cref="ReadOnlyCollection{MethodMetadata}"/> with the specified method name.
	/// </summary>
	/// <param name="methodName">Name of the method.</param>
	public ReadOnlyCollection<MethodMetadata> this[string methodName] => m_Methods[methodName];

	void ICollection<MethodMetadata>.Add(MethodMetadata item) => throw new NotSupportedException();

	void ICollection<MethodMetadata>.Clear() => throw new NotSupportedException();

	/// <summary>
	/// Determines whether the <see cref="ICollection{T}" /> contains a specific value.
	/// </summary>
	/// <param name="item">The object to locate in the <see cref="ICollection{T}" />.</param>
	/// <returns>true if <paramref name="item" /> is found in the <see cref="ICollection{T}" />; otherwise, false.</returns>
	public bool Contains(MethodMetadata item) => m_QuickList.Contains(item);

	/// <summary>
	/// Copies the elements of the <see cref="ICollection{T}" /> to an <see cref="Array" />, starting at a particular <see cref="Array" /> index.
	/// </summary>
	/// <param name="array">The one-dimensional <see cref="Array" /> that is the destination of the elements copied from <see cref="ICollection{T}" />. The <see cref="Array" /> must have zero-based indexing.</param>
	/// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
	public void CopyTo(MethodMetadata[] array, int arrayIndex) => m_QuickList.CopyTo(array, arrayIndex);

	/// <summary>
	/// Returns an enumerator that iterates through the collection.
	/// </summary>
	/// <returns>An enumerator that can be used to iterate through the collection.</returns>
	public IEnumerator<MethodMetadata> GetEnumerator() => ((IEnumerable<MethodMetadata>)m_QuickList).GetEnumerator();

	/// <summary>
	/// Returns an enumerator that iterates through a collection.
	/// </summary>
	/// <returns>An <see cref="IEnumerator" /> object that can be used to iterate through the collection.</returns>
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	/// <summary>
	/// Determines the index of a specific item in the <see cref="IList{T}" />.
	/// </summary>
	/// <param name="item">The object to locate in the <see cref="IList{T}" />.</param>
	/// <returns>The index of <paramref name="item" /> if found in the list; otherwise, -1.</returns>
	public int IndexOf(MethodMetadata item) => m_QuickList.IndexOf(item);

	void IList<MethodMetadata>.Insert(int index, MethodMetadata item) => throw new NotSupportedException();

	bool ICollection<MethodMetadata>.Remove(MethodMetadata item) => throw new NotImplementedException();

	void IList<MethodMetadata>.RemoveAt(int index) => throw new NotSupportedException();

	/// <summary>
	/// Gets the public methods.
	/// </summary>
	/// <value>The public methods.</value>
	public ImmutableArray<MethodMetadata> PublicMethods { get; }

	/// <summary>
	/// Gets the protected methods.
	/// </summary>
	/// <value>The protected methods.</value>
	public ImmutableArray<MethodMetadata> ProtectedMethods { get; }
}
