namespace Tortuga.Anchor
{
    /// <summary>
    /// Provides the functionality to offer custom error information that a user interface
    /// can bind to.
    /// </summary>
    public interface IDataErrorInfo
    {

        /// <summary>
        /// Gets the error message for the property with the given name.
        /// </summary>
        /// <param name="columnName">
        /// The name of the property whose error message to get.
        /// </param>
        /// <value>
        /// The error message for the property. The default is an empty string ("").
        /// </value>
        string this[string columnName] { get; }

  
        /// <summary>
        /// Gets an error message indicating what is wrong with this object.
        /// </summary>
        /// <value>
        /// An error message indicating what is wrong with this object. The default is an
        /// empty string ("").
        /// </value>
        string Error { get; }
    }
}
