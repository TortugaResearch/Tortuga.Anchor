using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Tortuga.Anchor.ComponentModel;
using Tortuga.Anchor.Eventing;

namespace Tortuga.Anchor.Collections;

/// <summary>
/// This is an ObservableCollection with a read-only wrapper and support for weak events.
/// This will use weak events to listen to objects implementing INotifyPropertyChangedWeak.
/// This will use normal events to listen to objects implementing INotifyPropertyChanged.
/// </summary>
/// <typeparam name="T">The type of elements in the collection.</typeparam>
public partial class ObservableCollectionExtended<T> : ObservableCollection<T>, INotifyCollectionChangedWeak, INotifyPropertyChangedWeak, INotifyItemPropertyChangedWeak, IReadOnlyList<T>
{
	CollectionChangedEventManager? m_CollectionChangeEventManager;

	IListener<PropertyChangedEventArgs>? m_ItemPropertyChanged;

	ItemPropertyChangedEventManager? m_ItemPropertyChangedEventManager;

	/// <summary>
	/// When someone attaches to the ItemPropertyChanged event this is set to true and we start listening for change notifications.
	/// </summary>
	bool m_ListeningToItemEvents;

	PropertyChangedEventManager? m_PropertyChangedEventManager;

	//These are created on demand.
	ReadOnlyObservableCollectionExtended<T>? m_ReadOnlyWrapper;

	/// <summary>
	/// Initializes a new instance of the ObservableCollectionExtended class.
	/// </summary>
	public ObservableCollectionExtended()
	{
	}

	/// <summary>
	/// Initializes a new instance of the ObservableCollectionExtended class that contains elements copied from the specified list.
	/// </summary>
	/// <param name="list"></param>
	public ObservableCollectionExtended(List<T> list)
	{
		if (list != null)
			AddRange(list);
	}

	/// <summary>
	/// Initializes a new instance of the ObservableCollectionExtended class that contains elements copied from the specified collection.
	/// </summary>
	/// <param name="collection"></param>
	public ObservableCollectionExtended(IEnumerable<T> collection)
	{
		if (collection != null)
			AddRange(collection);
	}

	/// <summary>
	/// This type safe event fires after an item is added to the collection no matter how it is added.
	/// </summary>
	/// <remarks>Triggered by InsertItem and SetItem</remarks>
	public event EventHandler<ItemEventArgs<T>>? ItemAdded;

	/// <summary>
	/// Use this event to listen for changes to properties on items contained by this collection without having to explicitly attach an event handler to each item.
	/// </summary>
	public event RelayedEventHandler<PropertyChangedEventArgs> ItemPropertyChanged
	{
		add
		{
			ListenToEvents();
			m_ItemPropertyChangedEvent += value;
		}
		remove { m_ItemPropertyChangedEvent -= value; }
	}

	/// <summary>
	/// This type safe event fires after an item is removed from the collection no matter how it is removed.
	/// </summary>
	/// <remarks>Triggered by SetItem, RemoveItem, and ClearItems</remarks>
	public event EventHandler<ItemEventArgs<T>>? ItemRemoved;

	/// <summary>
	/// This just exposes the INotifyPropertyChanged.PropertyChanged from the base class so you don't have to cast to get to it.
	/// </summary>
	public new event PropertyChangedEventHandler? PropertyChanged
	{
		add { base.PropertyChanged += value; }
		remove { base.PropertyChanged -= value; }
	}

#pragma warning disable IDE1006 // Naming Styles

	event RelayedEventHandler<PropertyChangedEventArgs>? m_ItemPropertyChangedEvent;

#pragma warning restore IDE1006 // Naming Styles

	/// <summary>
	/// Returns a read-only wrapper around this collection.
	/// </summary>
	/// <remarks>
	/// If sub classing this class then it may be useful to shadow ReadOnlyWrapper method
	/// with one that returns a subclass of ReadOnlyObservableCollectionExtended.
	/// </remarks>
	public ReadOnlyObservableCollectionExtended<T> ReadOnlyWrapper
	{
		get
		{
			if (m_ReadOnlyWrapper == null)
				m_ReadOnlyWrapper = new ReadOnlyObservableCollectionExtended<T>(this);

			return m_ReadOnlyWrapper;
		}
	}

	/// <summary>
	/// Adds a weak event handler
	/// </summary>
	/// <param name="eventHandler">The event handler.</param>
	/// <exception cref="ArgumentNullException">eventHandler</exception>
	public void AddHandler(IListener<NotifyCollectionChangedEventArgs> eventHandler)
	{
		if (eventHandler == null)
			throw new ArgumentNullException(nameof(eventHandler), $"{nameof(eventHandler)} is null.");

		if (m_CollectionChangeEventManager == null)
			m_CollectionChangeEventManager = new CollectionChangedEventManager(this);

		m_CollectionChangeEventManager.AddHandler(eventHandler);
	}

	/// <summary>
	/// Adds a weak event handler
	/// </summary>
	/// <param name="eventHandler"></param>
	public void AddHandler(IListener<PropertyChangedEventArgs> eventHandler)
	{
		if (eventHandler == null)
			throw new ArgumentNullException(nameof(eventHandler), $"{nameof(eventHandler)} is null.");

		if (m_PropertyChangedEventManager == null)
			m_PropertyChangedEventManager = new PropertyChangedEventManager(this);

		m_PropertyChangedEventManager.AddHandler(eventHandler);
	}

	/// <summary>
	/// Adds a weak event handler
	/// </summary>
	/// <param name="eventHandler"></param>
	public void AddHandler(IListener<RelayedEventArgs<PropertyChangedEventArgs>> eventHandler)
	{
		if (eventHandler == null)
			throw new ArgumentNullException(nameof(eventHandler), $"{nameof(eventHandler)} is null.");

		if (m_ItemPropertyChangedEventManager == null)
		{
			m_ItemPropertyChangedEventManager = new ItemPropertyChangedEventManager(this);
			ListenToEvents();
		}

		m_ItemPropertyChangedEventManager.AddHandler(eventHandler);
	}

	/// <summary>
	/// Adds a list of values to this collection
	/// </summary>
	/// <param name="list"></param>
	public void AddRange(IEnumerable<T> list)
	{
		if (list == null)
			throw new ArgumentNullException(nameof(list), $"{nameof(list)} is null.");

		foreach (var item in list)
			Add(item);
	}

	/// <summary>
	/// Removes all the elements that match the conditions defined by the specified predicate.
	/// </summary>
	/// <param name="match"> The System.Predicate`1 delegate that defines the conditions of the elements to remove.</param>
	/// <returns>The number of elements removed.</returns>
	public int RemoveAll(Predicate<T> match)
	{
		if (match == null)
			throw new ArgumentNullException(nameof(match), $"{nameof(match)} is null.");

		var count = 0;
		for (int i = Count - 1; i >= 0; i--)
		{
			if (match(this[i]))
			{
				RemoveAt(i);
				count += 1;
			}
		}
		return count;
	}

	/// <summary>
	/// Removes a weak event handler
	/// </summary>
	/// <param name="eventHandler"></param>
	public void RemoveHandler(IListener<NotifyCollectionChangedEventArgs> eventHandler)
	{
		if (eventHandler == null)
			throw new ArgumentNullException(nameof(eventHandler), $"{nameof(eventHandler)} is null.");

		if (m_CollectionChangeEventManager == null)
			return;

		m_CollectionChangeEventManager.RemoveHandler(eventHandler);
	}

	/// <summary>
	/// Removes a weak event handler
	/// </summary>
	/// <param name="eventHandler"></param>
	public void RemoveHandler(IListener<PropertyChangedEventArgs> eventHandler)
	{
		if (eventHandler == null)
			throw new ArgumentNullException(nameof(eventHandler), $"{nameof(eventHandler)} is null.");

		m_PropertyChangedEventManager?.RemoveHandler(eventHandler);
	}

	/// <summary>
	/// Removes a weak event handler
	/// </summary>
	/// <param name="eventHandler"></param>
	public void RemoveHandler(IListener<RelayedEventArgs<PropertyChangedEventArgs>> eventHandler)
	{
		if (eventHandler == null)
			throw new ArgumentNullException(nameof(eventHandler), $"{nameof(eventHandler)} is null.");

		m_ItemPropertyChangedEventManager?.RemoveHandler(eventHandler);
	}

	/// <summary>
	/// Removes all elements from the collection.
	/// </summary>
	protected override void ClearItems()
	{
		var temp = this.ToList();
		base.ClearItems();
		foreach (var item in temp)
			OnItemRemoved(item);
	}

	/// <summary>
	/// Inserts an item into the collection at the specified index.
	/// </summary>
	/// <param name="index">The zero-based index at which item should be inserted.</param>
	/// <param name="item">The object to insert.</param>
	/// <remarks>Do NOT invoke this method directly. This may be overridden to provide additional validation before an item is added to the collection.</remarks>
	protected override void InsertItem(int index, T item)
	{
		base.InsertItem(index, item);
		OnItemAdded(item);
	}

	/// <summary>
	/// This fires after an item is removed from the collection no matter how it is removed.
	/// </summary>
	/// <param name="item"></param>
	/// <remarks>Triggered by SetItem, RemoveItem, and ClearItems.</remarks>
	protected virtual void OnItemAdded(T item)
	{
		ItemAdded?.Invoke(this, new ItemEventArgs<T>(item));

		if (m_ListeningToItemEvents)
		{
			if (item is INotifyPropertyChangedWeak notifyPropertyChangedWeak)
				(notifyPropertyChangedWeak).AddHandler(m_ItemPropertyChanged!);
			else if (item is INotifyPropertyChanged notifyPropertyChanged)
				(notifyPropertyChanged).PropertyChanged += OnItemPropertyChanged;
		}
	}

	/// <summary>
	/// This fires after an item is removed from the collection no matter how it is removed.
	/// </summary>
	/// <param name="item"></param>
	/// <remarks>Triggered by SetItem, RemoveItem, and ClearItems.</remarks>
	protected virtual void OnItemRemoved(T item)
	{
		ItemRemoved?.Invoke(this, new ItemEventArgs<T>(item));

		if (m_ListeningToItemEvents)
		{
			if (item is INotifyPropertyChangedWeak notifyPropertyChangedWeak)
				(notifyPropertyChangedWeak).RemoveHandler(m_ItemPropertyChanged!);
			else if (item is INotifyPropertyChanged notifyPropertyChanged)
				(notifyPropertyChanged).PropertyChanged -= OnItemPropertyChanged;
		}
	}

	/// <summary>
	/// Raises the <see cref="ObservableCollection{T}.PropertyChanged" /> event with the provided arguments.
	/// </summary>
	/// <param name="propertyName">
	/// Property that is being changed.
	/// </param>
	protected void OnPropertyChanged(string propertyName)
	{
		OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
	}

	/// <summary>
	/// Removes the element at the specified index of the <see cref="Collection{T}" />.
	/// </summary>
	/// <param name="index">
	/// The zero-based index of the element to remove.
	/// </param>
	/// <exception cref="ArgumentOutOfRangeException">
	/// <paramref name="index" /> is less than zero -or-<paramref name="index" /> is equal to or greater than <see cref="Collection{T}.Count" />.
	/// </exception>
	/// <remarks>Do NOT invoke this method directly. This may be overridden to provide additional validation before an item is removed to the collection.</remarks>
	protected override void RemoveItem(int index)
	{
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index), index, $"{nameof(index)} must be >= 0");
		if (index >= Count)
			throw new ArgumentOutOfRangeException(nameof(index), index, $"{nameof(index)} must be < Count");

		T temp = base[index];
		base.RemoveItem(index);
		OnItemRemoved(temp);
	}

	/// <summary>
	/// Replaces the element at the specified index.
	/// </summary>
	/// <param name="index">The zero-based index of the element to replace.</param>
	/// <param name="item">The new value for the element at the specified index.</param>
	/// <remarks>Do NOT invoke this method directly. This may be overridden to provide additional validation before an item is added to the collection.</remarks>
	protected override void SetItem(int index, T item)
	{
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index), index, $"{nameof(index)} must be >= 0");
		if (index >= Count)
			throw new ArgumentOutOfRangeException(nameof(index), index, $"{nameof(index)} must be < Count");

		T temp = base[index];
		base.SetItem(index, item);
		OnItemRemoved(temp);
		OnItemAdded(item);
	}

	/// <summary>
	/// This enables the ItemPropertyChanged events.
	/// </summary>
	void ListenToEvents()
	{
		if (m_ListeningToItemEvents)
			return;

		m_ItemPropertyChanged = new Listener<PropertyChangedEventArgs>(OnItemPropertyChanged);
		foreach (var item in this.OfType<INotifyPropertyChanged>())
		{
			if (item is INotifyPropertyChangedWeak notifyPropertyChangedWeak)
				(notifyPropertyChangedWeak).AddHandler(m_ItemPropertyChanged);
			else
				item.PropertyChanged += OnItemPropertyChanged;
		}

		m_ListeningToItemEvents = true;
	}

	void OnItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		m_ItemPropertyChangedEvent?.Invoke(this, RelayedEventArgs.Create(sender, e));
	}
}
