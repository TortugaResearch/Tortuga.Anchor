namespace Tortuga.Anchor
{
    /// <summary>
    /// Defines the mechanism for querying the object for changes and resetting of the
    /// changed status.</summary>
    public interface IChangeTracking
    {
        //
        // Summary:
        //     Gets the object's changed status.
        //
        // Returns:
        //     true if the object’s content has changed since the last call to System.ComponentModel.IChangeTracking.AcceptChanges;
        //     otherwise, false.


        /// <summary>
        /// Gets the object's changed status.
        /// </summary>
        /// <value>
        /// true if the object’s content has changed since the last call to System.ComponentModel.IChangeTracking.AcceptChanges;
        /// otherwise, false.
        /// </value>
        bool IsChanged { get; }


        /// <summary>
        /// Resets the object’s state to unchanged by accepting the modifications.
        /// </summary>
        void AcceptChanges();
    }
}
