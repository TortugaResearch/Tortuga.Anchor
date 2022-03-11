using System.Collections.Specialized;

namespace Tortuga.Dragnet;

/// <summary>
/// Assertions for INotifyCollectionChanged
/// </summary>
public class CollectionChangedEventTest : EventTest<NotifyCollectionChangedEventArgs>
{
	readonly INotifyCollectionChanged m_Source;


	/// <summary>
	/// Creates a new INotifyCollectionChanged assertion
	/// </summary>
	/// <param name="source"></param>
	public CollectionChangedEventTest(Verify verify, INotifyCollectionChanged source)
		: base(verify, source)
	{
		if (source == null)
			throw new ArgumentNullException("source", "source is null.");

		m_Source = source;
		m_Source.CollectionChanged += SourceEventFired;
	}

	/// <summary>
	/// Asserts that an event is in the queue and that it has the indicated properties.
	/// </summary>
	/// <param name="action">Expected action value.</param>
	/// <returns>This will remove the event from the queue.</returns>
	public EventPair<NotifyCollectionChangedEventArgs> ExpectEvent(NotifyCollectionChangedAction action)
	{
		var nextEvent = ExpectEvent();
		Verify.AreEqual(action, nextEvent.EventArgs.Action, "The wrong action was set.");
		return nextEvent;
	}
}
