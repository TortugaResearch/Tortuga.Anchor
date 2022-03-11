namespace Tortuga.Anchor.Eventing;

/// <summary>
/// A relayed event wraps a sender/event args pair so that it can be forwarded by another class.
/// Implements the <see cref="EventArgs" />
/// </summary>
/// <typeparam name="T"></typeparam>
/// <seealso cref="EventArgs" />

public class RelayedEventArgs<T> : EventArgs where T : EventArgs
{
	/// <summary>
	/// Create a new relayed event from an existing event
	/// </summary>
	/// <param name="originalSender">The original sender.</param>
	/// <param name="eventArgs">The event arguments.</param>
	/// <exception cref="ArgumentNullException">eventArgs</exception>
	public RelayedEventArgs(object? originalSender, T eventArgs)
	{
		EventArgs = eventArgs ?? throw new ArgumentNullException(nameof(eventArgs), $"{nameof(eventArgs)} is null.");
		OriginalSender = originalSender;
	}

	/// <summary>
	/// The wrapped event args
	/// </summary>
	public T EventArgs { get; }

	/// <summary>
	/// The object that raised the original event
	/// </summary>
	public object? OriginalSender { get; }
}
