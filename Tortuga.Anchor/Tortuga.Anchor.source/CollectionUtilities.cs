using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

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

            var typedList = list as List<T>;
            if (typedList != null)
                AddRange(target, typedList); //switch to fast path

            foreach (var item in list)
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
        /// Returns the enumeration as an IList. If it isn't already an IList, it makes it into one so that you can safely enumeration the list multiple times. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source. If the source is null, the result will be null.</param>
        /// <returns>Returns an IList.</returns>
        /// <remarks>This is primarily meant to be used with poorly designed interfaces that return lists disguised as IEnumerable.</remarks>
        public static IList<T> AsList<T>(this IEnumerable<T> source)
        {
            if (source == null)
                return null;
            if (source is IList<T>)
                return (IList<T>)source;
            return source.ToList();
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

            foreach (var element in list)
                yield return element;
            yield return item;
        }

        /// <summary>
        /// Inserts a list of values into the target collection. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="startingIndex"></param>
        /// <param name="list"></param>
        /// <remarks>This isn't as fast as a real InsertRange, it just adds one item at a time.</remarks>
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "startingIndex")]
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
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "startingIndex")]
        public static void RemoveRange<T>(this IList<T> list, int startingIndex, int count)
        {
            if (list == null)
                throw new ArgumentNullException("list", "list is null.");
            if (list.IsReadOnly)
                throw new ArgumentException("list.IsReadOnly must be false", "list");
            if (startingIndex < 0)
                throw new ArgumentOutOfRangeException("startingIndex", startingIndex, "startingIndex must be >= 0");
            if (startingIndex >= list.Count)
                throw new ArgumentOutOfRangeException("startingIndex", startingIndex, "startingIndex must be < list.Count");


            for (int i = 0; i < count; i++)
                list.RemoveAt(startingIndex);
        }
    }
}
