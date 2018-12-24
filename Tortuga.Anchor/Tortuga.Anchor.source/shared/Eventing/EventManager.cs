using System;
using Tortuga.Anchor.Collections;

namespace Tortuga.Anchor.Eventing
{
    /// <summary>
    /// This is used to attach weak event handlers to the indicated source.
    /// </summary>
    /// <typeparam name="T">The type of event being listened to.</typeparam>
    public abstract class EventManager<T> where T : EventArgs
    {
        private readonly WeakReferenceCollection<IListener<T>> m_Targets = new WeakReferenceCollection<IListener<T>>();
        private bool m_Listening;

        /// <summary>
        /// Adds a weak event handler.
        /// </summary>
        /// <param name="eventHandler"></param>
        public void AddHandler(IListener<T> eventHandler)
        {
            if (eventHandler == null)
                throw new ArgumentNullException(nameof(eventHandler), $"{nameof(eventHandler)} is null.");

            m_Targets.Add(eventHandler);
            if (!m_Listening)
            {
                AttachToEvent();
                m_Listening = true;
            }
        }

        /// <summary>
        /// Removes a weak event handler.
        /// </summary>
        /// <param name="eventHandler"></param>
        public void RemoveHandler(IListener<T> eventHandler)
        {
            if (eventHandler == null)
                throw new ArgumentNullException(nameof(eventHandler), $"{nameof(eventHandler)} is null.");

            m_Targets.Remove(eventHandler);
            m_Targets.CleanUp();

            if (m_Listening && m_Targets.Count == 0)
            {
                m_Listening = false;
                DetachFromEvent();
            }
        }

        /// <summary>
        /// The implementation of this event must attach the event source to the EventFired method.
        /// </summary>
        protected abstract void AttachToEvent();

        /// <summary>
        /// The implementation of this event must detach the event source to the EventFired method.
        /// </summary>
        protected abstract void DetachFromEvent();

        /// <summary>
        /// This dispatches the event to all of the listeners
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void EventFired(object sender, T e)
        {
            m_Targets.CleanUp();

            //disconnect if no longer needed
            if (m_Targets.Count == 0)
            {
                m_Listening = false;
                DetachFromEvent();
            }
            else
            {
                foreach (var target in m_Targets)
                    target.Invoke(sender, e);
            }
        }
    }
}