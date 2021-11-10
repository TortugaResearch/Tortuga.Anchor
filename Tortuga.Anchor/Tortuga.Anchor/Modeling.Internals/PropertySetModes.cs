namespace Tortuga.Anchor.Modeling.Internals;

/// <summary>
/// Used to indicate what special behaviors are needed when setting a value in PropertyBagBase.
/// </summary>
[Flags]
public enum PropertySetModes
{
	/// <summary>
	/// No special effects or events.
	/// </summary>
	None = 0,

	/// <summary>
	/// Causes the model to raise its property changed event
	/// </summary>
	RaiseChangedEvent = 1,

	/// <summary>
	/// For property bags that support it, treat the new value as the original, unchanged value. Primarily used for lazy-loading properties.
	/// </summary>
	SetAsOriginal = 2,

	/// <summary>
	/// Causes the model to revalidate the indicated property.
	/// </summary>
	ValidateProperty = 4,

	/// <summary>
	/// Causes the model to revalidate the object. Property-level validators are not run.
	/// </summary>
	ValidateObject = 8,

	/// <summary>
	/// The property name may have the wrong casing or otherwise need to be mapped.
	/// </summary>
	FixCasing = 16,

	/// <summary>
	/// Causes the model to update its IsChanged and IsChangedLocal properties
	/// </summary>
	UpdateIsChangedProperty = 64
}
