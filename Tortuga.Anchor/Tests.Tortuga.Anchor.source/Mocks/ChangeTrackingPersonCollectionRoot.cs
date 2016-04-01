using Tortuga.Anchor.Modeling;

namespace Tests.Mocks
{
    public class ChangeTrackingPersonCollectionRoot : ChangeTrackingModelBase
    {
        public ChangeTrackingPersonCollection ChangeTrackingPersonCollection { get { return GetNew<ChangeTrackingPersonCollection>("ChangeTrackingPersonCollection"); } }
    }
}
