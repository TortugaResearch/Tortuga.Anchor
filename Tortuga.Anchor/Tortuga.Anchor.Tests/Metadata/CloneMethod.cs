namespace Tests.Metadata;

public class CloneMethod
{
	public int Value { get; set; }

	public CloneMethod Clone() => new CloneMethod { Value = Value + 2 };
}
