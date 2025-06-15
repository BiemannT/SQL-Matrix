using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Matrix.MsSql.Unit
{
    /// <summary>
    /// Represents the instruction, if the database sould be kept after the complete test run.
    /// </summary>
    public enum KeepDatabaseEnum
    {
        /// <summary>
        /// Drops the test databse after the test run in any case.
        /// </summary>
        DropAlways = 0,

        /// <summary>
        /// Keeps the test databse only in case if one test fails. If all test runs are successful the test database will be dropped.
        /// </summary>
        /// <remarks>
        /// In case of a failed test, the test scenario can be repeated on the database manually.
        /// Therefore the test data of the failed run will be kept in the data tables.
        /// </remarks>
        KeepOnFailure = 1,

        /// <summary>
        /// Keeps the test database after the test run in any case.
        /// </summary>
        /// <remarks>
        /// Test data from the data tables will be removed.
        /// </remarks>
        KeepAlways = 2
    }
}