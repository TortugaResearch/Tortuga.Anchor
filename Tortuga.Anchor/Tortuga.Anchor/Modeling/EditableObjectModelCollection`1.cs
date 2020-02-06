using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using Tortuga.Anchor.ComponentModel;
using Tortuga.Anchor.Eventing;
using Tortuga.Anchor.Modeling.Internals;

namespace Tortuga.Anchor.Modeling
{
    /// <summary>
    /// A collection suitable for use in dialogs that have a cancel button. This also supports revertible change tracking.
    /// </summary>
    /// <typeparam name="TModelType"></typeparam>
	[DataContract(Namespace = "http://github.com/docevaad/Anchor")]
    public class EditableObjectModelCollection<TModelType> : AbstractModelCollection<TModelType, EditableObjectPropertyBag>, IRevertibleChangeTracking, IEditableObject
    {
        readonly List<TModelType> m_CheckpointItems = new List<TModelType>();
        readonly List<TModelType> m_OriginalList = new List<TModelType>();
        bool m_AllowIsChangedEvents;
        bool m_CollectionChanged;

        /// <summary>
        /// Creates a model by auto-constructing the property bag defined by TPropertyTracking.
        /// </summary>
        /// <remarks>
        /// Requires TPropertyTracking have a public constructor that accepts an Object
        /// </remarks>
        protected EditableObjectModelCollection()
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
        protected EditableObjectModelCollection(List<TModelType> list)
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
        protected EditableObjectModelCollection(IEnumerable<TModelType> collection)
            : base(collection)
        {
            ConstructorProcessing();
        }

        /// <summary>
        /// Returns True if any fields were modified since the last call to AcceptChanges. This does walks the object graph for changes in child objects as well.
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
                    if (item is IChangeTracking changeTracking && changeTracking.IsChanged)
                        return true;

                return false;
            }
        }

        /// <summary>
        /// Returns True if any fields were modified since the last call to AcceptChanges. This does not walk the object graph.
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
            Properties.AcceptChanges(true);
            foreach (var item in this)
            {
                if (item is IChangeTracking changeTracking)
                    changeTracking.AcceptChanges();
            }

            m_OriginalList.Clear();
            m_OriginalList.AddRange(this);
            m_CheckpointItems.Clear();
            UpdateCollectionChanged();
        }

        /// <summary>
        /// Begins an edit on an object.
        /// </summary>
        public virtual void BeginEdit()
        {
            if (Properties.IsEditing)
                return;

            Properties.BeginEdit();

            foreach (var item in this)
                m_CheckpointItems.Add(item);

            foreach (var item in this.OfType<IEditableObject>())
                item.BeginEdit();
        }

        /// <summary>
        /// Discards changes since the last <see cref="System.ComponentModel.IEditableObject.BeginEdit" /> call.
        /// </summary>
        public virtual void CancelEdit()
        {
            Properties.CancelEdit();
            Clear();
            AddRange(m_CheckpointItems);
            m_CheckpointItems.Clear();

            foreach (var item in this.OfType<IEditableObject>())
                item.CancelEdit();
        }

        /// <summary>
        /// Pushes changes since the last <see cref="System.ComponentModel.IEditableObject.BeginEdit" /> or <see cref="System.ComponentModel.IBindingList.AddNew" /> call into the underlying object.
        /// </summary>
        public virtual void EndEdit()
        {
            Properties.EndEdit();
            m_CheckpointItems.Clear();

            foreach (var item in this.OfType<IEditableObject>())
                item.EndEdit();
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

            Properties.RejectChanges(true);
            foreach (var item in this)
            {
                if (item is IRevertibleChangeTracking revertibleChangeTracking)
                    revertibleChangeTracking.RejectChanges();
            }

            m_AllowIsChangedEvents = true;
            UpdateCollectionChanged();
        }

        void ChangeTrackingModelCollection_OnItemPropertyChanged(object sender, RelayedEventArgs<PropertyChangedEventArgs> e)
        {
            if (e.EventArgs.PropertyName == CommonProperties.IsChangedProperty.PropertyName)
                OnPropertyChanged(CommonProperties.IsChangedProperty);
        }

        void ConstructorProcessing()
        {
            m_OriginalList.AddRange(this);

            ItemPropertyChanged += ChangeTrackingModelCollection_OnItemPropertyChanged;
            ItemAdded += EditableObjectEntityCollection_ItemAdded;
            ItemRemoved += EditableObjectEntityCollection_ItemRemoved;
            m_AllowIsChangedEvents = true;
        }

        void EditableObjectEntityCollection_ItemAdded(object sender, ItemEventArgs<TModelType> e)
        {
            if (m_AllowIsChangedEvents)
                UpdateCollectionChanged();
        }

        void EditableObjectEntityCollection_ItemRemoved(object sender, ItemEventArgs<TModelType> e)
        {
            if (m_AllowIsChangedEvents)
                UpdateCollectionChanged();
        }

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

        [OnDeserialized]
#pragma warning disable RCS1163 // Unused parameter.
        void OnDeserialized(StreamingContext context) => AcceptChanges();

#pragma warning restore RCS1163 // Unused parameter.
    }
}
