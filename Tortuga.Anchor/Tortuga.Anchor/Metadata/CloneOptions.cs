namespace Tortuga.Anchor.Metadata;

/// <summary>
/// Enum CloneOptions
/// </summary>
[Flags]
public enum CloneOptions
{
	/// <summary>
	/// No optional behavior selected
	/// </summary>
	None = 0,

	/// <summary>
	/// The bypass properties by directly copying the property bag. Only applies to classes that inherit from AbstractModelBase or AbstractModelCollection
	/// </summary>
	/// <remarks>Warning: This will not work if you have a mix of normal and property bag properties</remarks>
	BypassProperties = 1,

	/// <summary>
	/// Perform a deep clone on all properties. 
	/// </summary>
	/// <remarks>Set maxRecursions to limit depth of cloning</remarks>
	DeepClone = 2,

	/// <summary>
	/// Invoke IClonable.Clone() on any value that implements IClonable
	/// </summary>
	/// <remarks>Recursive cloning will not be used on objects where IClonable.Clone() is invoked.</remarks>
	UseIClonable = 4,

	/// <summary>
	/// Invoke a parameterless Clone() method on any value that has one. If both UseClone and UseIClonable are specified, UseIClonable takes precedence.
	/// </summary>
	/// <remarks>Recursive cloning will not be used on objects where Clone() is invoked.</remarks>
	UseClone = 8,


	/// <summary>
	/// When using the ReusePureObjects flag, the Clone operation will skip objects that are marked with the Pure attribute.
	/// 
	/// WARNING: The PureAttribute is not accessible unless the compiler constant CONTRACTS_FULL is defined.
	/// </summary>
	/// <remarks>This has no effect unless DeepClone is also specified.</remarks>
	ReusePureObjects = 16
}
