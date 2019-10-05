using System;

namespace Tortuga.Anchor.ComponentModel
{
    /// <summary>
    /// This is a type-safe version of INotifyCollectionChanged
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface INotifyCollectionChanged<T>
    {
        /// <summary>
        /// This type safe event fires after an item is added to the collection no matter how it is added.
        /// </summary>
        /// <remarks>Triggered by InsertItem and SetItem</remarks>
        event EventHandler<ItemEventArgs<T>> ItemAdded;

        /// <summary>
        /// This type safe event fires after an item is removed from the collection no matter how it is removed.
        /// </summary>
        /// <remarks>Triggered by SetItem, RemoveItem, and ClearItems</remarks>
        event EventHandler<ItemEventArgs<T>> ItemRemoved;
    }
}
