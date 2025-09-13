using System.Diagnostics.Contracts;

namespace Tests.Metadata;

[Pure]
public class PureClass
{
	public int Value { get; set; }
	public DateTime CreatedAt { get; } = DateTime.Now;
}
