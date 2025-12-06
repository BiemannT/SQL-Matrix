namespace BiemannT.MUT.MsSql.Def.Common
{
    /// <summary>
    /// Represents the result of a validation operation, including severity, the related property, and a descriptive
    /// message.
    /// </summary>
    /// <param name="severity">The severity level of the validation result, indicating the importance or impact of the validation finding.</param>
    /// <param name="property">The name of the property associated with the validation result. Can be null or empty if the result is not
    /// property-specific.</param>
    /// <param name="message">A descriptive message explaining the validation result.</param>
    public class ValidationResult(ValidationResultSeverity severity, string property, string message)
    {
        /// <summary>
        /// Gets the severity level associated with this validation result.
        /// </summary>
        public ValidationResultSeverity Severity { get; } = severity;

        /// <summary>
        /// Gets the name of the property related to this validation result.
        /// </summary>
        public string PropertyName { get; } = property;

        /// <summary>
        /// Gets the message of the validation result.
        /// </summary>
        public string Message { get; } = message;

        /// <summary>
        /// Returns a string that represents the current object, including severity,
        /// property name (if available), and message.
        /// </summary>
        /// <returns>
        /// A string containing the severity and message and optional the property name.
        /// </returns>
        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(PropertyName))
            {
                return $"{Severity}: {Message}";
            }

            return $"{Severity}: Property '{PropertyName}': {Message}";
        }
    }
}
