using System;
using System.Collections.Generic;

namespace Tortuga.Anchor.Collections
{
    /// <summary>
    /// Equality comparer for ValueTuple`2 where [Item1, Item2] equals [Item2, Item1].
    /// </summary>
    /// <remarks>Use with Dictionary`3 to provide a bi-directional mapping between [key1, key2] and [key2, key1].</remarks>
    public class ReversibleEqualityComparer<T> : IEqualityComparer<ValueTuple<T, T>>
    where T : IEquatable<T>
    {
        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type to compare.</param>
        /// <param name="y">The second object of type to compare.</param>
        /// <returns><see langword="true" /> if the specified objects are equal; otherwise, <see langword="false" />.</returns>
        public bool Equals(ValueTuple<T, T> x, ValueTuple<T, T> y)
        {
            return (x.Item1.Equals(y.Item1) && x.Item2.Equals(y.Item2)) || (x.Item1.Equals(y.Item2) & x.Item2.Equals(y.Item1));
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The value for which a hash code is to be returned.</param>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public int GetHashCode(ValueTuple<T, T> obj)
        {
            //Don't use System.HashCode. We need [x, y].GetHashCode to equal [y, x].GetHashCode
            return obj.Item1.GetHashCode() ^ obj.Item2.GetHashCode();
        }
    }
}
