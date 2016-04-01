using System.Collections;
using System.Collections.Generic;

namespace Tortuga.Anchor
{
    /// <summary>
    /// Interface IReadOnlyDictionary
    /// </summary>
    /// <typeparam name="TKey">The type of the t key.</typeparam>
    /// <typeparam name="TValue">The type of the t value.</typeparam>
    internal interface IReadOnlyDictionary<TKey, TValue> : IReadOnlyCollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
    {

        /// <summary>
        /// Gets the TValue with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>TValue.</returns>
        TValue this[TKey key] { get; }

        /// <summary>
        /// Gets the keys.
        /// </summary>
        /// <value>The keys.</value>
        IEnumerable<TKey> Keys { get; }

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <value>The values.</value>
        IEnumerable<TValue> Values { get; }

        /// <summary>
        /// Determines whether the specified key contains key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if the specified key contains key; otherwise, <c>false</c>.</returns>
        bool ContainsKey(TKey key);

        /// <summary>
        /// Tries the get value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool TryGetValue(TKey key, out TValue value);
    }
}
