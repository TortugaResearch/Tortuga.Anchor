using System.ComponentModel;
using Tortuga.Anchor.Eventing;
using Tortuga.Dragnet;



namespace Tests.HelperClasses
{
    public class ItemPropertyChangedEventTest : EventTest<RelayedEventArgs<PropertyChangedEventArgs>>
    {
        readonly INotifyItemPropertyChanged m_Source;

        public ItemPropertyChangedEventTest(Verify verify, INotifyItemPropertyChanged source)
            : base(verify, source)
        {
            m_Source = source;
            m_Source.ItemPropertyChanged += SourceEventFired;
        }

        public EventPair<RelayedEventArgs<PropertyChangedEventArgs>> ExpectEvent(object originalSender, string propertyName)
        {
            var nextEvent = ExpectEvent();
            Verify.AreSame(originalSender, nextEvent.EventArgs.OriginalSender, "The originalSender was not set correctly.");
            Verify.AreEqual(propertyName, nextEvent.EventArgs.EventArgs.PropertyName, "The wrong property name was set correctly.");
            return nextEvent;
        }



    }
}