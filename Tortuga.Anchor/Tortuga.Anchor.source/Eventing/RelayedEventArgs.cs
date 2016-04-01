using System;
using System.Diagnostics.CodeAnalysis;

namespace Tortuga.Anchor.Eventing
{
    /// <summary>
    /// A relayed event wraps a sender/event args pair so that it can be forwarded by another class.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]

    public static class RelayedEventArgs
    {
        /// <summary>
        /// Create a new relayed event from an existing event
        /// </summary>
        public static RelayedEventArgs<T> Create<T>(object originalSender, T eventArgs) where T : EventArgs
        {
            if (eventArgs == null)
                throw new ArgumentNullException("eventArgs", "eventArgs is null.");


            return new RelayedEventArgs<T>(originalSender, eventArgs);
        }
    }
}
