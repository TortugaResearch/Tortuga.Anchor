using Tortuga.Anchor.Modeling;

namespace Tests.Metadata;

public class TwoConstructorsTwoPerferred
{
	[PreferredConstructor]
	public TwoConstructorsTwoPerferred(int a) { }

	[PreferredConstructor]
	public TwoConstructorsTwoPerferred(int a, int b) { }
}
