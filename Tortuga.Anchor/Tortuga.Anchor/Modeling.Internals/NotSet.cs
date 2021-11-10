namespace Tortuga.Anchor.Modeling.Internals;

/// <summary>
/// Indicates a property was never set;
/// </summary>
public sealed class NotSet
{
	/// <summary>
	/// Indicates a property was never set;
	/// </summary>
	public static readonly NotSet Value = new();

	private NotSet() { }
}
