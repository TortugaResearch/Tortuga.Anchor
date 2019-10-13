using System;

namespace Tortuga.Anchor.ComponentModel
{
    /// <summary>
    /// This is used for ItemAdded and ItemRemoved events to indicate which item was affected.
    /// </summary>
    /// <typeparam name="T"></typeparam>

    public class ItemEventArgs<T> : EventArgs
    {
        /// <summary>
        /// Create a new instance of ItemEventArgs with the indicated item.
        /// </summary>
        /// <param name="item"></param>
        public ItemEventArgs(T item)
        {
            Item = item;
        }

        /// <summary>
        /// Item that was added or removed
        /// </summary>
        public T Item { get; private set; }
    }
}
