using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using Tortuga.Anchor.ComponentModel;
using Tortuga.Anchor.Eventing;
using Tortuga.Anchor.Modeling.Internals;

namespace Tortuga.Anchor.Modeling;

/// <summary>
/// A collection that supports revertible change tracking.
/// </summary>
/// <typeparam name="TModelType"></typeparam>
[DataContract(Namespace = "http://github.com/docevaad/Anchor")]
public class ChangeTrackingModelCollection<TModelType> : AbstractModelCollection<TModelType, ChangeTrackingPropertyBag>, IRevertibleChangeTracking
{
	readonly List<TModelType> m_OriginalList = [];
	bool m_AllowIsChangedEvents;
	bool m_CollectionChanged;

	/// <summary>
	/// Creates a model by auto-constructing the property bag defined by TPropertyTracking.
	/// </summary>
	/// <remarks>
	/// Requires TPropertyTracking have a public constructor that accepts an Object
	/// </remarks>
	protected ChangeTrackingModelCollection()
	{
		ConstructorProcessing();
	}

	/// <summary>
	/// Creates a model by auto-constructing the property bag defined by TPropertyTracking and populates it using the supplied collection
	/// </summary>
	/// <param name="list">The list from which the elements are copied.</param>
	/// <remarks>
	/// Requires TPropertyTracking have a public constructor that accepts an Object
	/// </remarks>
	protected ChangeTrackingModelCollection(List<TModelType> list)
		: base(list)
	{
		ConstructorProcessing();
	}

	/// <summary>
	/// Creates a model by auto-constructing the property bag defined by TPropertyTracking and populates it using the supplied collection
	/// </summary>
	/// <param name="collection">The collection from which the elements are copied.</param>
	/// <remarks>
	/// Requires TPropertyTracking have a public constructor that accepts an Object
	/// </remarks>
	protected ChangeTrackingModelCollection(IEnumerable<TModelType> collection)
		: base(collection)
	{
		ConstructorProcessing();
	}

	/// <summary>
	/// Walk the object graph, looking for changed items in properties and the collection.
	/// </summary>
	/// <returns></returns>
	[NotMapped]
	public bool IsChanged
	{
		get
		{
			if (IsChangedLocal)
				return true;

			//deep scan of properties
			if (Properties.IsChangedGraph())
				return true;

			//deep scan of child objects
			foreach (var item in this)
				if (item is IChangeTracking changeTracking && (changeTracking).IsChanged)
					return true;

			return false;
		}
	}

	/// <summary>
	/// Returns True if any fields were modified since the last call to Checkpoint. This also checks items that implement IChangeTracking.
	/// </summary>
	/// <returns>true if the object’s content has changed since the last call to <see cref="System.ComponentModel.IChangeTracking.AcceptChanges" />; otherwise, false.</returns>
	[NotMapped]
	public bool IsChangedLocal => Properties.IsChangedLocal || m_CollectionChanged;

	/// <summary>
	/// Marks all fields as unchanged and clears the IsChanged flag.
	/// </summary>
	/// <remarks>
	/// This will call AcceptChanges on properties and collection items that implement IChangeTracking
	/// </remarks>
	public void AcceptChanges()
	{
		foreach (var item in this)
			if (item is IChangeTracking changeTracking)
				(changeTracking).AcceptChanges();

		m_OriginalList.Clear();
		m_OriginalList.AddRange(this);

		Properties.AcceptChanges(true);
		UpdateCollectionChanged();
	}

	/// <summary>
	/// Discards all pending changes and reverts to the values used the last time AcceptChanges was called.
	/// </summary>
	/// <remarks>
	/// This will call RejectChanges on properties and collection items from the original collection that implement IRevertibleChangeTracking
	/// </remarks>
	public void RejectChanges()
	{
		m_AllowIsChangedEvents = false;

		Clear();
		foreach (var item in m_OriginalList)
			Add(item);

		foreach (var item in this)
			if (item is IRevertibleChangeTracking revertibleChangeTracking)
				(revertibleChangeTracking).RejectChanges();

		m_AllowIsChangedEvents = true;

		Properties.RejectChanges(true);
		UpdateCollectionChanged();
	}

	void ChangeTrackingModelCollection_ItemAdded(object? sender, ItemEventArgs<TModelType> e)
	{
		if (m_AllowIsChangedEvents)
			UpdateCollectionChanged();
	}

	void ChangeTrackingModelCollection_ItemRemoved(object? sender, ItemEventArgs<TModelType> e)
	{
		if (m_AllowIsChangedEvents)
			UpdateCollectionChanged();
	}

	void ChangeTrackingModelCollection_OnItemPropertyChanged(object? sender, RelayedEventArgs<PropertyChangedEventArgs> e)
	{
		if (string.IsNullOrEmpty(e.EventArgs.PropertyName) || e.EventArgs.PropertyName == CommonProperties.IsChangedProperty.PropertyName)
			OnPropertyChanged(CommonProperties.IsChangedProperty);
	}

	void ConstructorProcessing()
	{
		m_OriginalList.AddRange(this);

		ItemPropertyChanged += ChangeTrackingModelCollection_OnItemPropertyChanged;
		ItemAdded += ChangeTrackingModelCollection_ItemAdded;
		ItemRemoved += ChangeTrackingModelCollection_ItemRemoved;
		m_AllowIsChangedEvents = true;
	}

	/// <summary>Called after the object is deserialized.</summary>
	/// <param name="context">The context.</param>
	[OnDeserialized]
	void OnDeserialized(StreamingContext context) => AcceptChanges();

	void UpdateCollectionChanged()
	{
		var previousFlag = m_CollectionChanged;
		var newFlag = Count != m_OriginalList.Count;
		if (!newFlag)
		{
			for (int i = 0; i < Count; i++)
				if (!Equals(this[i], m_OriginalList[i]))
				{
					newFlag = true;
					break;
				}
		}

		if (previousFlag != newFlag)
		{
			m_CollectionChanged = newFlag;

			//These may fire unnecessarily depending on the state of Properties.IsChanged, but that will be very rare.
			OnPropertyChanged(CommonProperties.IsChangedLocalProperty);
			OnPropertyChanged(CommonProperties.IsChangedProperty);
		}
	}
}
