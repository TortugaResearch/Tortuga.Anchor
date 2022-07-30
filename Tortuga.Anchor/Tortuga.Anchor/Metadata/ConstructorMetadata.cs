using System.Collections.Immutable;
using System.Reflection;
using Tortuga.Anchor.Modeling;

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

		Attributes = ImmutableArray.CreateRange(Attribute.GetCustomAttributes(constructorInfo, true));

		IsPreferred = Attributes.OfType<PreferredConstructorAttribute>().Any();
	}

	/// <summary>
	/// Complete list of attributes that apply to the constructor.
	/// </summary>
	/// <remarks>This does not include parameter level attributes.</remarks>
	public ImmutableArray<Attribute> Attributes { get; }

	/// <summary>
	/// Gets the constructor information.
	/// </summary>
	/// <value>The constructor information.</value>
	public ConstructorInfo ConstructorInfo { get; }

	/// <summary>
	/// Gets a value indicating whether this is the preferred constructor.
	/// </summary>
	public bool IsPreferred { get; }

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
