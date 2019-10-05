namespace Tortuga.Anchor.Modeling
{
    /// <summary>
    /// Delegate for the ValueChanged action used in the Set method.
    /// </summary>
    /// <typeparam name="T">Type of the property</typeparam>
    /// <param name="oldValue">Previous value of the property.</param>
    /// <param name="newValue">Current value of the property</param>
    public delegate void ValueChanged<in T>(T oldValue, T newValue);
}