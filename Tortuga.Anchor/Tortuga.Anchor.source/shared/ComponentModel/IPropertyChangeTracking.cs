using System.Collections.Generic;
using System.ComponentModel;

namespace Tortuga.Anchor.ComponentModel
{
    /// <summary>
    /// This interface is used to determine which properties have changed. 
    /// </summary>
    public interface IPropertyChangeTracking : IChangeTracking
    {
        /// <summary>
        /// List of changed properties.
        /// </summary>
        IReadOnlyList<string> ChangedProperties();
    }
}
