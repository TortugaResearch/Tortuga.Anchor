namespace Tortuga.Anchor
{
    /// <summary>
    /// String join options
    /// </summary>
    public enum StringJoinOption
    {
        /// <summary>
        /// Use the same behavior as string.Join
        /// </summary>
        None = 0,

        /// <summary>
        /// Nulls are skipped. This prevents two separators from appearing one after the other.
        /// </summary>
        DiscardNulls = 1,

        /// <summary>
        /// Nulls and empty strings are skipped. This prevents two separators from appearing one after the other.
        /// </summary>
        DiscardEmptyAndNull = 2
    }
}
