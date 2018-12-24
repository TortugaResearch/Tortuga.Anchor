namespace Tortuga.Anchor.Modeling
{
    /// <summary>
    /// This enumeration is used to indicate how AcceptChanges calls will be propagated
    /// </summary>
    public enum UpdateMode
    {
        /// <summary>
        /// Do not mark changes as accepted
        /// </summary>
        None,

        /// <summary>
        /// Recursively mark changes as accepted
        /// </summary>
        AcceptChanges,

        /// <summary>
        /// Mark changes as accepted, but not on child objects
        /// </summary>
        AcceptLocalChanges,
    }
}