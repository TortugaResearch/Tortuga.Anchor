using System;

namespace Tortuga.Anchor.Eventing
{
    /// <summary>
    /// A relayed event wraps a sender/event args pair so that it can be forwarded by another class.
    /// </summary>

    public class RelayedEventArgs<T> : EventArgs where T : EventArgs
    {
        /// <summary>
        /// Create a new relayed event from an existing event
        /// </summary>
        public RelayedEventArgs(object originalSender, T eventArgs)
        {
            EventArgs = eventArgs ?? throw new ArgumentNullException(nameof(eventArgs), $"{nameof(eventArgs)} is null.");
            OriginalSender = originalSender;
        }

        /// <summary>
        /// The wrapped event args
        /// </summary>
        public T EventArgs { get; private set; }

        /// <summary>
        /// The object that raised the original event
        /// </summary>
        public object OriginalSender { get; private set; }
    }
}