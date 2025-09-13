using System.ComponentModel.DataAnnotations.Schema;
using Tortuga.Anchor.Modeling;

namespace Tests.Metadata;

[Table("TableBase")]
public class Base
{
	[Decompose]
	public ChildA ChildA { get; set; } = null!;

	[Decompose("Bbb")]
	public ChildB ChildB { get; set; } = null!;

	public int Property0 { get; set; }
}
