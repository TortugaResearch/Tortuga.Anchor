namespace Tortuga.Anchor
{
    /// <summary>
    /// Provides support for rolling back the changes
    /// </summary>
    /// <seealso cref="IChangeTracking" />
    public interface IRevertibleChangeTracking : IChangeTracking
    {

        /// <summary>
        /// Resets the object’s state to unchanged by rejecting the modifications.            
        /// </summary>
        void RejectChanges();
    }
}
