using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Tortuga.Anchor.Modeling.Internals
{
    /// <summary>
    /// Used to compare ValidationResults
    /// </summary>
    /// <remarks>
    /// This does not properly support subclasses 
    /// </remarks>

    public class ValidationResultEqualityComparer : IEqualityComparer<ValidationResult>
    {
        ValidationResultEqualityComparer() { }

        /// <summary>
        /// Performs a value comparison by looking at the error message and member names
        /// </summary>
        /// <param name="x">The first object of to compare.</param>
        /// <param name="y">The second object of to compare.</param>
        /// <returns>
        /// true if the specified objects are equal; otherwise, false.
        /// </returns>
        /// <remarks>
        /// WARNING: The result of Equals can change! It is based on ValidationResult, which unfortunately isn't immutable.
        /// </remarks>
        public bool Equals(ValidationResult x, ValidationResult y)
        {

            if (x == y)
                return true;

            if (x == null || y == null)
                return false;

            if (x.ErrorMessage != y.ErrorMessage)
                return false;

            if (x.MemberNames == y.MemberNames)
                return true;

            return x.MemberNames.SequenceEqual(y.MemberNames);
        }

        /// <summary>
        /// Returns a hash code derived from the error message.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        /// <remarks>
        /// WARNING: The result of GetHashCode can change! It is based on ValidationResult, which unfortunately isn't immutable.
        /// </remarks>
        public int GetHashCode(ValidationResult obj)
        {
            if (obj == null || obj.ErrorMessage == null)
                return 0;

            return obj.ErrorMessage.GetHashCode();
        }

        /// <summary>
        /// Returns the default instance of the comparer.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public static readonly ValidationResultEqualityComparer Default = new ValidationResultEqualityComparer();
    }
}
