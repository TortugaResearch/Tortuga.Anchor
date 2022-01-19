using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Tortuga.Anchor.Collections
{
    /// <summary>
    /// This represents a collection of weak references.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    public sealed class WeakReferenceCollection<T> : ICollection<T>, IReadOnlyCollection<T> where T : class
    {
        List<WeakReference> m_Collection = new List<WeakReference>();

        /// <summary>
        /// Returns the count of live objects.
        /// </summary>
        /// <remarks>For a more accurate count call CleanUp before reading this property.</remarks>
        public int Count => m_Collection.Count;

        /// <summary>
        /// Always returns true
        /// </summary>
        bool ICollection<T>.IsReadOnly => false;

        /// <summary>
        /// Adds a weak reference to the indicated item
        /// </summary>
        /// <param name="item">The object to add to the <see cref="ICollection{T}" />.</param>
        /// <exception cref="ArgumentNullException">item</exception>
        public void Add(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), $"{nameof(item)} is null.");

            m_Collection.Add(new WeakReference(item));
        }

        /// <summary>
        /// Adds a list of values to this collection
        /// </summary>
        /// <param name="list">The list.</param>
        /// <exception cref="ArgumentNullException">list</exception>
        /// <exception cref="ArgumentException">list</exception>
        public void AddRange(IEnumerable<T> list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list), $"{nameof(list)} is null.");
            if (list.Any(i => i == null))
                throw new ArgumentException($"{nameof(list)} is not allowed to contain null items", nameof(list));

            foreach (var item in list)
                Add(item);
        }

        /// <summary>
        /// Removes the dead references from the collection
        /// </summary>
        public void CleanUp()
        {
            m_Collection = m_Collection.Where(x => x.IsAlive).ToList();
        }

        /// <summary>
        /// Removes all items from the collection
        /// </summary>
        public void Clear() => m_Collection.Clear();

        /// <summary>
        /// Returns true if the item is found in the collection
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>

        public bool Contains(T item)
        {
            if (Count == 0)
                return false;

            return m_Collection.Any(x => Equals(x.Target, item));
        }

        /// <summary>
        /// Copies a snapshot of the collection to an array.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="Array" /> that is the destination of the elements copied from <see cref="ICollection{T}" />. The <see cref="Array" /> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// <exception cref="ArgumentNullException">array - array is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// arrayIndex
        /// or
        /// arrayIndex
        /// </exception>
        /// <exception cref="ArgumentException">array</exception>

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array), "array is null");
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), arrayIndex, $"{nameof(arrayIndex)} cannot be less than zero");
            if (arrayIndex > array.Length)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), arrayIndex, $"{nameof(arrayIndex)} is greater than the array's length");
            if (Count + arrayIndex > array.Length)
                throw new ArgumentException($"{nameof(Count)} + {nameof(arrayIndex)} is greater than the array's length", nameof(array));

            var temp = m_Collection.Select(x => (T?)x.Target).Where(x => x != null).Cast<T>().ToList();

            for (var i = 0; i < temp.Count; i++)
                array[arrayIndex + i] = temp[i];
        }

        /// <summary>
        /// Returns an enumerator containing references that were live at the time this is called.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return m_Collection.Select(x => (T?)x.Target).Where(x => x != null).ToList().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Removes the indicated item from the array
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="ICollection{T}" />.</param>
        /// <returns>Returns true if the collection previously contained the item</returns>
        /// <exception cref="ArgumentNullException">item</exception>
        /// <remarks>If an item appears more than once only the first instance will be removed.</remarks>
        public bool Remove(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), $"{nameof(item)} is null.");

            var result = Contains(item);
            if (result)
            {
                var newCollection = new List<WeakReference>(m_Collection.Count);
                var found = false; //used to ensure we only remove one
                foreach (var wr in m_Collection)
                {
                    var target = (T?)wr.Target;
                    if (target == null)
                        continue;

                    if (found || !Equals(target, item))
                        newCollection.Add(wr);
                    else
                        found = true; //skip the first one;
                }
                m_Collection = newCollection;
            }
            return result;
        }
    }
}
