using System.ComponentModel;

namespace Tortuga.Anchor.Eventing;

/// <summary>
/// Indicates that property change notifications on child objects are relayed by this class
/// </summary>
public interface INotifyItemPropertyChanged
{
	/// <summary>
	/// Use this event to listen for changes to properties on items contained by this object without having to explicitly attach an event handler to each item.
	/// </summary>
	event RelayedEventHandler<PropertyChangedEventArgs> ItemPropertyChanged;
}
