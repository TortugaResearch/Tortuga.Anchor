using System.Collections;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using Tortuga.Anchor.Collections;

namespace Tortuga.Anchor;

/// <summary>
/// Utility methods for collection classes.
/// </summary>
public static partial class CollectionUtilities
{
	/// <summary>
	/// Adds a list of values into the target collection.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="target"></param>
	/// <param name="list"></param>
	public static void AddRange<T>(this ICollection<T> target, List<T> list)
	{
		if (target == null)
			throw new ArgumentNullException(nameof(target), $"{nameof(target)} is null.");
		if (target.IsReadOnly)
			throw new ArgumentException($"{nameof(target)}.IsReadOnly must be false", nameof(target));
		if (list == null)
			throw new ArgumentNullException(nameof(list), $"{nameof(list)} is null.");

		foreach (var item in list)
			target.Add(item);
	}

	/// <summary>
	/// Adds a list of values into the target collection.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="target"></param>
	/// <param name="list"></param>
	public static void AddRange<T>(this ICollection<T> target, IEnumerable<T> list)
	{
		if (target == null)
			throw new ArgumentNullException(nameof(target), $"{nameof(target)} is null.");
		if (target.IsReadOnly)
			throw new ArgumentException($"{nameof(target)}.IsReadOnly must be false", nameof(target));
		if (list == null)
			throw new ArgumentNullException(nameof(list), $"{nameof(list)} is null.");

		if (list is List<T> typedList)
			foreach (var item in typedList) //switch to fast path
				target.Add(item);
		else
			foreach (var item in list) //Using IEnumerable<T> is slower that List<T>
				target.Add(item);
	}

	/// <summary>
	/// Adds a list of values into the target collection.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="target"></param>
	/// <param name="list"></param>
	public static void AddRange<T>(this ICollection<T> target, params T[] list)
	{
		if (target == null)
			throw new ArgumentNullException(nameof(target), $"{nameof(target)} is null.");
		if (target.IsReadOnly)
			throw new ArgumentException($"{nameof(target)}.IsReadOnly must be false", nameof(target));
		if (list == null)
			throw new ArgumentNullException(nameof(list), $"{nameof(list)} is null.");

		foreach (var item in list)
			target.Add(item);
	}

	/// <summary>
	/// Unwraps a list of Tasks and adds their results to the target collection.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="target">The target collection to be added to.</param>
	/// <param name="list">The list.</param>
	public static async Task AddRange<T>(this ICollection<T> target, IEnumerable<Task<T>> list)
	{
		if (target == null)
			throw new ArgumentNullException(nameof(target), $"{nameof(target)} is null.");
		if (list == null)
			throw new ArgumentNullException(nameof(list), $"{nameof(list)} is null.");

		foreach (var item in list)
			target.Add(await item.ConfigureAwait(false));
	}

	/// <summary>
	/// Returns the enumeration as an IList. If it isn't already an IList, it makes it into one
	/// so that you can safely enumeration the list multiple times.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="source">The source. If the source is null, the result will be null.</param>
	/// <returns>Returns an IList.</returns>
	/// <remarks>
	/// This is primarily meant to be used with poorly designed interfaces that return lists
	/// disguised as IEnumerable.
	/// </remarks>
	[return: NotNullIfNotNull("source")]
	public static IList<T>? AsList<T>(this IEnumerable<T>? source)
	{
		if (source == null)
			return null;
		if (source is IList<T> lists)
			return lists;
		return source.ToList();
	}

	/// <summary>
	/// Casts an IList&lt;T&gt; into a AsReadOnlyCollection&lt;T&gt;. If the cast fails, the list is
	/// wrapped in an adapter.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="source">The source. If the source is null, the result will be null.</param>
	/// <returns>IReadOnlyList&lt;T&gt;.</returns>
	/// <remarks>This is meant to be used for legacy codebases that predate IReadOnlyCollection&lt;T&gt;.</remarks>
	[return: NotNullIfNotNull("source")]
	public static IReadOnlyCollection<T>? AsReadOnlyCollection<T>(this IEnumerable<T>? source)
	{
		if (source == null)
			return null;

		if (source is IReadOnlyCollection<T> result)
			return result;

		return new SimpleReadOnlyCollection<T>(source);
	}

	/// <summary>
	/// Casts an IList&lt;T&gt; into a IReadOnlyList&lt;T&gt;. If the cast fails, the list is
	/// wrapped in a ReadOnlyCollection&lt;T&gt;.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="source">The source. If the source is null, the result will be null.</param>
	/// <returns>IReadOnlyList&lt;T&gt;.</returns>
	/// <remarks>This is meant to be used for legacy codebases that predate IReadOnlyList&lt;T&gt;.</remarks>
	[return: NotNullIfNotNull("source")]
	public static IReadOnlyList<T>? AsReadOnlyList<T>(this IEnumerable<T>? source)
	{
		if (source == null)
			return null;

		if (source is IReadOnlyList<T> rList)
			return rList;

		if (source is IList<T> list)
			return new ReadOnlyCollection<T>(list);

		return new ReadOnlyCollection<T>(source.ToList());
	}

	/// <summary>
	/// Batches the specified enumeration into lists according to the indicated batch size.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="source">The source.</param>
	/// <param name="batchSize">Size of the batch.</param>
	/// <returns>IEnumerable&lt;List&lt;T&gt;&gt;.</returns>
	/// <exception cref="ArgumentNullException">source</exception>
	/// <exception cref="ArgumentOutOfRangeException">batchSize</exception>
	public static IEnumerable<List<T>> BatchAsLists<T>(this IEnumerable<T> source, int batchSize)
	{
		if (source == null)
			throw new ArgumentNullException(nameof(source), $"{nameof(source)} is null.");
		if (batchSize <= 0)
			throw new ArgumentOutOfRangeException(nameof(batchSize), batchSize, $"{batchSize} must be greater than 0");

		return BatchAsListsCore();

		IEnumerable<List<T>> BatchAsListsCore()
		{
			int count;
			using (var iter = source.GetEnumerator())
			{
				while (iter.MoveNext())
				{
					var chunk = new List<T>(batchSize);
					count = 1;
					chunk.Add(iter.Current);
					for (int i = 1; i < batchSize && iter.MoveNext(); i++)
					{
						chunk.Add(iter.Current);
						count++;
					}
					yield return chunk;
				}
			}
		}
	}

	//    if (source is IList<T> list)
	//        return new ReadOnlyCollection<T>(list);
	//    else
	//        return new ReadOnlyCollection<T>(source.ToList());
	//}
	/// <summary>
	/// Batches the specified enumeration into lightweight segments according to the indicated batch size.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="source">The source.</param>
	/// <param name="batchSize">Size of the batch.</param>
	/// <returns>IEnumerable&lt;List&lt;T&gt;&gt;.</returns>
	/// <exception cref="ArgumentNullException">source</exception>
	/// <exception cref="ArgumentOutOfRangeException">batchSize</exception>
	public static IEnumerable<ReadOnlyListSegment<T>> BatchAsSegments<T>(this IReadOnlyList<T> source, int batchSize)
	{
		if (source == null)
			throw new ArgumentNullException(nameof(source), $"{nameof(source)} is null.");
		if (batchSize <= 0)
			throw new ArgumentOutOfRangeException(nameof(batchSize), batchSize, $"{batchSize} must be greater than 0");

		return BatchAsListsCore();

		IEnumerable<ReadOnlyListSegment<T>> BatchAsListsCore()
		{
			var currentStart = 0;
			while (currentStart < source.Count)
			{
				var nextSize = Math.Min(batchSize, source.Count - currentStart);
				yield return new ReadOnlyListSegment<T>(source, currentStart, nextSize);
				currentStart += nextSize;
			}
		}
	}

	///// <summary>
	///// Creates a read-only list from an System.Collections.Generic.IEnumerable`1.
	///// </summary>
	///// <typeparam name="T"></typeparam>
	///// <param name="source">The source.</param>
	///// <returns>IReadOnlyList&lt;T&gt;.</returns>
	///// <exception cref="ArgumentNullException">source</exception>
	///// <remarks>This is meant to be used for legacy codebases that predate IReadOnlyList&lt;T&gt;.</remarks>
	//public static IReadOnlyList<T> ToReadOnlyList<T>(this IEnumerable<T> source)
	//{
	//    if (source == null)
	//        throw new ArgumentNullException(nameof(source), $"{nameof(source)} is null.");
	/// <summary>
	/// Concatenates an item onto the emd of an enumeration.
	/// </summary>
	/// <typeparam name="TSource">The type of enumerable</typeparam>
	/// <param name="list">The source to be enumerated.</param>
	/// <param name="item">The item to be appended to the enumeration.</param>
	/// <returns>
	/// An System.Collections.Generic.IEnumerable&lt;T&gt; that contains the concatenated
	/// elements of the two input sequences.
	/// </returns>
	public static IEnumerable<TSource> Concat<TSource>(this IEnumerable<TSource> list, TSource item)
	{
		if (list == null)
			throw new ArgumentNullException(nameof(list), $"{nameof(list)} is null.");

		return ConcatCore();

		IEnumerable<TSource> ConcatCore()
		{
			foreach (var element in list)
				yield return element;
			yield return item;
		}
	}

	/// <summary>
	/// Determines whether the source contains all of the specified values.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="source">The source to be checked.</param>
	/// <param name="values">The values.</param>
	/// <returns><c>true</c> if the source contains all of the specified values; otherwise, <c>false</c>.</returns>
	/// <remarks>Duplicates in either collection will be ignored.</remarks>
	public static bool ContainsAllOf<T>(this IEnumerable<T> source, params T[] values)
	{
		if (source == null)
			throw new ArgumentNullException(nameof(source), $"{nameof(source)} is null.");

		if (values == null)
			throw new ArgumentNullException(nameof(values), $"{nameof(values)} is null.");

		var list = source.AsList();

		foreach (var item in values)
			if (!list.Contains(item))
				return false;

		return true;
	}

	/// <summary>
	/// Determines whether the source contains all of the specified values and nothing else.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="source">The source to be checked.</param>
	/// <param name="values">The values.</param>
	/// <returns><c>true</c> if the source contains all of the specified values and nothing else; otherwise, <c>false</c>.</returns>
	/// <remarks>Duplicates in either collection will be ignored.</remarks>
	public static bool ContainsOnly<T>(this IEnumerable<T> source, params T[] values)
	{
		if (source == null)
			throw new ArgumentNullException(nameof(source), $"{nameof(source)} is null.");

		if (values == null)
			throw new ArgumentNullException(nameof(values), $"{nameof(values)} is null.");

		var list = source.AsList();

		foreach (var item in values)
			if (!list.Contains(item))
				return false;

		//We can't just compare counts because there may be duplicates
		foreach (var item in source)
			if (!values.Contains(item))
				return false;

		return true;
	}

	/// <summary>
	/// Gets the keys as a readonly collection.
	/// </summary>
	/// <typeparam name="TKey">The type of the key.</typeparam>
	/// <typeparam name="TValue">The type of the value.</typeparam>
	/// <param name="dictionary">The dictionary.</param>
	/// <returns></returns>
	/// <remarks>This is just a cast. It accounts for an API bug in ConcurrentDictionary.</remarks>
	public static ReadOnlyCollection<TKey> GetKeys<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary)
		where TKey : notnull
	{
		if (dictionary == null)
			throw new ArgumentNullException(nameof(dictionary), $"{nameof(dictionary)} is null ");
		return (ReadOnlyCollection<TKey>)dictionary.Keys;
	}

	/// <summary>
	/// Gets the values as a readonly collection.
	/// </summary>
	/// <typeparam name="TKey">The type of the key.</typeparam>
	/// <typeparam name="TValue">The type of the value.</typeparam>
	/// <param name="dictionary">The dictionary.</param>
	/// <returns></returns>
	/// <remarks>This is just a cast. It accounts for an API bug in ConcurrentDictionary.</remarks>
	public static ReadOnlyCollection<TValue> GetValues<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary)
		where TKey : notnull
	{
		if (dictionary == null)
			throw new ArgumentNullException(nameof(dictionary), $"{nameof(dictionary)} is null ");
		return (ReadOnlyCollection<TValue>)dictionary.Values;
	}

	/// <summary>
	/// Locates the index of the indicated item.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="list">The list.</param>
	/// <param name="item">The item.</param>
	/// <returns>System.Int32.</returns>
	/// <exception cref="ArgumentNullException">list</exception>
	public static int IndexOf<T>(this IReadOnlyList<T> list, T item)
	{
		if (list == null)
			throw new ArgumentNullException(nameof(list), $"{nameof(list)} is null ");

		for (var i = 0; i < list.Count; i++)
		{
			var value = list[i];
			if (value == null)
			{
				if (item == null)
					return i;
			}
			else if (value.Equals(item))
				return i;
		}
		return -1;
	}

	/// <summary>
	/// Inserts a list of values into the target collection.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="target"></param>
	/// <param name="startingIndex"></param>
	/// <param name="list"></param>
	/// <remarks>This isn't as fast as a real InsertRange, it just adds one item at a time.</remarks>
	public static void InsertRange<T>(this IList<T> target, int startingIndex, IEnumerable<T> list)
	{
		if (target == null)
			throw new ArgumentNullException(nameof(target), $"{nameof(target)} is null.");
		if (target.IsReadOnly)
			throw new ArgumentException($"{nameof(target)}.IsReadOnly must be false", nameof(target));
		if (list == null)
			throw new ArgumentNullException(nameof(list), $"{nameof(list)} is null.");

		if (startingIndex < 0)
			throw new ArgumentOutOfRangeException(nameof(startingIndex), startingIndex, $"{nameof(startingIndex)} must be >= 0");
		if (startingIndex > target.Count)
			throw new ArgumentOutOfRangeException(nameof(startingIndex), startingIndex, $"{nameof(startingIndex)} must be <= {nameof(target)}.Count");

		var index = startingIndex;
		foreach (var item in list)
		{
			target.Insert(index, item);
			index = Math.Min(index + 1, target.Count); //this is needed in case the collection is filtering out duplicates
		}
	}

	/// <summary>
	/// Removes count items from the collection
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="list"></param>
	/// <param name="startingIndex"></param>
	/// <param name="count"></param>
	/// <remarks>
	/// This isn't as fast as a real RemoveRange, it just removes one item at a time.
	/// </remarks>
	public static void RemoveRange<T>(this IList<T> list, int startingIndex, int count)
	{
		if (list == null)
			throw new ArgumentNullException(nameof(list), $"{nameof(list)} is null.");
		if (list.IsReadOnly)
			throw new ArgumentException("list.IsReadOnly must be false", nameof(list));
		if (startingIndex < 0)
			throw new ArgumentOutOfRangeException(nameof(startingIndex), startingIndex, "startingIndex must be >= 0");
		if (startingIndex >= list.Count)
			throw new ArgumentOutOfRangeException(nameof(startingIndex), startingIndex, "startingIndex must be < list.Count");

		for (int i = 0; i < count; i++)
			list.RemoveAt(startingIndex);
	}

	/// <summary>
	/// Performs an in-place sort of the specified list using it's IComparable&lt;T&gt; interface.
	/// </summary>
	/// <typeparam name="T">Type of item in the list</typeparam>
	/// <param name="list">The list to sort.</param>
	/// <exception cref="ArgumentNullException">list or comparer</exception>
	public static void Sort<T>(this IList<T> list) where T : IComparable<T>
	{
		if (list == null)
			throw new ArgumentNullException(nameof(list), $"{nameof(list)} is null.");

		if (list.Count == 0)
			return;

		//This will pull the compare method from IComparable<T>
		IntrospectiveSort(list, 0, list.Count, Comparer<T>.Default.Compare);
	}

	/// <summary>
	/// Performs an in-place sort of the specified list.
	/// </summary>
	/// <typeparam name="T">Type of item in the list</typeparam>
	/// <param name="list">The list to sort.</param>
	/// <param name="comparer">The comparer to use when sorting.</param>
	/// <exception cref="ArgumentNullException">list or comparer</exception>
	public static void Sort<T>(this IList<T> list, IComparer<T> comparer)
	{
		if (list == null)
			throw new ArgumentNullException(nameof(list), $"{nameof(list)} is null.");
		if (comparer == null)
			throw new ArgumentNullException(nameof(comparer), $"{nameof(comparer)} is null.");

		if (list.Count == 0)
			return;

		IntrospectiveSort(list, 0, list.Count, comparer.Compare);
	}

	/// <summary>
	/// Performs an in-place sort of the specified list.
	/// </summary>
	/// <typeparam name="T">Type of item in the list</typeparam>
	/// <param name="list">The list to sort.</param>
	/// <param name="startIndex">The start index.</param>
	/// <param name="count">The count.</param>
	/// <param name="comparer">The comparer to use when sorting.</param>
	/// <exception cref="ArgumentNullException">list or comparer</exception>
	public static void Sort<T>(this IList<T> list, int startIndex, int count, IComparer<T> comparer)
	{
		if (list == null)
			throw new ArgumentNullException(nameof(list), $"{nameof(list)} is null.");
		if (comparer == null)
			throw new ArgumentNullException(nameof(comparer), $"{nameof(comparer)} is null.");
		if (startIndex >= list.Count)
			throw new ArgumentOutOfRangeException(nameof(startIndex), startIndex, $"{nameof(startIndex)} must be less than {nameof(list.Count)}");
		if (startIndex < 0)
			throw new ArgumentOutOfRangeException(nameof(startIndex), startIndex, $"{nameof(startIndex)} must be greater than 0");
		if (count <= 0)
			throw new ArgumentOutOfRangeException(nameof(count), count, $"{nameof(count)} must be greater than 0");

		if (list.Count == 0)
			return;

		IntrospectiveSort(list, startIndex, count, comparer.Compare);
	}

	/// <summary>
	/// Performs an in-place sort of the specified list.
	/// </summary>
	/// <typeparam name="T">Type of item in the list</typeparam>
	/// <param name="list">The list to sort.</param>
	/// <param name="comparison">The comparison function to use when sorting.</param>
	/// <exception cref="ArgumentNullException">list or comparer</exception>
	public static void Sort<T>(this IList<T> list, Comparison<T> comparison)
	{
		if (list == null)
			throw new ArgumentNullException(nameof(list), $"{nameof(list)} is null.");
		if (comparison == null)
			throw new ArgumentNullException(nameof(comparison), $"{nameof(comparison)} is null.");

		if (list.Count == 0)
			return;

		IntrospectiveSort(list, 0, list.Count, comparison);
	}

	///// <summary>
	///// Copies an enumerator into a list, disposing it afterwards.
	///// </summary>
	///// <typeparam name="T"></typeparam>
	///// <param name="source">The source.</param>
	///// <remarks>The source will be disposed is applicable..</remarks>
	//public static IList<T> ToList<T>(this IEnumerator<T> source)
	//{
	//	if (source == null)
	//		throw new ArgumentNullException(nameof(source), $"{nameof(source)} is null.");

	//	var result = new List<T>();
	//	while (source.MoveNext())
	//		result.Add(source.Current);
	//	source.Dispose();
	//	return result;
	//}

	class SimpleReadOnlyCollection<T> : IReadOnlyCollection<T>
	{
		readonly IEnumerable<T> m_List;

		public SimpleReadOnlyCollection(IEnumerable<T> list)
		{
			m_List = list;
		}

		public int Count
		{
			get { return m_List.Count(); }
		}

		public IEnumerator<T> GetEnumerator()
		{
			return m_List.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return m_List.GetEnumerator();
		}
	}
}
