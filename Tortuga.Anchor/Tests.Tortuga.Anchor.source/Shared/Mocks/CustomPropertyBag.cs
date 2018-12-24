using Tortuga.Anchor.Modeling.Internals;

namespace Tests.Mocks
{
    public class CustomPropertyBag : PropertyBag
    {
        private readonly int m_Sample;
        public CustomPropertyBag(object owner, int sample)
            : base(owner)
        {
            m_Sample = sample;

        }
        public int Sample
        {
            get { return m_Sample; }
        }
    }
}
