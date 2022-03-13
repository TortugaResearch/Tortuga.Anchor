#if !NETCOREAPP3_1_OR_GREATER
namespace Tortuga.Anchor;

static class LegacyUtilities
{
	public static int GetHashCode(this string source, StringComparison stringComparison)
	{
		return stringComparison switch
		{
			StringComparison.CurrentCultureIgnoreCase or
			StringComparison.OrdinalIgnoreCase or
			StringComparison.InvariantCultureIgnoreCase
				=> source.ToUpperInvariant().GetHashCode(),
			_ => source.GetHashCode(),
		};
	}
}

#endif
