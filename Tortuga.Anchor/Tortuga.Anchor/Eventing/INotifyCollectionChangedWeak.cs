using System.Collections.Specialized;

#pragma warning disable CA1716 // Identifiers should not match keywords

namespace Tortuga.Anchor.Eventing;

/// <summary>
/// This interface allows attaching and removing weak event handlers that listen for CollectionChanged events
/// </summary>
public interface INotifyCollectionChangedWeak : INotifyCollectionChanged
{
	/// <summary>
	/// Attach a weak event handler to this object
	/// </summary>
	/// <param name="eventHandler"></param>
	void AddHandler(IListener<NotifyCollectionChangedEventArgs> eventHandler);

	/// <summary>
	/// Remove a weak event handler from this object
	/// </summary>
	/// <param name="eventHandler"></param>
	void RemoveHandler(IListener<NotifyCollectionChangedEventArgs> eventHandler);
}
