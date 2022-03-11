using System.ComponentModel.DataAnnotations;
using Tortuga.Anchor.Modeling;

namespace Tests.Metadata;

class Mock
{
	private int PrivateProperty { get; set; }

	public int PublicProperty { get; set; }
	protected int ProtectedProperty { get; set; }
	public int PublicPrivateProperty { get; private set; }
	public int PublicProtectedProperty { get; protected set; }
#pragma warning disable CA1822 // Mark members as static
	public int SetOnlyProperty { set { } }
#pragma warning restore CA1822 // Mark members as static

	[Required()]
	public string CalculatedSource1 { get; set; } = null!;
	public string CalculatedSource2 { get; set; } = null!;

	[CalculatedField("CalculatedSource1")]
	[CalculatedField("CalculatedSource2")]
	public int CalculatedTarget { get { return 0; } }

}
