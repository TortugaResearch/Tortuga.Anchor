using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using Tortuga.Anchor.ComponentModel;
using Tortuga.Anchor.DataAnnotations;
using Tortuga.Anchor.Eventing;

namespace Tortuga.Anchor.Modeling.Internals
{
    /// <summary>
    /// Abstract base class to deal with the limitations of generics. This is not meant to be used directly by client code.
    /// </summary>
    [DataContract(Namespace = "http://github.com/docevaad/Anchor")]
    public abstract partial class AbstractModelBase : INotifyPropertyChanged, INotifyPropertyChangedWeak, IValidatable
    {
        PropertyChangedEventManager m_PropertyChangedEventManager;
        ErrorsDictionary m_Errors = new ErrorsDictionary();

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractModelBase"/> class.
        /// </summary>

        protected AbstractModelBase()
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
                throw new ArgumentNullException(nameof(eventHandler), "eventHandler is null.");

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
                throw new ArgumentNullException(nameof(eventHandler), $"{nameof(eventHandler)} is null.");
            m_PropertyChangedEventManager.RemoveHandler(eventHandler);
        }

        /// <summary>
        /// Triggers the PropertyChanged event.
        /// </summary>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        internal void InvokePropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        /// <summary>
        /// This forces the object to be completely revalidated.
        /// </summary>
        /// <returns>
        /// True if the object has no errors
        /// </returns>
        public abstract bool Validate();

        /// <summary>
        /// Override this method to add imperative validation at the object level.
        /// </summary>
        /// <param name="results">A collection of the declarative validation errors. You may add and remove errors from this collection.</param>

        protected virtual void OnValidateObject(ValidationResultCollection results)
        {
        }

        /// <summary>
        /// Override this method to add imperative validation at the property level.
        /// </summary>
        /// <param name="propertyName">The name of the property being validated.</param>
        /// <param name="results">A collection of the declarative validation errors. You may add and remove errors from this collection.</param>

        protected virtual void OnValidateProperty(string propertyName, ValidationResultCollection results)
        {
        }

        [NotMapped]
        internal ErrorsDictionary ErrorsDictionary
        {
            get { return m_Errors; }
        }

        /// <summary>
        /// Clears the error collections and the HasErrors property
        /// </summary>
        public void ClearErrors()
        {
            OnErrorsChanged("", ErrorsDictionary.Clear());
        }

        /// <summary>
        /// Returns True if there are any errors.
        /// </summary>
        /// <value>
        ///   <c>true</c> if there are errors; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// Call Validate() to refresh this property
        /// </remarks>
        [NotMapped]
        public bool HasErrors
        {
            get { return ErrorsDictionary.HasErrors(); }
        }

        /// <summary>
        /// Returns a collection of all errors (object and property level).
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Call Validate() to refresh this property.
        /// </remarks>

        public ReadOnlyCollection<ValidationResult> GetAllErrors()
        {
            return ErrorsDictionary.GetAllErrors();
        }

        /// <summary>
        /// Returns a collection of object-level errors.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Call Validate() to refresh this property.
        /// </remarks>

        public ReadOnlyCollection<ValidationResult> GetErrors()
        {
            return GetErrors("");
        }

        /// <summary>
        /// Returns a collection of property-level errors.
        /// </summary>
        /// <param name="propertyName">Null or String.Empty will return the object-level errors</param>
        /// <returns></returns>
        /// <remarks>
        /// Call Validate() to refresh this property.
        /// </remarks>

        public ReadOnlyCollection<ValidationResult> GetErrors(string propertyName)
        {
            return ErrorsDictionary.GetErrors(propertyName);
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
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }
    }

    partial class AbstractModelBase : IDataErrorInfo
    {
        /// <summary>
        /// Returns the errors associated with the object. Does not include property level errors.
        /// </summary>
        /// <returns>An error message indicating what is wrong with this object. The default is an empty string ("").</returns>
        string IDataErrorInfo.Error
        {
#pragma warning disable CA1033 // Interface methods should be callable by child types
            get
#pragma warning restore CA1033 // Interface methods should be callable by child types
            {
                var errors = from e in GetErrors("") select e.ToString();
                return string.Join("\n", errors.ToArray());
            }
        }

        /// <summary>
        /// Returns the errors associated with a specific property
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        string IDataErrorInfo.this[string columnName]
        {
#pragma warning disable CA1033 // Interface methods should be callable by child types
            get
#pragma warning restore CA1033 // Interface methods should be callable by child types
            {
                var errors = from e in GetErrors(columnName) select e.ToString();
                return string.Join("\n", errors.ToArray());
            }
        }
    }
}