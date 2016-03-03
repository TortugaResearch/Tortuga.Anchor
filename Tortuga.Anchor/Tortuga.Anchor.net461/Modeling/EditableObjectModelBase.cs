using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.Serialization;
using Tortuga.Anchor.ComponentModel;
using Tortuga.Anchor.Modeling.Internals;

namespace Tortuga.Anchor.Modeling
{
    /// <summary>
    /// This ModelBase adds the IEditableObject interface to the ChangeTrackingModelBase. The IEditableObject methods are designed for dialogs with ok/cancel semantics. Use the IChangeTracking to track unsaved changes.
    /// </summary>
    [DataContract(Namespace = "http://github.com/docevaad/Anchor")]
    public class EditableObjectModelBase : AbstractModelBase<EditableObjectPropertyBag>, IDetailedPropertyChangeTracking, IEditableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EditableObjectModelBase"/> class.
        /// </summary>

        public EditableObjectModelBase() { }

        /// <summary>
        /// Begins an edit on an object.
        /// </summary>
        public virtual void BeginEdit()
        {
            if (Properties.IsEditing)
                return;

            Properties.BeginEdit();
        }

        /// <summary>
        /// Discards changes since the last <see cref="M:System.ComponentModel.IEditableObject.BeginEdit" /> call.
        /// </summary>
        public virtual void CancelEdit()
        {
            if (!Properties.IsEditing)
                return;

            Properties.CancelEdit();
        }

        /// <summary>
        /// Pushes changes since the last <see cref="M:System.ComponentModel.IEditableObject.BeginEdit" /> or <see cref="M:System.ComponentModel.IBindingList.AddNew" /> call into the underlying object.
        /// </summary>
        public virtual void EndEdit()
        {
            if (!Properties.IsEditing)
                return;

            Properties.EndEdit();
        }

        /// <summary>
        /// Returns True if any fields were modified since the last call to AcceptChanges. This does not walk the object graph.
        /// </summary>
        /// <returns>true if the object’s content has changed since the last call to <see cref="M:System.ComponentModel.IChangeTracking.AcceptChanges" />; otherwise, false.</returns>
        public bool IsChangedLocal
        {
            get { return Properties.IsChangedLocal; }
        }

        /// <summary>
        /// Returns True if any fields were modified since the last call to AcceptChanges. This does walks the object graph for changes in child objects as well.
        /// </summary>
        /// <returns></returns>

        public bool IsChanged
        {
            get
            {
                if (IsChangedLocal)
                    return true;
                return Properties.IsChangedGraph();
            }
        }

        /// <summary>
        /// Marks all fields as unchanged and clears the IsChanged flag.
        /// </summary>
        /// <remarks>
        /// This will call AcceptChanges on properties that implement IChangeTracking
        /// </remarks>
        public void AcceptChanges()
        {
            Properties.AcceptChanges(true);
        }

        /// <summary>
        /// Discards all pending changes and reverts to the values used the last time AcceptChanges was called.
        /// </summary>
        /// <remarks>
        /// This will call RejectChanges on properties that implement IRevertibleChangeTracking
        /// </remarks>
        public void RejectChanges()
        {
            Properties.RejectChanges(true);
        }


        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "context")]
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [OnDeserialized]
        void OnDeserialized(StreamingContext context)
        {
            AcceptChanges();
        }

        /// <summary>
        /// Gets the previous value for the indicated property.
        /// </summary>
        /// <param name="propertyName">
        /// Name of the property.
        /// </param>
        public object GetPreviousValue(string propertyName)
        {

            var result = Properties.GetPreviousValue(propertyName);

            if (result == Missing.Value) //use reflection to get the default value
                return Properties.Metadata.Properties[propertyName].InvokeGet(this); //this will throw is the property doesn't exist.
            else
                return result;
        }

        /// <summary>
        /// List of changed properties.
        /// </summary>
        public System.Collections.Generic.IReadOnlyList<string> ChangedProperties()
        {
            return Properties.ChangedProperties();
        }
    }
}
