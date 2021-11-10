using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Tortuga.Anchor.ComponentModel;

/// <summary>
/// This interface exposes strongly typed validation information.
/// </summary>
/// <remarks>While this interface is easier to work with than IDataErrorInfo, classes should still implement IDataErrorInfo so that they work with UI components.</remarks>
public interface IValidatable
{
	/// <summary>
	/// Raised when the errors collection has changed.
	/// </summary>
	/// <remarks>
	/// This may be fired even when no actual change has occurred.
	/// </remarks>
	event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

	/// <summary>
	/// Returns True if there are any errors.
	/// </summary>
	/// <value>
	///   <c>true</c> if there are errors; otherwise, <c>false</c>.
	/// </value>
	/// <remarks>
	/// Call Validate() to refresh this property
	/// </remarks>
	bool HasErrors { get; }

	/// <summary>
	/// Clears the error collections and the HasErrors property
	/// </summary>
	void ClearErrors();

	/// <summary>
	/// Returns a collection of all errors (object and property level).
	/// </summary>
	/// <returns></returns>
	/// <remarks>
	/// Call Validate() to refresh this property.
	/// </remarks>
	ReadOnlyCollection<ValidationResult> GetAllErrors();

	/// <summary>
	/// Returns a collection of object-level errors.
	/// </summary>
	/// <returns></returns>
	/// <remarks>
	/// Call Validate() to refresh this property.
	/// </remarks>
	ReadOnlyCollection<ValidationResult> GetErrors();

	/// <summary>
	/// Returns a collection of property-level errors.
	/// </summary>
	/// <param name="propertyName">Null or String.Empty will return the object-level errors</param>
	/// <returns></returns>
	/// <remarks>
	/// Call Validate() to refresh this property.
	/// </remarks>
	ReadOnlyCollection<ValidationResult> GetErrors(string propertyName);

	/// <summary>
	/// This forces the object to be completely revalidated.
	/// </summary>
	/// <returns>
	/// True if the object has no errors
	/// </returns>
	bool Validate();
}
