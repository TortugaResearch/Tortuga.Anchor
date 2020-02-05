#if !ORDINAL_STRINGS

using System;

namespace Tortuga.Anchor
{
    static class LegacyUtilities
    {
        public static int GetHashCode(this string source, StringComparison stringComparison)
        {
            switch (stringComparison)
            {
                case StringComparison.CurrentCultureIgnoreCase:
                case StringComparison.OrdinalIgnoreCase:
                case StringComparison.InvariantCultureIgnoreCase:
                    return source.ToUpperInvariant().GetHashCode();

                default:
                    return source.GetHashCode();
            }
        }
    }
}

#endif
