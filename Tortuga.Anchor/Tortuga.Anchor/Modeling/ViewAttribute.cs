namespace Tortuga.Anchor.Modeling;

/// <summary>
/// Allows custom mapping to the database. Use when a different view is used for read vs write operations.
/// </summary>
/// <seealso cref="System.ComponentModel.DataAnnotations.Schema.TableAttribute" />
/// <remarks>The TableAttribute will be used for write operations.</remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class ViewAttribute : Attribute
{
	/// <summary>Initializes a new instance of the <see cref="TableAndViewAttribute" /> class using the specified name of the table.</summary>
	/// <param name="name">The name of the table the class is mapped to.</param>
	public ViewAttribute(string name)
	{
		if (string.IsNullOrEmpty(name))
			throw new ArgumentException($"{nameof(name)} is null or empty.", nameof(name));
		Name = name;
	}

	/// <summary>
	/// Gets or sets the name of the view to be used for read operations.
	/// </summary>
	/// <value>The name of the view.</value>
	public string Name { get; }


	/// <summary>
	/// Gets or sets the schema of the view the class is mapped to.
	/// </summary>
	public string? Schema
	{
#if NET5_0_OR_GREATER
		get; init;
#else
		get; set;
#endif
	}


}
