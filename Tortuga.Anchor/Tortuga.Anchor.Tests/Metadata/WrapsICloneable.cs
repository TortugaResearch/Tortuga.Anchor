namespace Tests.Metadata;

public class WrapsICloneable
{
	public WrapsICloneable()
	{
	}

	public WrapsICloneable(int value)
	{
		Cloneable = new() { Value = value };
		NotCloneable = new() { Value = value };
	}

	public Cloneable? Cloneable { get; set; }
	public NotCloneable? NotCloneable { get; set; }
}
