#if !NETCOREAPP3_1_OR_GREATER

namespace System
{
    //Not the most elegant hash code function, but it is only needed for down-level platforms.

    static class HashCode
    {
        internal static int Combine(int a, int b, int c)
        {
            return a ^ b ^ c;
        }
    }
}
#endif
