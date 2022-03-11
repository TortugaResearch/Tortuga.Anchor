namespace Tortuga.Anchor.Modeling;

/// <summary>
/// The Decompose attribute indicates that property is complex and its child properties should be treated as part of the parent object.
/// </summary>
/// <remarks>The optional prefix attribute can be used to match with result set column names.</remarks>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Property)]
public sealed class DecomposeAttribute : Attribute
{
	/// <summary>
	/// Initializes a new instance of the <see cref="DecomposeAttribute"/> class.
	/// </summary>
	public DecomposeAttribute() { }

	/// <summary>
	/// Initializes a new instance of the <see cref="DecomposeAttribute"/> class.
	/// </summary>
	/// <param name="prefix">The prefix.</param>
	public DecomposeAttribute(string? prefix) { Prefix = prefix; }

	/// <summary>
	/// Gets the prefix used in the result set columns.
	/// </summary>
	/// <value>The prefix.</value>
	public string? Prefix { get; }
}
