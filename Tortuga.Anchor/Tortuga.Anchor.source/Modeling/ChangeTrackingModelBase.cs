
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using Tortuga.Anchor.ComponentModel;
using Tortuga.Anchor.Modeling.Internals;

#if !DataAnnotations_Missing
using System.ComponentModel.DataAnnotations.Schema;
#endif

namespace Tortuga.Anchor.Modeling
{
    /// <summary>
    /// This ModelBase tracks which fields have changed since the last time AcceptChanges or RejectChanges was called. The purpose of this ModelBase is to easy to determine which objects have unsaved changes.
    /// </summary>
    [DataContract(Namespace = "http://github.com/docevaad/Anchor")]
    public class ChangeTrackingModelBase : AbstractModelBase<ChangeTrackingPropertyBag>, IDetailedPropertyChangeTracking
    {

        /// <summary>
        /// This ModelBase tracks which fields have changed since the last time Checkpoint or Revert was called.
        /// </summary>

        public ChangeTrackingModelBase() { }

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


        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "context"), SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
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

            if (result == NotSet.Value) //use reflection to get the default value
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
