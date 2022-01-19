using Tortuga.Anchor.Modeling;

namespace Tests.ChangeTracking;

public class OrderLine : ChangeTrackingModelBase
{
	/// <summary>
	/// Initializes a new instance of the OrderLine class.
	/// </summary>
	public OrderLine(decimal cost, decimal quantity)
	{
		Cost = cost;
		Quantity = quantity;
	}

	public decimal Cost
	{
		get { return Get<decimal>(); }
		set { Set(value); }
	}

	[CalculatedField("Cost,Quantity")]
	public decimal Extended
	{
		get { return Cost * Quantity; }
	}

	public decimal Quantity
	{
		get { return Get<decimal>(); }
		set { Set(value); }
	}
}
