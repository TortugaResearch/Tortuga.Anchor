namespace Tortuga.Anchor.Eventing;

/// <summary>
/// A relayed event wraps a sender/event args pair so that it can be forwarded by another class.
/// </summary>
public static class RelayedEventArgs
{
	/// <summary>
	/// Create a new relayed event from an existing event
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="originalSender">The original sender.</param>
	/// <param name="eventArgs">The event arguments.</param>
	/// <returns>RelayedEventArgs&lt;T&gt;.</returns>
	/// <exception cref="ArgumentNullException">eventArgs</exception>
	public static RelayedEventArgs<T> Create<T>(object? originalSender, T eventArgs!!) where T : EventArgs
	{
		return new RelayedEventArgs<T>(originalSender, eventArgs);
	}
}
