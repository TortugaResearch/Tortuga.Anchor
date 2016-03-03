using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Tortuga.Anchor.ComponentModel;
using Tortuga.Anchor.Eventing;
using Tortuga.Anchor.Modeling.Internals;

namespace Tortuga.Anchor.Modeling
{

    /// <summary>
    /// A collection that supports revertible change tracking.
    /// </summary>
    /// <typeparam name="TModelType"></typeparam>
	[DataContract(Namespace = "http://github.com/docevaad/Anchor")]
    public class ChangeTrackingModelCollection<TModelType> : AbstractModelCollection<TModelType, ChangeTrackingPropertyBag>, IRevertibleChangeTracking
    {
        readonly List<TModelType> m_OriginalList = new List<TModelType>();
        bool m_AllowIsChangedEvents;

        void ConstructorProcessing()
        {
            m_OriginalList.AddRange(this);

            ItemPropertyChanged += ChangeTrackingModelCollection_OnItemPropertyChanged;
            ItemAdded += ChangeTrackingModelCollection_ItemAdded;
            ItemRemoved += ChangeTrackingModelCollection_ItemRemoved;
            m_AllowIsChangedEvents = true;
        }

        void ChangeTrackingModelCollection_ItemRemoved(object sender, ItemEventArgs<TModelType> e)
        {
            if (m_AllowIsChangedEvents)
                Properties.IsChangedLocal = true;
        }

        void ChangeTrackingModelCollection_ItemAdded(object sender, ItemEventArgs<TModelType> e)
        {
            if (m_AllowIsChangedEvents)
                Properties.IsChangedLocal = true;
        }

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
        /// Returns True if any fields were modified since the last call to Checkpoint. This also checks items that implement IChangeTracking.
        /// </summary>
        /// <returns>true if the object’s content has changed since the last call to <see cref="M:System.ComponentModel.IChangeTracking.AcceptChanges" />; otherwise, false.</returns>
        public bool IsChangedLocal
        {
            get { return Properties.IsChangedLocal; }

        }

        /// <summary>
        /// Walk the object graph, looking for changed items in properties and the collection.
        /// </summary>
        /// <returns></returns>
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
                    if (item is IChangeTracking && ((IChangeTracking)item).IsChanged)
                        return true;

                return false;
            }
        }

        /// <summary>
        /// Marks all fields as unchanged and clears the IsChanged flag.
        /// </summary>
        /// <remarks>
        /// This will call AcceptChanges on properties and collection items that implement IChangeTracking
        /// </remarks>
        public void AcceptChanges()
        {
            foreach (var item in this)
            {
                if (item is IChangeTracking)
                    ((IChangeTracking)item).AcceptChanges();
            }

            m_OriginalList.Clear();
            m_OriginalList.AddRange(this);

            Properties.AcceptChanges(true);
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
            {
                if (item is IRevertibleChangeTracking)
                    ((IRevertibleChangeTracking)item).RejectChanges();
            }

            m_AllowIsChangedEvents = true;

            Properties.RejectChanges(true);
        }

        void ChangeTrackingModelCollection_OnItemPropertyChanged(object sender, RelayedEventArgs<PropertyChangedEventArgs> e)
        {
            if (string.IsNullOrEmpty(e.EventArgs.PropertyName) || e.EventArgs.PropertyName == CommonProperties.IsChangedProperty.PropertyName)
                OnPropertyChanged(CommonProperties.IsChangedProperty);
        }

    }
}
