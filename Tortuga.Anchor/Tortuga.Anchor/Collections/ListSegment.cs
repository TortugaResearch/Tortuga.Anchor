﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace Tortuga.Anchor.Collections
{
    /// <summary>
    /// This represents a segment of an IList.
    /// </summary>
    [SuppressMessage("Design", "CA1066:Type {0} should implement IEquatable<T> because it overrides Equals", Justification = "<Pending>")]
    [SuppressMessage("Naming", "CA1710:Identifiers should have correct suffix", Justification = "<Pending>")]
    public readonly struct ListSegment<T> : IList<T>, IReadOnlyList<T>
    {
        private readonly IList<T>? m_List;

        /// <summary>
        /// Used to detect if the underlying list has been modified.
        /// </summary>
        private readonly int m_InitialCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListSegment{T}" /> struct.
        /// </summary>
        /// <param name="list">The array.</param>
        /// <exception cref="ArgumentNullException">list</exception>
        /// <exception cref="ArgumentException">list</exception>
        public ListSegment(IList<T> list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list), $"{nameof(list)} is null.");
            if (list.Count == 0)
                throw new ArgumentException($"{nameof(list)} is empty.", nameof(list));

            m_List = list;
            Offset = 0;
            Count = list.Count;
            m_InitialCount = list.Count;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListSegment{T}"/> struct.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        /// <exception cref="ArgumentNullException">list</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// offset
        /// or
        /// count
        /// </exception>
        public ListSegment(IList<T> list, int offset, int count)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list), $"{nameof(list)} is null.");
            if (list.Count == 0)
                throw new ArgumentException($"{nameof(list)} is empty.", nameof(list));

            if (offset > list.Count)
                throw new ArgumentOutOfRangeException(nameof(offset), offset, $"{offset} is greater than list size of {list.Count}");
            if (count > (list.Count - offset))
                throw new ArgumentOutOfRangeException(nameof(count), count, $"{count} is greater than list size of {list.Count} minus offset of {offset}");
            if (count == 0)
                throw new ArgumentOutOfRangeException(nameof(count), count, $"{count} must be greater than zero");

            m_List = list;
            Offset = offset;
            Count = count;
            m_InitialCount = list.Count;
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count { get; }

        bool ICollection<T>.IsReadOnly => true;

        /// <summary>
        /// Gets the underlying list.
        /// </summary>
        /// <value>The list.</value>
        /// <exception cref="InvalidOperationException">The ListSegment is empty.</exception>
        public IList<T> List
        {
            get
            {
                CheckSource();
                return m_List!;
            }
        }

        /// <summary>
        /// Gets the offset.
        /// </summary>
        /// <value>The offset.</value>
        public int Offset { get; }

        /// <summary>
        /// Gets or sets the value at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>T.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// index
        /// or
        /// index
        /// </exception>
        public T this[int index]
        {
            get
            {
                if (index >= Count)
                    throw new ArgumentOutOfRangeException(nameof(index), index, $"{index} is greater than segment of {Count}");

                return List[Offset + index];
            }
            set
            {
                if (index >= Count)
                    throw new ArgumentOutOfRangeException(nameof(index), index, $"{index} is greater than segment of {Count}");

                List[Offset + index] = value;
            }
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="List{T}"/> to <see cref="ListSegment{T}"/>.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns>The result of the conversion.</returns>
        [SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "<Pending>")]
        public static implicit operator ListSegment<T>(List<T> list) => list != null ? new ListSegment<T>(list) : default;

        /// <summary>
        /// Performs an implicit conversion from <see cref="List{T}"/> to <see cref="ListSegment{T}"/>.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <returns>The result of the conversion.</returns>
        [SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "<Pending>")]
        public static implicit operator ListSegment<T>(Collection<T> collection) => collection != null ? new ListSegment<T>(collection) : default;

        /// <summary>
        /// Implements the != operator.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(ListSegment<T> a, ListSegment<T> b) => !(a == b);

        /// <summary>
        /// Implements the == operator.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(ListSegment<T> a, ListSegment<T> b) => a.Equals(b);

        void ICollection<T>.Add(T item) => throw new NotSupportedException();

        //cannot add/remove items. Doesn't say anything about replacing them.
        void ICollection<T>.Clear() => throw new NotSupportedException();

        /// <summary>
        /// Determines whether the list segment contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the list segment.</param>
        public bool Contains(T item) => List.Contains(item);

        /// <summary>
        /// Copies the list segment to the destination array.
        /// </summary>
        /// <param name="destination">The destination.</param>
        public void CopyTo(T[] destination) => CopyTo(destination, 0);

        /// <summary>
        /// Copies the list segment to the destination array.
        /// </summary>
        /// <param name="destination">The destination.</param>
        /// <param name="destinationIndex">Index of the destination.</param>
        public void CopyTo(T[] destination, int destinationIndex)
        {
            if (m_List == null)
                throw new InvalidOperationException("The ListSegment is empty.");

            if (destination == null || destination.Length == 0)
                throw new ArgumentException($"{nameof(destination)} is null or empty.", nameof(destination));

            var j = destinationIndex;
            for (var i = Offset; i < Count; i++)
            {
                if (destination.Length == j)
                    return;

                destination[j] = m_List[i];
                j++;
            }
        }

        /// <summary>
        /// Copies the list segment to the destination list segment.
        /// </summary>
        /// <param name="destination">The destination.</param>
        /// <exception cref="InvalidOperationException">The ListSegment is empty.</exception>
        /// <exception cref="ArgumentException"></exception>
        public void CopyTo(ListSegment<T> destination)
        {
            if (m_List == null)
                throw new InvalidOperationException("The ListSegment is empty.");
            if (destination.Count == 0)
                throw new ArgumentException($"{nameof(destination)} is empty.");
            var j = 0;
            for (var i = Offset; i < Count; i++)
            {
                if (destination.Count == j)
                    return;

                destination[j] = m_List[i];
                j++;
            }
        }

        void CheckSource()
        {
            if (m_List == null)
                throw new InvalidOperationException("The ListSegment is empty.");

            if (m_List.Count != m_InitialCount)
                throw new InvalidOperationException("The size of the underlying list has been modified. This array segment is no longer valid.");
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="other">The object to compare with the current instance.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object? other) =>
            other is ListSegment<T> && Equals((ListSegment<T>)other);

        /// <summary>
        /// Returns true if both list segments refer to the same list at the same offset and count.
        /// </summary>
        /// <param name="other">The other segment.</param>
        /// <returns>System.Boolean.</returns>
        public bool Equals(ListSegment<T> other) =>
                            other.m_List == m_List && other.Offset == Offset && other.Count == Count;

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>Enumerator.</returns>
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode() =>
            m_List is null ? 0 : HashCode.Combine(Offset, Count, m_List.GetHashCode());

        int IList<T>.IndexOf(T item)
        {
            var index = List.IndexOf(item);
            return index >= 0 ? index - Offset : -1;
        }

        void IList<T>.Insert(int index, T item) => throw new NotSupportedException();

        bool ICollection<T>.Remove(T item) => throw new NotSupportedException();

        void IList<T>.RemoveAt(int index) => throw new NotSupportedException();

        /// <summary>
        /// Slices the segment specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>ListSegment&lt;T&gt;.</returns>
        public ListSegment<T> Slice(int index)
        {
            if (index > Count)
                throw new ArgumentOutOfRangeException(nameof(index), index, $"{index} is greater than segment size of {Count}");

            return new ListSegment<T>(List, Offset + index, Count - index);
        }

        /// <summary>
        /// Slices the segment specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="count">The count.</param>
        /// <returns>ListSegment&lt;T&gt;.</returns>
        public ListSegment<T> Slice(int index, int count)
        {
            if (index > Count)
                throw new ArgumentOutOfRangeException(nameof(index), index, $"{index} is greater than segment size of {Count}");
            if (count > (Count - index))
                throw new ArgumentOutOfRangeException(nameof(count), count, $"{count} is greater than segment size of {Count} minus offset of {index}");

            return new ListSegment<T>(List, Offset + index, count);
        }

        /// <summary>
        /// Enumerator for a ListSegment
        /// Implements the <see cref="System.Collections.Generic.IEnumerator{T}" /></summary>
        /// <seealso cref="System.Collections.Generic.IEnumerator{T}" />
        public struct Enumerator : IEnumerator<T>
        {
            private readonly int m_End;
            private readonly IList<T> m_List;
            private readonly int m_Start;
            private int m_Current;

            internal Enumerator(ListSegment<T> ListSegment)
            {
                m_List = ListSegment.List;
                m_Start = ListSegment.Offset;
                m_End = ListSegment.Offset + ListSegment.Count;
                m_Current = ListSegment.Offset - 1;
            }

            /// <summary>
            /// Gets the element in the collection at the current position of the enumerator.
            /// </summary>
            public T Current
            {
                get
                {
                    if (m_Current < m_Start)
                        throw new InvalidOperationException("Enumeration hasn't started. Call MoveNext().");
                    if (m_Current >= m_End)
                        throw new InvalidOperationException("Moved past end of list. Current isn't available if MoveNext() returns false. Call Reset to restart the enumeration.");
                    return m_List[m_Current];
                }
            }

            object? IEnumerator.Current => Current;

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose() { }

            /// <summary>
            /// Advances the enumerator to the next element of the collection.
            /// </summary>
            /// <returns>
            ///   <see langword="true" /> if the enumerator was successfully advanced to the next element; <see langword="false" /> if the enumerator has passed the end of the collection.</returns>
            public bool MoveNext()
            {
                if (m_Current < m_End)
                {
                    m_Current++;
                    return m_Current < m_End;
                }
                return false;
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            void IEnumerator.Reset() => m_Current = m_Start - 1;
        }
    }
}