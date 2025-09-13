using System.ComponentModel.DataAnnotations.Schema;

namespace Tests.Metadata;

public class EmptyColumn
{
	[Column(TypeName = "money")]
	public int Property0 { get; set; }
}
