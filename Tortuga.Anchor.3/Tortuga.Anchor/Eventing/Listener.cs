using System;

namespace Tortuga.Anchor.Eventing
{
    /// <summary>
    /// The object that is referenced by the eventHandler must keep a reference to this class.
    /// Implements the <see cref="IListener{T}" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="IListener{T}" />
    public class Listener<T> : IListener<T> where T : EventArgs
    {
        readonly Action<object, T> m_EventHandler;

        /// <summary>
        /// The object that is referenced by the eventHandler must keep a reference to this class.
        /// </summary>
        /// <param name="eventHandler">The event handler.</param>
        /// <exception cref="ArgumentNullException">eventHandler</exception>

        public Listener(Action<object, T> eventHandler)
        {
            m_EventHandler = eventHandler ?? throw new ArgumentNullException(nameof(eventHandler), $"{nameof(eventHandler)} is null.");
        }

        /// <summary>
        /// Invokes the associated delegate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Invoke(object sender, T e)
        {
            m_EventHandler(sender, e);
        }
    }
}
