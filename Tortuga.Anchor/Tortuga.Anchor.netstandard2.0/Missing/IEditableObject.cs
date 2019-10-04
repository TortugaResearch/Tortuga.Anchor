namespace Tortuga.Anchor
{
    /// <summary>
    /// Provides functionality to commit or rollback changes to an object that is used
    /// as a data source.
    /// </summary>
    public interface IEditableObject
    {
        /// <summary>
        /// Begins an edit on an object.
        /// </summary>
        void BeginEdit();

        /// <summary>
        /// Discards changes since the last System.ComponentModel.IEditableObject.BeginEdit
        /// call.
        /// </summary>
        void CancelEdit();

        /// <summary>
        /// Pushes changes since the last System.ComponentModel.IEditableObject.BeginEdit
        /// or System.ComponentModel.IBindingList.AddNew call into the underlying object.
        /// </summary>
        void EndEdit();
    }
}
