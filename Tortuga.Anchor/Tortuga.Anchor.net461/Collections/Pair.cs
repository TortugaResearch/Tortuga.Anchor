
using System;
using System.Collections.Generic
    ;
namespace Tortuga.Anchor.Collections
{
    /// <summary>
    /// Helper methods for the Pair structure.
    /// </summary>
    public static class Pair
    {
        /// <summary>
        /// Creates a pair.
        /// </summary>
        /// <typeparam name="T1">The type of the first item.</typeparam>
        /// <typeparam name="T2">The type of the second item.</typeparam>
        /// <param name="item1">The first item.</param>
        /// <param name="item2">The second item.</param>
        /// <returns></returns>
        public static Pair<T1, T2> Create<T1, T2>(T1 item1, T2 item2)
            where T1 : IEquatable<T1>
            where T2 : IEquatable<T2>
        {
            return new Pair<T1, T2>(item1, item2);
        }

        /// <summary>
        /// Convert from a tuple.
        /// </summary>
        /// <typeparam name="T1">The type of the first item.</typeparam>
        /// <typeparam name="T2">The type of the second item.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>Pair&lt;T1, T2&gt;.</returns>
        public static Pair<T1, T2> FromTuple<T1, T2>(Tuple<T1, T2> value)
            where T1 : IEquatable<T1>
            where T2 : IEquatable<T2>
        {
            if (value == null)
                return new Pair<T1, T2>(default(T1), default(T2));

            return new Pair<T1, T2>(value.Item1, value.Item2);
        }

        /// <summary>
        /// Convert from a KeyValuePair.
        /// </summary>
        /// <typeparam name="T1">The type of the first item.</typeparam>
        /// <typeparam name="T2">The type of the second item.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>Pair&lt;T1, T2&gt;.</returns>
        public static Pair<T1, T2> FromKeyValuePair<T1, T2>(KeyValuePair<T1, T2> value)
            where T1 : IEquatable<T1>
            where T2 : IEquatable<T2>
        {
            return new Pair<T1, T2>(value.Key, value.Value);
        }
    }
}
