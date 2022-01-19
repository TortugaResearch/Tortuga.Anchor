using System.Collections.Immutable;
using System.Reflection;

namespace Tortuga.Anchor.Metadata;

/// <summary>
/// Class ConstructorMetadata.
/// </summary>
public sealed class ConstructorMetadata
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ConstructorMetadata"/> class.
	/// </summary>
	/// <param name="constructorInfo">The constructor information.</param>
	internal ConstructorMetadata(ConstructorInfo constructorInfo)
	{
		ConstructorInfo = constructorInfo;

		Signature = constructorInfo.GetParameters().Select(p => p.ParameterType).ToImmutableArray();
		ParameterNames = constructorInfo.GetParameters().Select(p => p.Name).Cast<string>().ToImmutableArray();
	}

	/// <summary>
	/// Gets the constructor information.
	/// </summary>
	/// <value>The constructor information.</value>
	public ConstructorInfo ConstructorInfo { get; }

	/// <summary>
	/// Gets the parameter names.
	/// </summary>
	/// <value>The parameter names.</value>
	public ImmutableArray<string> ParameterNames { get; }

	/// <summary>
	/// Gets the signature.
	/// </summary>
	/// <value>The signature.</value>
	public ImmutableArray<Type> Signature { get; }
}
