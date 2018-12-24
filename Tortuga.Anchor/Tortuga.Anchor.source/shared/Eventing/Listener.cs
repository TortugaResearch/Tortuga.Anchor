using System;

namespace Tortuga.Anchor.Eventing
{
    /// <summary>
    /// The object that is referenced by the eventHandler must keep a reference to this class.
    /// </summary>
    public class Listener<T> : IListener<T> where T : EventArgs
    {
        private readonly Action<object, T> m_EventHandler;

        /// <summary>
        /// The object that is referenced by the eventHandler must keep a reference to this class.
        /// </summary>

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