using System;

namespace Tortuga.Anchor
{


    /// <summary>
    /// Specifies whether a property or event should be displayed in a Properties window.
    /// </summary>
    /// <seealso cref="Attribute" />
    /// <remarks>This is used to ensure UWP libraries compile. It has no effect on that platform.</remarks>
    [AttributeUsage(AttributeTargets.All)]
    internal sealed class BrowsableAttribute : Attribute
    {
        private readonly bool m_Browsable;

        /// <summary>
        /// Initializes a new instance of the <see cref="BrowsableAttribute"/> class.
        /// </summary>
        /// <param name="browsable">if set to <c>true</c> [browsable].</param>
        public BrowsableAttribute(bool browsable)
        {
            m_Browsable = browsable;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="BrowsableAttribute"/> is browsable1.
        /// </summary>
        /// <value>
        ///   <c>true</c> if browsable1; otherwise, <c>false</c>.
        /// </value>
        public bool Browsable
        {
            get { return m_Browsable; }
        }
    }

        
}
