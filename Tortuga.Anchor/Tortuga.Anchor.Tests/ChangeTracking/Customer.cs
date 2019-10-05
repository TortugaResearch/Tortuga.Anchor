using Tortuga.Anchor.Modeling;

namespace Tests.ChangeTracking
{
    public class Customer : ChangeTrackingModelBase
    {
        public string FirstName
        {
            get { return Get<string>(); }
            set { Set(value); }
        }

        [CalculatedField("FirstName,LastName")]
        public string FullName
        {
            get { return (FirstName + " " + LastName).Trim(); }
        }

        public string LastName
        {
            get { return GetNew(); }
            set { Set(value); }
        }

        public OrdersCollection Orders { get { return GetNew<OrdersCollection>(); } }
    }
}