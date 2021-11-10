using System.Globalization;

namespace Tortuga.Dragnet;

/// <summary>
/// Base class for event tests
/// </summary>
/// <typeparam name="TEventArgs"></typeparam>
public abstract class EventTest<TEventArgs> where TEventArgs : EventArgs
{
	readonly Queue<EventPair<TEventArgs>> m_Queue = new();
	readonly object m_Source;
	readonly Verify m_Verify;

	/// <summary>
	/// This us used for tracking the expected source of an event.
	/// </summary>
	/// <param name="source"></param>
	protected EventTest(Verify verify, object source)
	{
		if (verify == null)
			throw new ArgumentNullException("verify", "verify is null.");
		if (source == null)
			throw new ArgumentNullException("source", "source is null.");

		m_Verify = verify;
		m_Source = source;
	}

	/// <summary>
	/// The number of events that have not yet been consumed.
	/// </summary>
	public int Count
	{
		get { return m_Queue.Count; }
	}

	/// <summary>
	/// Gets the verification object.
	/// </summary>
	protected Verify Verify
	{
		get { return m_Verify; }
	}

	/// <summary>
	/// Asserts that the number of pending events equals a given amount without actually consuming those events.
	/// </summary>
	/// <param name="expectedValue"></param>
	public void ExpectCountEquals(int expectedValue)
	{
		ExpectCountEquals(expectedValue, "");
	}

	/// <summary>
	/// Asserts that the number of pending events equals a given amount without actually consuming those events.
	/// </summary>
	/// <param name="expectedValue"></param>
	/// <param name="message"></param>
	public void ExpectCountEquals(int expectedValue, string message)
	{
		if (Count != expectedValue)
		{
			string errorMessage = string.Format(CultureInfo.CurrentUICulture, "Expected {0} events but found {1}", expectedValue, Count);

			if (!string.IsNullOrWhiteSpace(message))
				Verify.Fail(message + Environment.NewLine + errorMessage);
			else
				Verify.Fail(errorMessage);
		}
	}

	/// <summary>
	/// Asserts that an event is in the queue.
	/// </summary>
	/// <returns>The first event in the queue</returns>
	/// <remarks>This will verify that the sender parameter is correct. This will remove the event from the queue.</remarks>
	public EventPair<TEventArgs> ExpectEvent()
	{
		return ExpectEvent("");
	}

	/// <summary>
	/// Asserts that an event is in the queue.
	/// </summary>
	/// <returns>The first event in the queue</returns>
	/// <remarks>This will verify that the sender parameter is correct. This will remove the event from the queue.</remarks>
	public EventPair<TEventArgs> ExpectEvent(string message)
	{
		if (Count == 0)
		{
			string errorMessage = string.Format(CultureInfo.CurrentUICulture, "No events of type {0} remain in the queue.", typeof(TEventArgs).Name);

			if (!string.IsNullOrWhiteSpace(message))
				Verify.Fail(message + Environment.NewLine + errorMessage);
			else
				Verify.Fail(errorMessage);
		}

		var nextEvent = m_Queue.Dequeue();
		Verify.AreSame(m_Source, nextEvent.Sender, "The sender was not the the object that raised the event");

		return nextEvent;
	}

	/// <summary>
	/// Asserts that the event queue is empty.
	/// </summary>
	public void ExpectNothing()
	{
		ExpectCountEquals(0, null);
	}

	/// <summary>
	/// Asserts that the event queue is empty.
	/// </summary>
	public void ExpectNothing(string message)
	{
		ExpectCountEquals(0, message);
	}

	/// <summary>
	/// Attach this to the event in the subclass constructor
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void SourceEventFired(object? sender, TEventArgs e)
	{
		m_Queue.Enqueue(new EventPair<TEventArgs>(sender, e));
	}
}
