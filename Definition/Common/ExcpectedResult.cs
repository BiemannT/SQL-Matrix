namespace BiemannT.MUT.MsSql.Def.Common
{
    /// <summary>
    /// This class provides the base properties and methods to represent one expected result of the test.
    /// </summary>
    /// <remarks>All definition formats must inherit from this class.</remarks>
    public abstract class ExcpectedResult
    {
        /// <summary>
        /// Gets or sets the type of this expected result.
        /// </summary>
        public abstract ExpectedResultTypes ResultType { get; set; }

        /// <summary>
        /// Gets or sets the error number returned from the test, if an exception is expected.
        /// </summary>
        /// <remarks>
        /// This property is required, if <see cref="ResultType"/> is <see cref="ExpectedResultTypes.Exception"/>.
        /// Otherwise this property is ignored and the value is <see langword="null"/>.
        /// </remarks>
        public abstract int? ErrorNumber { get; set; }

        /// <summary>
        /// Gets or sets the value from the test, if a single value is expected.
        /// </summary>
        /// <remarks>
        /// This property is required, if <see cref="ResultType"/> is <see cref="ExpectedResultTypes.Value"/>.
        /// Otherwise this property is ignored and the value is <see langword="null"/>.
        /// </remarks>
        public abstract object? Value { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of the last time this test was run,
        /// regardless of whether the test passed or failed.
        /// </summary>
        /// <value>
        /// Returns a <see cref="DateTime"/> object if the test was already executed,
        /// otherwise <see langword="null"/>.
        /// </value>
        public abstract DateTime? LastExecution { get; set; }

        /// <summary>
        /// Gets or sets the duration of the last time this test was run,
        /// regardless of whether the test passed or failed.
        /// </summary>
        /// <value>
        /// Returns a <see cref="TimeSpan"/> object if the test was already executed,
        /// otherwise <see langword="null"/>.
        /// </value>
        public abstract TimeSpan? LastDuration { get; set; }
    }
}
