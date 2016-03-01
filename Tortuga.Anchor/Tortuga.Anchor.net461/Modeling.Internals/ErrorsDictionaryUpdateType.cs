namespace Tortuga.Anchor.Modeling.Internals
{
    /// <summary>
    /// This is used to indicate whether or not the errors collection was actually updated.
    /// </summary>
    internal enum ErrorsDictionaryUpdateType
    {
        /// <summary>
        /// The no changes detected
        /// </summary>
        NoChange = 0,
        /// <summary>
        /// The <c>HasErrors</c> property has been changed to true
        /// </summary>
        HasErrorsIsTrue = 1,
        /// <summary>
        /// The <c>HasErrors</c> property has been changed to false
        /// </summary>
        HasErrorsIsFalse = 2,
        /// <summary>
        /// One or more errors changed
        /// </summary>
        ErrorsChanged = 3
    }
}
