namespace Tests.Metadata;

public class WrapsCloneMethod
{
	public WrapsCloneMethod()
	{
	}

	public WrapsCloneMethod(int value)
	{
		Cloneable = new() { Value = value };
		NotCloneable = new() { Value = value };
	}

	public CloneMethod? Cloneable { get; set; }
	public NotCloneable? NotCloneable { get; set; }
}
