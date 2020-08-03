using System;
using System.ComponentModel;

namespace Tortuga.Anchor.Modeling.Internals
{
    /// <summary>
    /// Property bag with two-level change tracking capabilities.
    /// </summary>
    public class EditableObjectPropertyBag : ChangeTrackingPropertyBag
    {
        readonly object?[] m_CheckpointValues;
        bool m_OldIsChangedLocal;

        /// <summary>
        /// Property bag with two-level change tracking capabilities.
        /// </summary>
        /// <param name="owner">Owning model, used to fetch metadata</param>

        public EditableObjectPropertyBag(object owner)
            : base(owner)
        {
            var count = Metadata.Properties.Count;
            m_CheckpointValues = new object?[count];
            for (var i = 0; i < count; i++)
            {
                m_CheckpointValues[i] = NotSet.Value;
            }
        }

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
        public void BeginEdit()
        {
            if (IsEditing)
                return;

            if (BlockReentrant)
                throw new InvalidOperationException("Reentrant call to BeginEdit detected");

            m_OldIsChangedLocal = IsChangedLocal;
            IsEditing = true;
            BlockReentrant = true;

            for (int i = 0; i < Values.Length; i++)
            {
                var currentValue = Values[i];
                m_CheckpointValues[i] = currentValue;
                if (currentValue is IEditableObject eo)
                    eo.BeginEdit();
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
        public void CancelEdit()
        {
            if (!IsEditing)
                return;

            if (BlockReentrant)
                throw new InvalidOperationException("Reentrant call to CancelEdit detected");

            var count = Metadata.Properties.Count;

            IsChangedLocal = m_OldIsChangedLocal;
            IsEditing = false;
            BlockReentrant = true;

            ////remove properties that no longer exist
            //foreach (var item in Values.ToList())
            //{
            //    if (!m_CheckpointValues.ContainsKey(item.Key))
            //    {
            //        var property = Metadata.Properties[item.Key];
            //        OnPropertyChanging(property);
            //        Values.Remove(item.Key);
            //        OnPropertyChanged(property);
            //        OnRevalidateProperty(property);
            //    }
            //}

            ////update remaining properties

            for (int i = 0; i < count; i++)
            {
                var checkpointValue = m_CheckpointValues[i];
                var currentValue = Values[i];
                if (!Equals(currentValue, checkpointValue))
                {
                    var property = Metadata.Properties[i];
                    OnPropertyChanging(property);
                    Values[i] = checkpointValue;
                    OnPropertyChanged(property);
                    OnRevalidateProperty(property);
                }
            }

            //recursively call CancelEdit
            for (int i = 0; i < count; i++)
                if (m_CheckpointValues[i] is IEditableObject eo)
                    eo.CancelEdit();

            OnRevalidateObject();

            //Clear the checkpoint
            for (var i = 0; i < count; i++)
                m_CheckpointValues[i] = NotSet.Value;

            UpdateIsChangedLocal();

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
        public void EndEdit()
        {
            if (!IsEditing)
                return;

            if (BlockReentrant)
                throw new InvalidOperationException("Reentrant call to EndEdit detected");

            IsEditing = false;
            BlockReentrant = true;

            var count = Metadata.Properties.Count;

            //recursively call EndEdit
            for (int i = 0; i < count; i++)
                if (m_CheckpointValues[i] is IEditableObject eo)
                    eo.EndEdit();

            //Clear the checkpoint
            for (var i = 0; i < count; i++)
                m_CheckpointValues[i] = NotSet.Value;

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
