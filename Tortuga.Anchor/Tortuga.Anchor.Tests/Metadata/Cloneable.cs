namespace Tests.Metadata;

public class Cloneable : ICloneable
{
	public int Value { get; set; }

	object ICloneable.Clone() => new Cloneable { Value = Value + 1 };

	public CloneMethod Clone() => new CloneMethod { Value = Value + 2 };
}
