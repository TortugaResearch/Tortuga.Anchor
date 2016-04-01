using System.Diagnostics.CodeAnalysis;

namespace Tortuga.Anchor.Modeling.Internals
{
    /// <summary>
    /// Indicates a property was never set;
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1052:StaticHolderTypesShouldBeSealed")]
    public class NotSet
    {
        private NotSet() { }

        /// <summary>
        /// Indicates a property was never set;
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public static readonly NotSet Value = new NotSet();
    }
}
