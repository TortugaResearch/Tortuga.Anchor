#if !NETCOREAPP3_1_OR_GREATER

namespace System.Diagnostics.CodeAnalysis;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = true, Inherited = false)]
internal sealed class MaybeNullWhen : Attribute
{
	public MaybeNullWhen(bool returnValue)
	{
		ReturnValue = returnValue;
	}

	public bool ReturnValue { get; }
}

#endif
