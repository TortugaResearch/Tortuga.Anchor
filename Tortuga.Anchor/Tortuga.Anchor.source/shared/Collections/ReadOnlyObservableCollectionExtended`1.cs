using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Tortuga.Anchor.Eventing;

namespace Tortuga.Anchor.Collections
{
    /// <summary>
    /// A ReadOnlyObservableCollection that includes the functionality from ExtendedObservableCollection. 
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    public class ReadOnlyObservableCollectionExtended<T> : ReadOnlyCollection<T>,
    INotifyCollectionChanged, INotifyPropertyChanged, INotifyCollectionChangedWeak, INotifyPropertyChangedWeak, INotifyItemPropertyChanged
    {
        private readonly ObservableCollectionExtended<T> m_List;
        private readonly Listener<NotifyCollectionChangedEventArgs> m_SourceCollectionChanged;
        private readonly Listener<RelayedEventArgs<PropertyChangedEventArgs>> m_SourceItemPropertyChanged;
        private readonly IListener<PropertyChangedEventArgs> m_SourcePropertyChanged;
        private CollectionChangedEventManager m_CollectionChangeEventManager;
        private int m_PreviousCount;
        private PropertyChangedEventManager m_PropertyChangedEventManager;

        /// <summary>
        /// Initializes a new instance of the ExtendedReadOnlyObservableCollection
        /// class that serves as a wrapper around the specified System.Collections.ObjectModel.ObservableCollection.
        /// </summary>
        /// <param name="list">
        /// The System.Collections.ObjectModel.ObservableCollection with which to
        /// create this instance of the System.Collections.ObjectModel.ReadOnlyObservableCollection
        /// class.
        /// </param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]

        public ReadOnlyObservableCollectionExtended(ObservableCollectionExtended<T> list)
            : base(list)
        {
            m_List = list;

            m_SourceCollectionChanged = new Listener<NotifyCollectionChangedEventArgs>(OnSourceCollectionChanged);
            m_SourcePropertyChanged = new Listener<PropertyChangedEventArgs>(OnSourcePropertyChanged);
            m_SourceItemPropertyChanged = new Listener<RelayedEventArgs<PropertyChangedEventArgs>>(OnItemPropertyChanged);

            list.AddHandler(m_SourceCollectionChanged);
            list.AddHandler(m_SourcePropertyChanged);
            list.AddHandler(m_SourceItemPropertyChanged);


            m_PreviousCount = Count;
        }


        /// <summary>
        /// Occurs when an item is added or removed.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Use this event to listen for changes to properties on items contained by this collection without having to explicitly attach an event handler to each item.
        /// </summary>
        public event RelayedEventHandler<PropertyChangedEventArgs> ItemPropertyChanged;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The list being wrapped.
        /// </summary>
        protected ObservableCollectionExtended<T> SourceList
        {
            get { return m_List; }
        }

        /// <summary>
        /// Adds a weak event handler
        /// </summary>
        /// <param name="eventHandler"></param>
        public void AddHandler(IListener<NotifyCollectionChangedEventArgs> eventHandler)
        {
            if (eventHandler == null)
                throw new ArgumentNullException(nameof(eventHandler), $"{nameof(eventHandler)} is null.");

            if (m_CollectionChangeEventManager == null)
                m_CollectionChangeEventManager = new CollectionChangedEventManager(this);

            m_CollectionChangeEventManager.AddHandler(eventHandler);
        }

        /// <summary>
        /// Adds a weak event handler
        /// </summary>
        /// <param name="eventHandler"></param>
        public void AddHandler(IListener<PropertyChangedEventArgs> eventHandler)
        {
            if (eventHandler == null)
                throw new ArgumentNullException(nameof(eventHandler), $"{nameof(eventHandler)} is null.");


            if (m_PropertyChangedEventManager == null)
                m_PropertyChangedEventManager = new PropertyChangedEventManager(this);

            m_PropertyChangedEventManager.AddHandler(eventHandler);
        }

        /// <summary>
        /// Removes a weak event handler
        /// </summary>
        /// <param name="eventHandler"></param>
        public void RemoveHandler(IListener<NotifyCollectionChangedEventArgs> eventHandler)
        {
            if (eventHandler == null)
                throw new ArgumentNullException(nameof(eventHandler), $"{nameof(eventHandler)} is null.");


            m_CollectionChangeEventManager?.RemoveHandler(eventHandler);
        }

        /// <summary>
        /// Removes a weak event handler
        /// </summary>
        /// <param name="eventHandler"></param>
        public void RemoveHandler(IListener<PropertyChangedEventArgs> eventHandler)
        {
            if (eventHandler == null)
                throw new ArgumentNullException(nameof(eventHandler), $"{nameof(eventHandler)} is null.");

            m_PropertyChangedEventManager?.RemoveHandler(eventHandler);
        }

        /// <summary>
        /// Raises the CollectionChanged
        /// event using the provided arguments.
        /// </summary>
        /// <param name="args">Arguments of the event being raised.</param>
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            CollectionChanged?.Invoke(this, args);

            if (Count != m_PreviousCount)
            {
                m_PreviousCount = Count;
                OnPropertyChanged(CommonProperties.CountProperty);
            }
            OnPropertyChanged(CommonProperties.ItemIndexedProperty);
        }

        /// <summary>
        /// Raises the PropertyChanged
        /// event using the provided arguments.
        /// </summary>
        /// <param name="propertyName">Arguments of the event being raised.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Raises the PropertyChanged
        /// event using the provided arguments.
        /// </summary>
        /// <param name="args">Arguments of the event being raised.</param>
        protected void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChanged?.Invoke(this, args);
        }

        /// <summary>
        /// This method is called when a property on the source collection changes. You may use this to forward property change notifications for properties on this class that wrap the source collection.
        /// </summary>
        /// <remarks>
        /// This will not forward the properties "Count" and "Item[]". 
        /// </remarks>
        protected virtual void OnSourcePropertyChanged(string propertyName) { }

        void OnItemPropertyChanged(object sender, RelayedEventArgs<PropertyChangedEventArgs> e)
        {
            ItemPropertyChanged?.Invoke(this, e);
        }

        void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnCollectionChanged(e);
        }
        void OnSourcePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.PropertyName))
                OnPropertyChanged(e);
            else if (e.PropertyName != CommonProperties.CountProperty.PropertyName && e.PropertyName != CommonProperties.ItemIndexedProperty.PropertyName)
                OnSourcePropertyChanged(e.PropertyName);
        }
    }
}
