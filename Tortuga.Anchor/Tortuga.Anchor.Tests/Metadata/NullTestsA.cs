namespace Tests.Metadata;

public class NullTestsA
{
	public object NotNull { get; set; } = null!;
	public object? Nullable { get; set; }

#nullable disable
	public object NullUnknown { get; set; }
#nullable enable
}
