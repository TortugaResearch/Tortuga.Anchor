using System.Collections.Generic;

namespace Tortuga.Anchor
{
    /// <summary>
    /// Class ValidationResult.
    /// </summary>
    public class ValidationResult
    {
        private IEnumerable<string> m_MemberNames;

        /// <summary>
        /// The success
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public static readonly ValidationResult Success;


        ///// <summary>
        ///// Initializes a new instance of the <see cref="ValidationResult"/> class.
        ///// </summary>
        ///// <param name="validationResult">The validation result.</param>
        ///// <exception cref="System.ArgumentNullException">validationResult</exception>
        //protected ValidationResult(ValidationResult validationResult)
        //{
        //    if (validationResult == null)
        //    {
        //        throw new ArgumentNullException("validationResult");
        //    }
        //    this.ErrorMessage = validationResult.ErrorMessage;
        //    this._memberNames = validationResult._memberNames;
        //}


        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationResult"/> class.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        public ValidationResult(string errorMessage) : this(errorMessage, null)
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationResult"/> class.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="memberNames">The member names.</param>
        public ValidationResult(string errorMessage, IEnumerable<string> memberNames)
        {
            ErrorMessage = errorMessage;
            m_MemberNames = memberNames.AsList() ?? new string[0];
        }


        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public override string ToString()
        {
            return (ErrorMessage ?? base.ToString());
        }


        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        /// <value>The error message.</value>
        public string ErrorMessage { get; set; }


        /// <summary>
        /// Gets the member names.
        /// </summary>
        /// <value>The member names.</value>
        public IEnumerable<string> MemberNames
        {

            get { return m_MemberNames; }
        }
    }
}
