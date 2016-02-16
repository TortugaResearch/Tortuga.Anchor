using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Tortuga.Anchor.Eventing;

namespace Tortuga.Anchor.Collections
{
    /// <summary>
    /// A ReadOnlyObservableCollection that includes the functionality from ObservableCollectionExtended. 
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    public class ReadOnlyObservableCollectionExtended<T> : ReadOnlyCollection<T>,
    INotifyCollectionChanged, INotifyPropertyChanged, INotifyItemPropertyChanged
    {
        private readonly ObservableCollectionExtended<T> m_List;
        private int m_PreviousCount;

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

            list.CollectionChanged += OnSourceCollectionChanged;
            list.PropertyChanged += OnSourcePropertyChanged;
            list.ItemPropertyChanged += OnItemPropertyChanged;

            m_PreviousCount = Count;
        }


        void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnCollectionChanged(e);
        }

        /// <summary>
        /// Occurs when an item is added or removed.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Use this event to listen for changes to properties on items contained by this collection without having to explicitly attach an event handler to each item.
        /// </summary>
        public event RelayedEventHandler<PropertyChangedEventArgs> ItemPropertyChanged;


        /// <summary>
        /// Raises the CollectionChanged
        /// event using the provided arguments.
        /// </summary>
        /// <param name="args">Arguments of the event being raised.</param>
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (CollectionChanged != null)
                CollectionChanged(this, args);

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
            if (PropertyChanged != null)
                PropertyChanged(this, args);
        }

        /// <summary>
        /// This method is called when a property on the source collection changes. You may use this to forward property change notifications for properties on this class that wrap the source collection.
        /// </summary>
        /// <remarks>
        /// This will not forward the properties "Count" and "Item[]". 
        /// </remarks>
        protected virtual void OnSourcePropertyChanged(string propertyName) { }

        void OnSourcePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.PropertyName))
                OnPropertyChanged(e);
            else if (e.PropertyName != CommonProperties.CountProperty.PropertyName && e.PropertyName != CommonProperties.ItemIndexedProperty.PropertyName)
                OnSourcePropertyChanged(e.PropertyName);
        }


        void OnItemPropertyChanged(object sender, RelayedEventArgs<PropertyChangedEventArgs> e)
        {
            if (ItemPropertyChanged != null)
                ItemPropertyChanged(this, e);
        }

        /// <summary>
        /// The list being wrapped.
        /// </summary>
        protected ObservableCollectionExtended<T> SourceList
        {
            get { return m_List; }
        }


    }
}
