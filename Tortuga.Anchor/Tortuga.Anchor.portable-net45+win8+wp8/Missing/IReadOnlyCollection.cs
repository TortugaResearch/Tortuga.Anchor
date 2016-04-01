using System.Collections;
using System.Collections.Generic;

namespace Tortuga.Anchor
{
    /// <summary>
    /// Interface IReadOnlyCollection
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal interface IReadOnlyCollection<out T> : IEnumerable<T>, IEnumerable
    {
        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        int Count { get; }
    }
}
