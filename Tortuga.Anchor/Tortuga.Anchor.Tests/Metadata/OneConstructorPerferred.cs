using Tortuga.Anchor.Modeling;

namespace Tests.Metadata;

public class OneConstructorPerferred
{
	[PreferredConstructor]
	public OneConstructorPerferred(int a) { }
}
