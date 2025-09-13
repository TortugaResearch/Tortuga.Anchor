using Tortuga.Anchor.Modeling;

namespace Tests.Metadata;

public class TwoConstructorsOnePerferred
{
	[PreferredConstructor]
	public TwoConstructorsOnePerferred(int a) { }

	public TwoConstructorsOnePerferred(int a, int b)
	{
	}
}
