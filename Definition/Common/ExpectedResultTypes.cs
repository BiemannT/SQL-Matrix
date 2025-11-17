namespace BiemannT.MUT.MsSql.Def.Common
{
    /// <summary>
    /// This enumeration represents possible types of expected test results.
    /// </summary>
    public enum ExpectedResultTypes
    {
        /// <summary>
        /// Indicates that the test will return a single value.
        /// </summary>
        Value = 1,

        /// <summary>
        /// Indicates that the test will return one or more result sets.
        /// </summary>
        Resultset = 2,

        /// <summary>
        /// Indicates that the test will end with an expected exception.
        /// </summary>
        Exception = 3,
    }
}
