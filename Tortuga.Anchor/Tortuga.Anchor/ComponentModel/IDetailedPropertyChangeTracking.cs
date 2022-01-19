using System.ComponentModel;

namespace Tortuga.Anchor.ComponentModel;

/// <summary>
/// This interface adds the ability to retrieve the previous values.
/// </summary>
public interface IDetailedPropertyChangeTracking : IPropertyChangeTracking, IRevertibleChangeTracking
{
	/// <summary>
	/// Gets the previous value for the indicated property.
	/// </summary>
	/// <param name="propertyName">Name of the property.</param>
	/// <returns>System.Object.</returns>
	object? GetPreviousValue(string propertyName);
}
