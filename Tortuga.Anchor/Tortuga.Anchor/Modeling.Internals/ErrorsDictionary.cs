using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Tortuga.Anchor.Modeling.Internals
{
    internal class ErrorsDictionary
    {
        readonly Dictionary<string, IList<ValidationResult>> m_Errors = new Dictionary<string, IList<ValidationResult>>();

        /// <summary>
        /// Clears all errors
        /// </summary>
        /// <returns>
        /// Returns True if this actually removed any errors from the collection.
        /// </returns>
        public ErrorsDictionaryUpdateType Clear()
        {
            var result = HasErrors();
            m_Errors.Clear();
            if (result)
                return ErrorsDictionaryUpdateType.HasErrorsIsFalse;
            else
                return ErrorsDictionaryUpdateType.NoChange;
        }

        /// <summary>
        /// Gets all of the errors.
        /// </summary>
        public ReadOnlyCollection<ValidationResult> GetAllErrors()
        {
            List<ValidationResult> errors = new List<ValidationResult>();

            foreach (var item in m_Errors)
                errors.AddRange(item.Value);

            return new ReadOnlyCollection<ValidationResult>(errors.Distinct().ToList());
        }

        public bool HasErrors()
        {
            return m_Errors.Values.Any(list => list.Count > 0);
        }

        /// <summary>
        /// Sets the errors at the property level
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="errors">The errors.</param>
        /// <returns></returns>
        public ErrorsDictionaryUpdateType SetErrors(string propertyName, IList<ValidationResult> errors)
        {
            bool differences;

            if (m_Errors.ContainsKey(propertyName)) //check for differences
            {
                var oldErrors = m_Errors[propertyName];

                if (oldErrors.Count != errors.Count)
                    differences = true;
                else
                    differences = !oldErrors.SequenceEqual(errors, ValidationResultEqualityComparer.Default);
            }
            else
                differences = true;

            if (differences)
            {
                var oldHasErrors = HasErrors();
                m_Errors[propertyName] = errors;
                var newHasErrors = HasErrors();

                if (oldHasErrors == newHasErrors)
                    return ErrorsDictionaryUpdateType.ErrorsChanged;
                else if (newHasErrors)
                    return ErrorsDictionaryUpdateType.HasErrorsIsTrue;
                else
                    return ErrorsDictionaryUpdateType.HasErrorsIsFalse;
            }
            else
                return ErrorsDictionaryUpdateType.NoChange;
        }

        /// <summary>
        /// Set the errors at the object level
        /// </summary>
        /// <param name="errors">The errors.</param>
        /// <param name="affectedProperties">The affected properties.</param>
        /// <returns></returns>
        public ErrorsDictionaryUpdateType SetErrors(IList<ValidationResult> errors, out HashSet<string> affectedProperties)
        {
            bool differences;
            affectedProperties = new HashSet<string>();

            foreach (var p in (from err in errors from prop in err.MemberNames select prop))
                affectedProperties.Add(p);

            if (m_Errors.ContainsKey("")) //check for differences
            {
                var oldErrors = m_Errors[""];

                if (oldErrors.Count != errors.Count)
                    differences = true;
                else
                    differences = !oldErrors.SequenceEqual(errors, ValidationResultEqualityComparer.Default);

                foreach (var p in (from err in oldErrors from prop in err.MemberNames select prop))
                    affectedProperties.Add(p);
            }
            else
                differences = true;

            if (differences)
            {
                var oldHasErrors = HasErrors();
                m_Errors[""] = errors;
                var newHasErrors = HasErrors();

                if (oldHasErrors == newHasErrors)
                    return ErrorsDictionaryUpdateType.ErrorsChanged;
                else if (newHasErrors)
                    return ErrorsDictionaryUpdateType.HasErrorsIsTrue;
                else
                    return ErrorsDictionaryUpdateType.HasErrorsIsFalse;
            }
            else
                return ErrorsDictionaryUpdateType.NoChange;
        }

        /// <summary>
        /// Determines whether this instance has errors.
        /// </summary>
        /// <returns></returns>
        /// <summary>
        /// Gets the errors for a given property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>

        internal ReadOnlyCollection<ValidationResult> GetErrors(string propertyName)
        {
            if (propertyName == null)
                propertyName = "";

            IEnumerable<ValidationResult> errors;

            //Fetch the property-level errors
            if (m_Errors.ContainsKey(propertyName))
                errors = m_Errors[propertyName];
            else
                errors = Enumerable.Empty<ValidationResult>();

            //Add the object-level errors related to this property
            if (!string.IsNullOrEmpty(propertyName) && m_Errors.ContainsKey(""))
            {
                var objectErrors = m_Errors[""];
                errors = errors.Union(from e in objectErrors where e.MemberNames.Contains(propertyName) select e);
            }

            return new ReadOnlyCollection<ValidationResult>(errors.ToList());
        }
    }
}
