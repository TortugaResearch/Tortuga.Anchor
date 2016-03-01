using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Tortuga.Anchor.ComponentModel;
using Tortuga.Anchor.Eventing;

namespace Tortuga.Anchor.Collections
{

    /// <summary>
    /// This is an ObservableCollection with a read-only wrapper and support for weak events.
    /// This will use weak events to listen to objects implementing INotifyPropertyChangedWeak.
    /// This will use normal events to listen to objects implementing INotifyPropertyChanged.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    public partial class ObservableCollectionExtended<T> : ObservableCollection<T>, IReadOnlyList<T>, INotifyItemPropertyChanged
    {
        /// <summary>
        /// When someone attaches to the ItemPropertyChanged event this is set to true and we start listening for change notifications.
        /// </summary>
        private bool m_ListeningToItemEvents;

        //This is created on demand.
        private ReadOnlyObservableCollectionExtended<T> m_ReadOnlyWrapper;

        /// <summary>
        /// Initializes a new instance of the ImprovedObservableCollection class.
        /// </summary>     
        public ObservableCollectionExtended()
        {
        }


        /// <summary>
        /// Initializes a new instance of the ImprovedObservableCollection class that contains elements copied from the specified list.
        /// </summary>
        /// <param name="list"></param>
        [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]

        public ObservableCollectionExtended(List<T> list)
        {
            if (list != null)
                AddRange(list);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableCollectionExtended{T}"/> class.
        /// </summary>
        /// <param name="collection">The collection from which the elements are copied.</param>
        public ObservableCollectionExtended(IEnumerable<T> collection)
        {
            if (collection != null)
                AddRange(collection);
        }

        /// <summary>
        /// This type safe event fires after an item is added to the collection no matter how it is added.
        /// </summary>
        /// <remarks>Triggered by InsertItem and SetItem</remarks>
        public event EventHandler<ItemEventArgs<T>> ItemAdded;

        /// <summary>
        /// Use this event to listen for changes to properties on items contained by this collection without having to explicitly attach an event handler to each item.
        /// </summary>
        public event RelayedEventHandler<PropertyChangedEventArgs> ItemPropertyChanged
        {
            add
            {
                ListenToEvents();
                m_ItemPropertyChangedEvent += value;
            }
            remove { m_ItemPropertyChangedEvent -= value; }
        }

        /// <summary>
        /// This type safe event fires after an item is removed from the collection no matter how it is removed.
        /// </summary>
        /// <remarks>Triggered by SetItem, RemoveItem, and ClearItems</remarks>
        public event EventHandler<ItemEventArgs<T>> ItemRemoved;

        /// <summary>
        /// This just exposes the INotifyPropertyChanged.PropertyChanged from the base class so you don't have to cast to get to it.
        /// </summary>
        public new event PropertyChangedEventHandler PropertyChanged
        {
            add { base.PropertyChanged += value; }
            remove { base.PropertyChanged -= value; }
        }

        private event RelayedEventHandler<PropertyChangedEventArgs> m_ItemPropertyChangedEvent;
        /// <summary>
        /// Returns a read-only wrapper around this collection. 
        /// </summary>
        /// <remarks>
        /// If sub classing this class then it may be useful to shadow ReadOnlyWrapper method 
        /// with one that returns a subclass of ExtendedReadOnlyObservableCollection.
        /// </remarks>
        public ReadOnlyObservableCollectionExtended<T> ReadOnlyWrapper
        {
            get
            {
                if (m_ReadOnlyWrapper == null)
                    m_ReadOnlyWrapper = new ReadOnlyObservableCollectionExtended<T>(this);

                return m_ReadOnlyWrapper;
            }
        }

        /// <summary>
        /// Adds a list of values to this collection
        /// </summary>
        /// <param name="list">The list.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void AddRange(IEnumerable<T> list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list), $"{nameof(list)} is null.");

            foreach (var item in list)
                Add(item);
        }

        /// <summary>
        /// Removes all elements from the collection.
        /// </summary>
        protected override void ClearItems()
        {
            var temp = this.ToList();
            base.ClearItems();
            foreach (var item in temp)
                OnItemRemoved(item);
        }

        /// <summary>
        /// Inserts an item into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The object to insert.</param>
        /// <remarks>Do NOT invoke this method directly. This may be overridden to provide additional validation before an item is added to the collection.</remarks>
        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);
            OnItemAdded(item);
        }

        /// <summary>
        /// This fires after an item is removed from the collection no matter how it is removed.
        /// </summary>
        /// <param name="item"></param>
        /// <remarks>Triggered by SetItem, RemoveItem, and ClearItems.</remarks>
        protected virtual void OnItemAdded(T item)
        {
            if (ItemAdded != null)
                ItemAdded(this, new ItemEventArgs<T>(item));

            if (m_ListeningToItemEvents && item is INotifyPropertyChanged)
                ((INotifyPropertyChanged)item).PropertyChanged += OnItemPropertyChanged;
        }

        /// <summary>
        /// This fires after an item is removed from the collection no matter how it is removed.
        /// </summary>
        /// <param name="item"></param>
        /// <remarks>Triggered by SetItem, RemoveItem, and ClearItems.</remarks>
        protected virtual void OnItemRemoved(T item)
        {
            if (ItemRemoved != null)
                ItemRemoved(this, new ItemEventArgs<T>(item));

            if (m_ListeningToItemEvents && item is INotifyPropertyChanged)
                ((INotifyPropertyChanged)item).PropertyChanged -= OnItemPropertyChanged;

        }
        /// <summary>
        /// Raises the <see cref="E:System.Collections.ObjectModel.ObservableCollection`1.PropertyChanged" /> event with the provided arguments.
        /// </summary>
        /// <param name="propertyName">
        /// Property that is being changed.
        /// </param>
        protected void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Removes the element at the specified index of the <see cref="T:System.Collections.ObjectModel.Collection`1" />.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index" /> is less than zero -or-<paramref name="index" /> is equal to or greater than <see cref="P:System.Collections.ObjectModel.Collection`1.Count" />.</exception>
        /// <remarks>Do NOT invoke this method directly. This may be overridden to provide additional validation before an item is removed to the collection.</remarks>
        protected override void RemoveItem(int index)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), index, $"{nameof(index)} must be >= 0");
            if (index >= Count)
                throw new ArgumentOutOfRangeException(nameof(index), index, $"{nameof(index)} must be < Count");


            T temp = base[index];
            base.RemoveItem(index);
            OnItemRemoved(temp);
        }

        /// <summary>
        /// Replaces the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to replace.</param>
        /// <param name="item">The new value for the element at the specified index.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// </exception>
        /// <remarks>Do NOT invoke this method directly. This may be overridden to provide additional validation before an item is added to the collection.</remarks>
        protected override void SetItem(int index, T item)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), index, $"{nameof(index)} must be >= 0");
            if (index >= Count)
                throw new ArgumentOutOfRangeException(nameof(index), index, $"{nameof(index)} must be < Count");


            T temp = base[index];
            base.SetItem(index, item);
            OnItemRemoved(temp);
            OnItemAdded(item);
        }
        /// <summary>
        /// This enables the ItemPropertyChanged events. 
        /// </summary>
        private void ListenToEvents()
        {
            if (m_ListeningToItemEvents)
                return;

            foreach (var item in this.OfType<INotifyPropertyChanged>())
                item.PropertyChanged += OnItemPropertyChanged;

            m_ListeningToItemEvents = true;
        }

        void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (m_ItemPropertyChangedEvent != null)
                m_ItemPropertyChangedEvent(this, RelayedEventArgs.Create(sender, e));
        }
    }


}
