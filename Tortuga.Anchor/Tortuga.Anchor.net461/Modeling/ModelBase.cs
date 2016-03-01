using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using Tortuga.Anchor.DataAnnotations;
using Tortuga.Anchor.Eventing;
using Tortuga.Anchor.Modeling.Internals;

namespace Tortuga.Anchor.Modeling
{



    /// <summary>
    /// Abstract base class to deal with the limitations of generics
    /// </summary>
    [DataContract(Namespace = "http://github.com/docevaad/Anchor")]
    public abstract partial class ModelBase : INotifyPropertyChanged, INotifyPropertyChangedWeak
    {
        PropertyChangedEventManager m_PropertyChangedEventManager;
        ErrorsDictionary m_Errors = new ErrorsDictionary();

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelBase"/> class.
        /// </summary>

        protected ModelBase()
        {
            Initialize();
        }

        void Initialize()
        {
            m_PropertyChangedEventManager = new PropertyChangedEventManager(this);
            m_Errors = new ErrorsDictionary();
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode"), SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "context")]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [OnDeserializing]
        void _ModelBase_OnDeserializing(StreamingContext context)
        {
            Initialize();
        }

        /// <summary>
        /// Raised when the errors collection has changed.
        /// </summary>
        /// <remarks>
        /// This may be fired even when no actual change has occurred.
        /// </remarks>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Adds a weak event handler.
        /// </summary>
        /// <param name="eventHandler"></param>
        /// <exception cref="ArgumentNullException">eventHandler;eventHandler is null.</exception>
        public void AddHandler(IListener<PropertyChangedEventArgs> eventHandler)
        {
            if (eventHandler == null)
                throw new ArgumentNullException("eventHandler", "eventHandler is null.");

            m_PropertyChangedEventManager.AddHandler(eventHandler);
        }

        /// <summary>
        /// Removes a weak event handler.
        /// </summary>
        /// <param name="eventHandler"></param>
        /// <exception cref="ArgumentNullException">eventHandler;eventHandler is null.</exception>
        public void RemoveHandler(IListener<PropertyChangedEventArgs> eventHandler)
        {
            if (eventHandler == null)
                throw new ArgumentNullException("eventHandler", "eventHandler is null.");

            m_PropertyChangedEventManager.RemoveHandler(eventHandler);
        }

        /// <summary>
        /// Triggers the PropertyChanged event.
        /// </summary>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        internal void InvokePropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

        /// <summary>
        /// Override this method to add imperative validation at the object level.
        /// </summary>
        /// <param name="results">A collection of the declarative validation errors. You may add and remove errors from this collection.</param>

        protected virtual void OnValidateObject(ValidationResultCollection results) { }

        /// <summary>
        /// Override this method to add imperative validation at the property level.
        /// </summary>
        /// <param name="propertyName">The name of the property being validated.</param>
        /// <param name="results">A collection of the declarative validation errors. You may add and remove errors from this collection.</param>

        protected virtual void OnValidateProperty(string propertyName, ValidationResultCollection results) { }


        internal ErrorsDictionary ErrorsDictionary
        {
            get { return m_Errors; }
        }

        /// <summary>
        /// Invoke this method to signal the events associated with changing the errors dictionary. The parameter updateType is returned by the methods on ErrorsDictionary.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="updateType">Type of the update.</param>
        internal void OnErrorsChanged(string propertyName, ErrorsDictionaryUpdateType updateType)
        {
            if (updateType == ErrorsDictionaryUpdateType.NoChange)
                return;

            OnErrorsChanged(propertyName);

            if (updateType == ErrorsDictionaryUpdateType.HasErrorsIsFalse || updateType == ErrorsDictionaryUpdateType.HasErrorsIsTrue)
                InvokePropertyChanged(CommonProperties.HasErrorsProperty);
        }

        /// <summary>
        /// Used to manually invoke the ErrorsChanged event.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void OnErrorsChanged(string propertyName)
        {
            if (ErrorsChanged != null)
                ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
        }
    }
}
