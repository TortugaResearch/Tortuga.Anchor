using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Tortuga.Anchor.ComponentModel;
using Tortuga.Anchor.Eventing;
using Tortuga.Anchor.Modeling.Internals;

#if !Serialization_Missing
using System.Runtime.Serialization;
#endif

#if !DataAnnotations_Missing
using System.ComponentModel.DataAnnotations.Schema;
#endif

namespace Tortuga.Anchor.Modeling
{

    /// <summary>
    /// A collection suitable for use in dialogs that have a cancel button. This also supports revertible change tracking.
    /// </summary>
    /// <typeparam name="TModelType"></typeparam>
#if !Serialization_Missing
	[DataContract(Namespace = "http://github.com/docevaad/Anchor")]
#endif
    public class EditableObjectModelCollection<TModelType> : AbstractModelCollection<TModelType, EditableObjectPropertyBag>, IRevertibleChangeTracking, IEditableObject
    {
        readonly List<TModelType> m_OriginalList = new List<TModelType>();
        readonly List<TModelType> m_CheckpointItems = new List<TModelType>();
        bool m_AllowIsChangedEvents;

        void ConstructorProcessing()
        {
            m_OriginalList.AddRange(this);

            ItemPropertyChanged += ChangeTrackingModelCollection_OnItemPropertyChanged;
            ItemAdded += EditableObjectEntityCollection_ItemAdded;
            ItemRemoved += EditableObjectEntityCollection_ItemRemoved;
            m_AllowIsChangedEvents = true;
        }


        void EditableObjectEntityCollection_ItemRemoved(object sender, ItemEventArgs<TModelType> e)
        {
            if (m_AllowIsChangedEvents)
                Properties.IsChangedLocal = true;
        }


        void EditableObjectEntityCollection_ItemAdded(object sender, ItemEventArgs<TModelType> e)
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
        /// Returns True if any fields were modified since the last call to AcceptChanges. This does not walk the object graph.
        /// </summary>
        /// <returns>true if the object’s content has changed since the last call to <see cref="M:System.ComponentModel.IChangeTracking.AcceptChanges" />; otherwise, false.</returns>
        [NotMapped]
        public bool IsChangedLocal
        {
            get { return Properties.IsChangedLocal; }
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
            Properties.AcceptChanges(true);
            foreach (var item in this)
            {
                if (item is IChangeTracking)
                    ((IChangeTracking)item).AcceptChanges();
            }

            m_OriginalList.Clear();
            m_OriginalList.AddRange(this);
            m_CheckpointItems.Clear();
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
                if (item is IRevertibleChangeTracking)
                    ((IRevertibleChangeTracking)item).RejectChanges();
            }

            m_AllowIsChangedEvents = true;
        }

        void ChangeTrackingModelCollection_OnItemPropertyChanged(object sender, RelayedEventArgs<PropertyChangedEventArgs> e)
        {
            if (e.EventArgs.PropertyName == CommonProperties.IsChangedProperty.PropertyName)
                OnPropertyChanged(CommonProperties.IsChangedProperty);
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
        /// Discards changes since the last <see cref="M:System.ComponentModel.IEditableObject.BeginEdit" /> call.
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
        /// Pushes changes since the last <see cref="M:System.ComponentModel.IEditableObject.BeginEdit" /> or <see cref="M:System.ComponentModel.IBindingList.AddNew" /> call into the underlying object.
        /// </summary>
        public virtual void EndEdit()
        {
            Properties.EndEdit();
            m_CheckpointItems.Clear();

            foreach (var item in this.OfType<IEditableObject>())
                item.EndEdit();
        }
    }
}
