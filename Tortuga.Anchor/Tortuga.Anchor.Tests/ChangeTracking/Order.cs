using System;
using System.ComponentModel;
using System.Linq;
using Tortuga.Anchor.Eventing;
using Tortuga.Anchor.Modeling;

namespace Tests.ChangeTracking
{
    public class Order : ChangeTrackingModelBase
    {
        /// <summary>
        /// Initializes a new instance of the Order class.
        /// </summary>
        public Order()
        {
            Lines.CollectionChanged += Lines_CollectionChanged;
            Lines.ItemPropertyChanged += Lines_ItemPropertyChanged;
        }

        public OrderLinesCollection Lines { get { return GetNew<OrderLinesCollection>(); } }

        public DateTime OrderDate
        {
            get { return Get<DateTime>(); }
            set { Set(value); }
        }

        public decimal TotalOrderCost
        {
            get { return Lines.Sum(line => line.Extended); }
        }

        void Lines_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("TotalOrderCost");
        }

        void Lines_ItemPropertyChanged(object sender, RelayedEventArgs<PropertyChangedEventArgs> e)
        {
            OnPropertyChanged("TotalOrderCost");
        }
    }
}