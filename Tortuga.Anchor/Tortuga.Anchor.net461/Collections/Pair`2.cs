using System;
using System.Collections.Generic;

namespace Tortuga.Anchor.Collections
{
    /// <summary>
    /// A lightweight Tuple implemented as a Value type.
    /// </summary>
    /// <typeparam name="T1">The type of the first item.</typeparam>
    /// <typeparam name="T2">The type of the second item.</typeparam>
    public struct Pair<T1, T2>
        where T1 : IEquatable<T1>
        where T2 : IEquatable<T2>
    {
        private readonly T1 m_Item1;
        private readonly T2 m_Item2;

        /// <summary>
        /// Initializes a new instance of the <see cref="Pair{T1, T2}"/> struct.
        /// </summary>
        /// <param name="item1">The first item.</param>
        /// <param name="item2">The second item.</param>
        public Pair(T1 item1, T2 item2)
        {
            m_Item1 = item1;
            m_Item2 = item2;
        }

        /// <summary>
        /// Gets the first item.
        /// </summary>
        /// <value>
        /// The item1.
        /// </value>
        public T1 Item1
        {
            get { return m_Item1; }
        }

        /// <summary>
        /// Gets the second item.
        /// </summary>
        /// <value>
        /// The item2.
        /// </value>
        public T2 Item2
        {
            get { return m_Item2; }
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Pair{T1, T2}"/> to <see cref="KeyValuePair{T1, T2}"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator KeyValuePair<T1, T2>(Pair<T1, T2> value)
        {
            return value.ToKeyValuePair();
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Tuple{T1, T2}"/> to <see cref="Pair{T1, T2}"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator Pair<T1, T2>(Tuple<T1, T2> value)
        {
            return Pair.FromTuple(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="KeyValuePair{T1, T2}"/> to <see cref="Pair{T1, T2}"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator Pair<T1, T2>(KeyValuePair<T1, T2> value)
        {
            return Pair.FromKeyValuePair(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Pair{T1, T2}"/> to <see cref="Tuple{T1, T2}"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator Tuple<T1, T2>(Pair<T1, T2> value)
        {
            return value.ToTuple();
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="pair1">The pair1.</param>
        /// <param name="pair2">The pair2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(Pair<T1, T2> pair1, Pair<T1, T2> pair2)
        {
            return !pair1.Equals(pair2);
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="pair1">The pair1.</param>
        /// <param name="pair2">The pair2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(Pair<T1, T2> pair1, Pair<T1, T2> pair2)
        {
            return pair1.Equals(pair2);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />.
        /// </summary>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">
        /// The object to compare with the current object. 
        /// </param>
        public override bool Equals(object obj)
        {
            if (!(obj is Pair<T1, T2>))
                return false;

            return Equals((Pair<T1, T2>)obj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Pair<T1, T2> other)
        {
            if (!Item1.Equals(other.Item1))
                return false;

            return Item2.Equals(other.Item2);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object" />.
        /// </returns>
        public override int GetHashCode()
        {
            return Item1.GetHashCode() ^ Item2.GetHashCode();
        }
        /// <summary>
        /// Convert to a KeyValuePair.
        /// </summary>
        /// <returns></returns>
        public KeyValuePair<T1, T2> ToKeyValuePair()
        {
            return new KeyValuePair<T1, T2>(Item1, Item2);
        }

        /// <summary>
        /// Convert to a tuple.
        /// </summary>
        /// <returns></returns>
        public Tuple<T1, T2> ToTuple()
        {
            return new Tuple<T1, T2>(Item1, Item2);
        }
    }
}
