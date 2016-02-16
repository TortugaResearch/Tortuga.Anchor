using System;

namespace Tortuga.Anchor.Eventing
{

    /// <summary>
    /// A relayed event wraps a sender/event args pair so that it can be forwarded by another class.
    /// </summary>

    public class RelayedEventArgs<T> : EventArgs where T : EventArgs
    {
        readonly T m_EventArgs;
        readonly object m_OriginalSender;

        /// <summary>
        /// Create a new relayed event from an existing event
        /// </summary>
        public RelayedEventArgs(object originalSender, T eventArgs)
        {
            m_EventArgs = eventArgs;
            m_OriginalSender = originalSender;
        }

        /// <summary>
        /// The wrapped event args
        /// </summary>
        public T EventArgs
        {
            get { return m_EventArgs; }
        }

        /// <summary>
        /// The object that raised the original event
        /// </summary>
        public object OriginalSender
        {
            get { return m_OriginalSender; }
        }

    }

}
