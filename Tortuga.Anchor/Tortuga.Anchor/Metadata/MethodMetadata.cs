using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Reflection;

namespace Tortuga.Anchor.Metadata;

/// <summary>
/// This is a cache of metadata about a specific method.
/// </summary>
public class MethodMetadata
{
	internal MethodMetadata(MethodInfo info)
	{
		MethodInfo = info ?? throw new ArgumentNullException(nameof(info), $"{nameof(info)} is null.");
		Attributes = ImmutableArray.CreateRange(Attribute.GetCustomAttributes(info, true));

		Name = info.Name;
		Signature = info.GetParameters().Select(p => p.ParameterType).ToImmutableArray();
		ParameterNames = info.GetParameters().Select(p => p.Name).Cast<string>().ToImmutableArray();
		ReturnType = info.ReturnType;

		IsStatic = info.IsStatic;

		IsPure = Attributes.OfType<PureAttribute>().Any();
	}

	/// <summary>
	/// Complete list of attributes that apply to the property
	/// </summary>
	public ImmutableArray<Attribute> Attributes { get; }

	/// <summary>
	/// Gets a value indicating whether this method is protected.
	/// </summary>
	/// <value><c>true</c> if this method is protected; otherwise, <c>false</c>.</value>
	public bool IsProtected => MethodInfo.IsPublic && MethodInfo.IsFamily;

	/// <summary>
	/// Gets a value indicating whether this method is public.
	/// </summary>
	/// <value><c>true</c> if this method is public; otherwise, <c>false</c>.</value>
	public bool IsPublic => MethodInfo.IsPublic && !MethodInfo.IsFamily;

	/// <summary>
	/// Gets a value indicating whether this method is pure.
	/// </summary>
	/// <value><c>true</c> if this method has a PureAttribute; otherwise, <c>false</c>.</value>
	public bool IsPure { get; }

	/// <summary>
	/// Gets a value indicating whether this instance is static.
	/// </summary>
	/// <value><c>true</c> if this instance is static; otherwise, <c>false</c>.</value>
	public bool IsStatic { get; }

	/// <summary>
	/// Gets the method information.
	/// </summary>
	/// <value>The constructor information.</value>
	public MethodInfo MethodInfo { get; }

	/// <summary>
	/// Public name of the method
	/// </summary>
	public string Name { get; }

	/// <summary>
	/// Gets the parameter names.
	/// </summary>
	/// <value>The parameter names.</value>
	public ImmutableArray<string> ParameterNames { get; }

	/// <summary>
	/// Gets the return type of this method.
	/// </summary>
	public Type? ReturnType { get; }

	/// <summary>
	/// Gets the method signature.
	/// </summary>
	/// <value>The signature.</value>
	public ImmutableArray<Type> Signature { get; }

	/// <summary>
	/// Invokes the method against the specified target.
	/// </summary>
	/// <param name="target">The target.</param>
	/// <param name="parameters">The parameters.</param>
	/// <returns>System.Object.</returns>
	public object? Invoke(object target, object[] parameters) => MethodInfo.Invoke(target, parameters);

	/// <summary>
	/// Invokes the static method.
	/// </summary>
	/// <param name="parameters">The parameters.</param>
	/// <returns>System.Object.</returns>
	public object? InvokeStatic(object[] parameters) => MethodInfo.Invoke(null, parameters);

	/// <summary>
	/// Returns a <see cref="String" /> that represents this instance.
	/// </summary>
	/// <returns>A <see cref="String" /> that represents this instance.</returns>
	public override string ToString()
	{
		return Name + "(" + String.Join(", ", Signature.Select(x => x.CSharpFullName())) + ")";
	}
}
