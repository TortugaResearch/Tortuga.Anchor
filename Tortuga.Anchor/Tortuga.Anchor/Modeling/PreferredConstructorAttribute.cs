namespace Tortuga.Anchor.Modeling;

/// <summary>
/// Indicates that this constructor should be used when multiple constructors are avaialble.
/// </summary>
[AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false)]
public sealed class PreferredConstructorAttribute : Attribute
{
}
