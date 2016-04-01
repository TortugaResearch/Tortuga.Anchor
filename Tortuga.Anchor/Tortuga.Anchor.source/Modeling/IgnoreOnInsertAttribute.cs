#if !DataAnnotations_Missing
using System;

namespace Tortuga.Anchor.Modeling
{
    /// <summary>
    /// This indicates that the property shouldn't be included in insert operations.
    /// </summary>
    /// <seealso cref="Attribute" />
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class IgnoreOnInsertAttribute : Attribute
    {

    }

}
#endif