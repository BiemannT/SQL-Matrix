namespace Matrix.MsSql.Unit
{
    /// <summary>
    /// Represents the current state of one test case.
    /// </summary>
    public enum TestCaseStateEnum
    {
        /// <summary>
        /// The test case class is just initialized without test data. This is the default value.
        /// </summary>
        Initialized = 0,

        /// <summary>
        /// The test case is ready to be executed. All necessary test data are defined.
        /// </summary>
        Ready = 1,

        /// <summary>
        /// The test case is currently being executed.
        /// </summary>
        Running = 2,

        /// <summary>
        /// There are no expected values defined for this test case, but a test run is possible anyhow.
        /// </summary>
        MissingExpectedResult = 10,

        /// <summary>
        /// The definition of the input parameters for this test case are not valid. Test run is not possible.
        /// </summary>
        InvalidInputParameter = 11,

        /// <summary>
        /// An error occured during setting up the required data table. The test run is not possible.
        /// </summary>
        RequiredDataTableFailed = 12,

        /// <summary>
        /// The test object [SchemaName].[TestObjectName] does not exist on the database. Test run not possible.
        /// </summary>
        TestObjectNotExist = 13,

        /// <summary>
        /// The test case was executed successfully. The actual result is equal to the expected result.
        /// </summary>
        TestSuccessful = 100,

        /// <summary>
        /// The execution of this test case failed. The actual result is not equal to the expected result.
        /// </summary>
        TestFailed = 101,

        /// <summary>
        /// The execution of this test case was stopped due to the allowed command timeout.
        /// </summary>
        TestTimeout = 102,
    }
}