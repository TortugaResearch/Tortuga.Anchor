

using Tortuga.Anchor.ComponentModel;
using Tortuga.Dragnet;

namespace Tests.HelperClasses;

public class ItemRemovedEventTest<T> : EventTest<ItemEventArgs<T>>
	where T : class
{
	readonly INotifyCollectionChanged<T> m_Source;

	public ItemRemovedEventTest(Verify verify, INotifyCollectionChanged<T> source)
		: base(verify, source)
	{
		m_Source = source;
		m_Source.ItemRemoved += SourceEventFired;
	}

	public EventPair<ItemEventArgs<T>> ExpectEvent(T expectedItem)
	{
		var nextEvent = ExpectEvent();
		Verify.AreSame(expectedItem, nextEvent.EventArgs.Item, "The Item property was not set correctly.");
		return nextEvent;
	}
}
