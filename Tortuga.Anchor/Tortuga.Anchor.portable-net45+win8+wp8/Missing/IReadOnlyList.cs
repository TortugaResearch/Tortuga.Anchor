using System.Collections;
using System.Collections.Generic;

namespace Tortuga.Anchor
{
    /// <summary>
    /// Interface IReadOnlyList
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal interface IReadOnlyList<out T> : IReadOnlyCollection<T>, IEnumerable<T>, IEnumerable
    {

        /// <summary>
        /// Gets the T at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>T.</returns>
        T this[int index] { get; }
    }
}
