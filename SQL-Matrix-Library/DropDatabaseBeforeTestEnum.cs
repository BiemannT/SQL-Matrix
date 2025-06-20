namespace Matrix.MsSql
{
    /// <summary>
    /// Represents the instruction on how an existing database should be used or created before starting the tests.
    /// </summary>
    public enum DropDatabaseBeforeTestEnum
    {
        /// <summary>
        /// Throw an exception, if the database name already exists on the SQL-Server.
        /// </summary>
        ExceptionIfExists = 0,

        /// <summary>
        /// Use the existing database if exists, or create a new database.
        /// </summary>
        UseExistingOrCreate = 1,

        /// <summary>
        /// The database will be dropped first - if existing - and a new database will be created.
        /// </summary>
        DropExistingAndCreate = 2
    }
}