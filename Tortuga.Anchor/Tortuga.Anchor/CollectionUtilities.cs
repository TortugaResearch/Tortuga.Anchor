using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Tortuga.Anchor
{
    /// <summary>
    /// Utility methods for collection classes.
    /// </summary>
    public static class CollectionUtilities
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
        /// Returns the enumeration as an IList. If it isn't already an IList, it makes it into one so that you can safely enumeration the list multiple times.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source. If the source is null, the result will be null.</param>
        /// <returns>Returns an IList.</returns>
        /// <remarks>This is primarily meant to be used with poorly designed interfaces that return lists disguised as IEnumerable.</remarks>
        [return: NotNullIfNotNull("source")]
        public static IList<T>? AsList<T>(this IEnumerable<T> source)
        {
            if (source == null)
                return null;
            if (source is IList<T> lists)
                return lists;
            return source.ToList();
        }

        /// <summary>
        /// Casts an IList&lt;T&gt; into a IReadOnlyList&lt;T&gt;. If the cast fails, the list is wrapped in an adapter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list.</param>
        /// <returns>IReadOnlyList&lt;T&gt;.</returns>
        /// <remarks>This is meant to be used for legacy codebases that predate IReadOnlyCollection&lt;T&gt;.</remarks>
        public static IReadOnlyCollection<T> AsReadOnlyCollection<T>(this ICollection<T> list)
        {
            if (list is IReadOnlyCollection<T> result)
                return result;

            return new SimpleReadOnlyCollection<T>(list);
        }

        /// <summary>
        /// Casts an IList&lt;T&gt; into a IReadOnlyList&lt;T&gt;. If the cast fails, the list is wrapped in a ReadOnlyCollection&lt;T&gt;.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list.</param>
        /// <returns>IReadOnlyList&lt;T&gt;.</returns>
        /// <remarks>This is meant to be used for legacy codebases that predate IReadOnlyList&lt;T&gt;.</remarks>
        public static IReadOnlyList<T> AsReadOnlyList<T>(this IList<T> list)
        {
            if (list is IReadOnlyList<T> result)
                return result;

            return new ReadOnlyCollection<T>(list);
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
                int count = 0;
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
        /// Gets the keys as a readonly collection.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <returns></returns>
        /// <remarks>This is just a cast. It accounts for an API bug in ConcurrentDictionary.</remarks>
        public static ReadOnlyCollection<TKey> GetKeys<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary)
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
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary), $"{nameof(dictionary)} is null ");
            return (ReadOnlyCollection<TValue>)dictionary.Values;
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
        /// <remarks>This isn't as fast as a real RemoveRange, it just removes one item at a time.</remarks>
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

        class SimpleReadOnlyCollection<T> : IReadOnlyCollection<T>
        {
            readonly ICollection<T> m_List;

            public SimpleReadOnlyCollection(ICollection<T> list)
            {
                m_List = list;
            }

            public int Count
            {
                get { return m_List.Count; }
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
}
