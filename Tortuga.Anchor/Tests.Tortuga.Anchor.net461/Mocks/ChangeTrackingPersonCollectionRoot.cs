namespace Tests.Mocks
{
    using Tortuga.Anchor.Modeling;
    public class ChangeTrackingPersonCollectionRoot : ChangeTrackingModelBase
    {
        public ChangeTrackingPersonCollection ChangeTrackingPersonCollection { get { return GetNew<ChangeTrackingPersonCollection>("ChangeTrackingPersonCollection"); } }
    }
}
