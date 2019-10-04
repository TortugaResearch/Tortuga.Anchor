using System;

namespace Tortuga.Anchor
{


    /// <summary>
    /// Initializes a new instance of the <see cref="NotMappedAttribute"/> class.
    /// </summary>
    /// <seealso cref="Attribute" />
    [AttributeUsage(AttributeTargets.Property)]
    internal sealed class NotMappedAttribute : Attribute
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="NotMappedAttribute"/> class.
        /// </summary>
        public NotMappedAttribute()
        {

        }


    }
}
