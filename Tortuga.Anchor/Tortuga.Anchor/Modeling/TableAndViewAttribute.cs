using System.ComponentModel.DataAnnotations.Schema;

namespace Tortuga.Anchor.Modeling;

/// <summary>
/// Allows custom mapping to the database. Use when a different view is used for read vs write operations.
/// </summary>
/// <seealso cref="System.ComponentModel.DataAnnotations.Schema.TableAttribute" />
public sealed class TableAndViewAttribute : TableAttribute
{
	/// <summary>Initializes a new instance of the <see cref="TableAndViewAttribute" /> class using the specified name of the table.</summary>
	/// <param name="name">The name of the table the class is mapped to.</param>
	public TableAndViewAttribute(string name) : base(name)
	{
		if (string.IsNullOrEmpty(name))
			throw new ArgumentException($"{nameof(name)} is null or empty.", nameof(name));
	}

	/// <summary>
	/// Gets or sets the name of the view to be used for read operations.
	/// </summary>
	/// <value>The name of the view.</value>
	public string? ViewName { get; set; }

	/*
	 * TASK-47: Reserved for future work
	/// <summary>
	/// Gets or sets the name of the insert function or stored procedure.
	/// </summary>
	/// <value>The name of the insert function or stored procedure.</value>
	public string? InsertFunctionName { get; set; }

	/// <summary>
	/// Gets or sets the name of the update function or stored procedure.
	/// </summary>
	/// <value>The name of the update function or stored procedure.</value>
	public string? UpdateFunctionName { get; set; }

	/// <summary>
	/// Gets or sets the name of the delete function or stored procedure.
	/// </summary>
	/// <value>The name of the delete function or stored procedure.</value>
	public string? DeleteFunctionName { get; set; }
	*/
}
