using System;
using System.ComponentModel;
using System.Linq;

namespace Tortuga.Dragnet
{
    /// <summary>
    /// Assertions for INotifyPropertyChanged
    /// </summary>
    public class PropertyChangedEventTest : EventTest<PropertyChangedEventArgs>
    {
        readonly INotifyPropertyChanged m_Source;

        /// <summary>
        /// Creates a new INotifyPropertyChanged test
        /// </summary>
        /// <param name="source"></param>
        public PropertyChangedEventTest(Verify verify, INotifyPropertyChanged source)
            : base(verify, source)
        {
            if (source == null)
                throw new ArgumentNullException("source", "source is null.");

            m_Source = source;
            m_Source.PropertyChanged += SourceEventFired;
        }

        /// <summary>
        /// Asserts that an event is in the queue and that it has the indicated properties.
        /// </summary>
        /// <param name="propertyName">Expected property name</param>
        /// <returns>This will remove the event from the queue.</returns>
        public new EventPair<PropertyChangedEventArgs> ExpectEvent(string propertyName)
        {
            var nextEvent = ExpectEvent();
            Verify.AreEqual(propertyName, nextEvent.EventArgs.PropertyName, "The wrong property name was indicated.");
            return nextEvent;
        }

        /// <summary>
        /// Asserts that the indicated property change events were fired, but in no particular order.
        /// </summary>
        /// <param name="propertyNames"></param>
        public void ExpectUnordered(params string[] propertyNames)
        {
            var workingList = propertyNames.ToList();
            while (workingList.Count > 0 && Count > 0)
            {
                var nextEvent = ExpectEvent();
                if (workingList.Contains(nextEvent.EventArgs.PropertyName))
                    workingList.Remove(nextEvent.EventArgs.PropertyName);
                else
                    Verify.Fail("The property named " + nextEvent.EventArgs.PropertyName + " was not expected.");
            }

            if (workingList.Count > 0 && Count == 0)
                Verify.Fail("Didn't find expected property names " + string.Join(", ", workingList));

        }

    }
}
