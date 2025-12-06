namespace BiemannT.MUT.MsSql.Def.Common
{
    /// <summary>
    /// Specifies the severity level of a validation result.
    /// </summary>
    public enum ValidationResultSeverity
    {
        /// <summary>
        /// Represents an informational validation message.
        /// </summary>
        Info,

        /// <summary>
        /// Represents a warning message for a validation issue.
        /// </summary>
        Warning,

        /// <summary>
        /// Represents an error message for a validation failure.
        /// The test execution will be not possible in this case.
        /// </summary>
        Error
    }
}
