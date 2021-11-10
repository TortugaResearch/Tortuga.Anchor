using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tortuga.Anchor.Modeling;

namespace Tests.Metadata;

class Mock
{
	private int PrivateProperty { get; set; }

	public int PublicProperty { get; set; }
	protected int ProtectedProperty { get; set; }
	public int PublicPrivateProperty { get; private set; }
	public int PublicProtectedProperty { get; protected set; }
	public int SetOnlyProperty { set { } }

	[Required()]
	public string CalculatedSource1 { get; set; }
	public string CalculatedSource2 { get; set; }

	[CalculatedField("CalculatedSource1")]
	[CalculatedField("CalculatedSource2")]
	public int CalculatedTarget { get { return 0; } }

}
