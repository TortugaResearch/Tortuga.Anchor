using System.ComponentModel.DataAnnotations.Schema;
using Tortuga.Anchor.Modeling;

namespace Tests.Metadata;

[Table("ChildTable")]
[View("ChildView")]
public class ChildA
{
	[Column("PropertyA2")]
	public int Property { get; set; }

	public int PropertyA1 { get; set; }

	[NotMapped]
	public int PropertyAX { get; set; }
}
