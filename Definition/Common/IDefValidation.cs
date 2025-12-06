using System.Collections.ObjectModel;

namespace BiemannT.MUT.MsSql.Def.Common
{
    /// <summary>
    /// Defines a method that validates the current object and returns the results of the validation.
    /// </summary>
    public interface IDefValidation
    {
        /// <summary>
        /// Validates the current object and returns a collection of validation results.
        /// </summary>
        /// <returns>
        /// A read-only collection of <see cref="ValidationResult"/> objects that describe any validation errors.
        /// The collection is empty if the object is valid.
        /// </returns>
        ReadOnlyCollection<ValidationResult> Validate();
    }
}
