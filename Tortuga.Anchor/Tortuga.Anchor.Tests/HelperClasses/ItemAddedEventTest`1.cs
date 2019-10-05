

using Tortuga.Anchor.ComponentModel;
using Tortuga.Dragnet;

namespace Tests.HelperClasses
{
    public class ItemAddedEventTest<T> : EventTest<ItemEventArgs<T>>
        where T : class
    {
        readonly INotifyCollectionChanged<T> m_Source;

        public ItemAddedEventTest(Verify verify, INotifyCollectionChanged<T> source)
            : base(verify, source)
        {
            m_Source = source;
            m_Source.ItemAdded += SourceEventFired;
        }

        public EventPair<ItemEventArgs<T>> ExpectEvent(T expectedItem)
        {
            var nextEvent = ExpectEvent();
            Verify.AreSame(expectedItem, nextEvent.EventArgs.Item, "The Item property was not set correctly.");
            return nextEvent;
        }
    }
}
