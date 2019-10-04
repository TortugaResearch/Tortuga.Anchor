using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;

namespace Tortuga.Anchor.Eventing
{
    /// <summary>
    /// This interface allows attaching and removing weak event handlers that listen for CollectionChanged events
    /// </summary>
    public interface INotifyCollectionChangedWeak : INotifyCollectionChanged
    {
        /// <summary>
        /// Attach a weak event handler to this object
        /// </summary>
        /// <param name="eventHandler"></param>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "AddHandler")]
        void AddHandler(IListener<NotifyCollectionChangedEventArgs> eventHandler);

        /// <summary>
        /// Remove a weak event handler from this object
        /// </summary>
        /// <param name="eventHandler"></param>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "RemoveHandler")]
        void RemoveHandler(IListener<NotifyCollectionChangedEventArgs> eventHandler);
    }
}