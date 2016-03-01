
namespace Tests.ChangeTracking
{
    using Tortuga.Anchor.Modeling;
    public class Customer : ChangeTrackingModelBase
    {
        public string FirstName
        {
            get { return Get<string>(); }
            set { Set(value); }
        }

        public string LastName
        {
            get { return GetNew(); }
            set { Set(value); }
        }

        [CalculatedField("FirstName,LastName")]
        public string FullName
        {
            get { return (FirstName + " " + LastName).Trim(); }
        }

        public OrdersCollection Orders { get { return GetNew<OrdersCollection>(); } }

    }
}
