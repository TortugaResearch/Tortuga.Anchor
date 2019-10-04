using System;
using System.ComponentModel;

namespace Tortuga.Anchor.Eventing
{
    /// <summary>
    /// This is used to attach weak event handlers to the indicated source.
    /// </summary>
    /// <remarks>
    /// If a class encapsulates this then it should also implement INotifyPropertyChangedWeak.
    /// </remarks>
    public class PropertyChangedEventManager : EventManager<PropertyChangedEventArgs>
    {
        readonly INotifyPropertyChanged m_Source;

        /// <summary>
        /// Creates a new CollectionChangedEventManager.
        /// </summary>
        /// <param name="source"></param>

        public PropertyChangedEventManager(INotifyPropertyChanged source)
        {
            m_Source = source ?? throw new ArgumentNullException(nameof(source), $"{nameof(source)} is null.");
        }

        /// <summary>
        /// The implementation of this event must attach the event source to the EventFired method.
        /// </summary>
        protected override void AttachToEvent()
        {
            m_Source.PropertyChanged += EventFired;
        }

        /// <summary>
        /// The implementation of this event must detach the event source to the EventFired method.
        /// </summary>
        protected override void DetachFromEvent()
        {
            m_Source.PropertyChanged -= EventFired;
        }
    }
}