using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Tortuga.Anchor.DataAnnotations
{
    /// <summary>
    /// Collection of validation results
    /// </summary>
    public class ValidationResultCollection : Collection<ValidationResult>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationResultCollection"/> class.
        /// </summary>

        public ValidationResultCollection()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationResultCollection"/> class.
        /// </summary>
        /// <param name="list">The list.</param>

        public ValidationResultCollection(IList<ValidationResult> list)
            : base(list)
        {

        }

        /// <summary>
        /// Adds a new validation result to the collection
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <param name="memberNames"></param>
        /// <returns></returns>
        public ValidationResult Add(string errorMessage, params string[] memberNames)
        {
            var result = new ValidationResult(errorMessage, memberNames);
            Add(result);
            return result;
        }
    }
}
