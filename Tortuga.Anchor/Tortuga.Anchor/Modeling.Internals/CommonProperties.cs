using System.ComponentModel;

namespace Tortuga.Anchor.Modeling.Internals;

/// <summary>
/// These are used for property changed notifications so that new objects don't need to be allocated.
/// </summary>

internal static class CommonProperties
{
	/// <summary>
	/// Used to indicate that many or all properties have been changed.
	/// </summary>
	public readonly static PropertyChangedEventArgs Empty = new("");

	/// <summary>
	/// The "HasErrors" property
	/// </summary>
	public readonly static PropertyChangedEventArgs HasErrorsProperty = new("HasErrors");

	/// <summary>
	/// The "IsChangedLocal" property
	/// </summary>
	public readonly static PropertyChangedEventArgs IsChangedLocalProperty = new("IsChangedLocal");

	/// <summary>
	/// The "IsChanged" property
	/// </summary>
	public readonly static PropertyChangedEventArgs IsChangedProperty = new("IsChanged");
}
