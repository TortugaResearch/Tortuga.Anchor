using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Tortuga.Anchor.Modeling.Internals
{
    /// <summary>
    /// Property bag with two-level change tracking capabilities.
    /// </summary>
    public class EditableObjectPropertyBag : ChangeTrackingPropertyBag
    {
        readonly Dictionary<string, object> m_CheckpointValues = new Dictionary<string, object>(StringComparer.Ordinal);
        bool m_OldIsChangedLocal;

        /// <summary>
        /// Property bag with two-level change tracking capabilities.
        /// </summary>
        /// <param name="owner">Owning model, used to fetch metadata</param>

        public EditableObjectPropertyBag(object owner)
            : base(owner)
        { }

        /// <summary>
        /// Currently editing
        /// </summary>
        /// <value>
        ///   <c>true</c> if the object is in editing mode; otherwise, <c>false</c>.
        /// </value>
        public bool IsEditing { get; private set; }

        /// <summary>
        /// Used to prevent reentrant calls to Begin/End/Cancel Edit
        /// </summary>
        bool BlockReentrant { get; set; }

        /// <summary>
        /// Marks all fields as unchanged by storing them in the original values collection.
        /// </summary>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        /// <remarks>
        /// Calling this ends all pending edits.
        /// </remarks>
        public override void AcceptChanges(bool recursive)
        {
            EndEdit();
            base.AcceptChanges(recursive);
        }

        /// <summary>
        /// This creates a checkpoint using the current values. The checkpoint remains available until EndEdit or CancelEdit is called.
        /// </summary>
        /// <exception cref="InvalidOperationException">Reentrant call to BeginEdit detected</exception>
        /// <remarks>
        /// Calling this method multiple times will have no effect.
        /// </remarks>
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "BeginEdit")]
        public void BeginEdit()
        {
            if (IsEditing)
                return;

            if (BlockReentrant)
                throw new InvalidOperationException("Reentrant call to BeginEdit detected");

            m_OldIsChangedLocal = IsChangedLocal;
            IsEditing = true;
            BlockReentrant = true;

            foreach (var item in Values)
            {
                m_CheckpointValues.Add(item.Key, item.Value);
                if (item.Value is IEditableObject)
                    ((IEditableObject)item.Value).BeginEdit();
            }

            BlockReentrant = false;
        }

        /// <summary>
        /// This reverts all changes to the checkpoint values. CancelEdit is called recursively on any value in the original list.
        /// </summary>
        /// <exception cref="InvalidOperationException">Reentrant call to CancelEdit detected</exception>
        /// <remarks>
        /// Calling this when there are no matching BeginEdit will have no effect.
        /// </remarks>
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "CancelEdit")]
        public void CancelEdit()
        {
            if (!IsEditing)
                return;

            if (BlockReentrant)
                throw new InvalidOperationException("Reentrant call to CancelEdit detected");

            IsChangedLocal = m_OldIsChangedLocal;
            IsEditing = false;
            BlockReentrant = true;

            //remove properties that no longer exist
            foreach (var item in Values.ToList())
            {
                if (!m_CheckpointValues.ContainsKey(item.Key))
                {
                    var property = Metadata.Properties[item.Key];
                    OnPropertyChanging(property);
                    Values.Remove(item.Key);
                    OnPropertyChanged(property);
                    OnRevalidateProperty(property);
                }
            }

            //update remaining properties
            foreach (var item in m_CheckpointValues)
            {
                var oldValue = GetValue(item.Key);
                if (!Equals(oldValue, item.Value))
                {
                    var property = Metadata.Properties[item.Key];
                    OnPropertyChanging(property);
                    Values[item.Key] = item.Value;
                    OnPropertyChanged(property);
                    OnRevalidateProperty(property);
                }
            }

            //recursively call CancelEdit
            foreach (var item in m_CheckpointValues)
            {
                if (item.Value is IEditableObject)
                    ((IEditableObject)item.Value).CancelEdit();
            }

            OnRevalidateObject();
            m_CheckpointValues.Clear();

            BlockReentrant = false;
        }

        /// <summary>
        /// This removed one level of checkpoint values.
        /// EndEdit is called recursively on any value in the list of checkpoint values.
        /// </summary>
        /// <exception cref="InvalidOperationException">Reentrant call to EndEdit detected</exception>
        /// <remarks>
        /// Calling this when there are no matching BeginEdit will have no effect.
        /// </remarks>
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "EndEdit")]
        public void EndEdit()
        {
            if (!IsEditing)
                return;

            if (BlockReentrant)
                throw new InvalidOperationException("Reentrant call to EndEdit detected");

            IsEditing = false;
            BlockReentrant = true;

            foreach (var item in m_CheckpointValues)
            {
                if (item.Value is IEditableObject)
                    ((IEditableObject)item.Value).EndEdit();
            }
            m_CheckpointValues.Clear();

            BlockReentrant = false;
        }

        /// <summary>
        /// Replaces the current values collection with the original values collection.
        /// </summary>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        /// <remarks>
        /// Calling this cancels all pending edits.
        /// </remarks>
        public override void RejectChanges(bool recursive)
        {
            CancelEdit();
            base.RejectChanges(recursive);
        }
    }
}