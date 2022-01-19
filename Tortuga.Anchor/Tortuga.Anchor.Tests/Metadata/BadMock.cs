using Tortuga.Anchor.Modeling;

namespace Tests.Metadata;

class BadMock
{
	[CalculatedField("MissingProperty")]
	public int CalculatedTarget { get { return 0; } }
}
