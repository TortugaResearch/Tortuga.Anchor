using System.ComponentModel;
using Tortuga.Anchor.Eventing;

namespace Tests.Mocks;

public class WeakNotifier : Notifier, INotifyPropertyChangedWeak
{
	readonly PropertyChangedEventManager m_Manager;
	public WeakNotifier()
	{
		m_Manager = new PropertyChangedEventManager(this);
	}

	public void AddHandler(IListener<PropertyChangedEventArgs> eventHandler!!)
	{
		m_Manager.AddHandler(eventHandler);
	}

	public void RemoveHandler(IListener<PropertyChangedEventArgs> eventHandler!!)
	{
		m_Manager.RemoveHandler(eventHandler);
	}

}
