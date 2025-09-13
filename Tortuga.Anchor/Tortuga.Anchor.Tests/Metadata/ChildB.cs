using System.ComponentModel.DataAnnotations.Schema;

namespace Tests.Metadata;

public class ChildB
{
	[Column("PropertyB2")]
	public int Property { get; set; }

	public int PropertyB1 { get; set; }

	[NotMapped]
	public int PropertyBX { get; set; }
}
