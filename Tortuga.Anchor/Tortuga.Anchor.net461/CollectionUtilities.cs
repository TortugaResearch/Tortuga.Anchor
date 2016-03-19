using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

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
                throw new ArgumentNullException("target", "target is null.");
            if (target.IsReadOnly)
                throw new ArgumentException("target.IsReadOnly must be false", "target");
            if (list == null)
                throw new ArgumentNullException("list", "list is null.");


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
                throw new ArgumentNullException("target", "target is null.");
            if (target.IsReadOnly)
                throw new ArgumentException("target.IsReadOnly must be false", "target");
            if (list == null)
                throw new ArgumentNullException("list", "list is null.");

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
                throw new ArgumentNullException("target", "target is null.");
            if (target.IsReadOnly)
                throw new ArgumentException("target.IsReadOnly must be false", "target");
            if (list == null)
                throw new ArgumentNullException("list", "list is null.");


            foreach (var item in list)
                target.Add(item);
        }

        /// <summary>
        /// Concatenates the specified first.
        /// </summary>
        /// <typeparam name="TSource">The type of enumerable</typeparam>
        /// <param name="first">The source to be enumerated.</param>
        /// <param name="second">The item to be appended to the enumeration.</param>
        /// <returns>
        /// An System.Collections.Generic.IEnumerable&lt;T&gt; that contains the concatenated
        /// elements of the two input sequences.
        /// </returns>
        public static IEnumerable<TSource> Concat<TSource>(this IEnumerable<TSource> first, TSource second)
        {
            foreach (var item in first)
                yield return item;
            yield return second;
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
                throw new ArgumentNullException("target", "target is null.");
            if (target.IsReadOnly)
                throw new ArgumentException("target.IsReadOnly must be false", "target");
            if (list == null)
                throw new ArgumentNullException("list", "list is null.");
            if (startingIndex < 0)
                throw new ArgumentOutOfRangeException("startingIndex", startingIndex, "startingIndex must be >= 0");
            if (startingIndex > target.Count)
                throw new ArgumentOutOfRangeException("startingIndex", startingIndex, "startingIndex must be <= target.Count");


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
