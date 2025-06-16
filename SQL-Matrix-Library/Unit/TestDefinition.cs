using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Dac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace Matrix.MsSql.Unit
{
    /// <summary>
    /// This class provides properties and methods to represent the JSON-test file.
    /// </summary>
    public class TestDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix.MsSql.Unit.TestDefinition"/>-class.
        /// </summary>
        /// <remarks>
        /// This constructor should be used only from the JSON-serialization-class.
        /// </remarks>
        public TestDefinition()
        {
            // Initialize the class with default values.
            ApiVersion = ApiVersion.None;
            TestObjectType = TestObjectTypeEnum.None;
            SchemaName = string.Empty;
            TestObjectName = string.Empty;
            MaxExecutionTime = 1;
            Inputs = [];
            FileName = new("test.json");
            SqlConn = new();
            DropDatabaseBeforePublish = false;
            KeepDatabase = KeepDatabaseEnum.DropAlways;
        }

        /// <summary>
        /// Gets or sets the used API-version of this librabry.
        /// </summary>
        /// <remarks>This property is required!</remarks>
        [JsonPropertyName("SQL-Matrix-Api-Version")]
        [JsonPropertyOrder(-10)]
        [JsonRequired]
        public ApiVersion ApiVersion { get; set; }

        /// <summary>
        /// Gets or sets the type of the tested object.
        /// </summary>
        [JsonPropertyName("TestObjectType")]
        [JsonPropertyOrder(-9)]
        [JsonRequired]
        public TestObjectTypeEnum TestObjectType { get; set; }

        /// <summary>
        /// Gets or sets the schema name of the tested object.
        /// </summary>
        /// <remarks>This propety is required!</remarks>
        [JsonPropertyName("SchemaName")]
        [JsonPropertyOrder(-8)]
        [JsonRequired]
        public string SchemaName { get; set; }

        /// <summary>
        /// Gets or sets the name of the tested object.
        /// </summary>
        /// <remarks>This property is required!</remarks>
        [JsonPropertyName("TestObjectName")]
        [JsonPropertyOrder(-7)]
        [JsonRequired]
        public string TestObjectName { get; set; }

        /// <summary>
        /// Gets or sets the maximum allowed execution time on the SQL-Server for this test in Seconds.
        /// If the execution time of the test exceeds this time the test will fail.
        /// The standard value is 1 s.
        /// The minimum value is 1 s.
        /// </summary>
        [JsonPropertyName("MaxExecutionTime")]
        [JsonPropertyOrder(-6)]
        public int MaxExecutionTime { get; set; }

        /// <summary>
        /// Gets or set the definitions of the requested parameters for the test object.
        /// </summary>
        [JsonPropertyName("Inputs")]
        public List<TestInput> Inputs { get; set; }

        /// <summary>
        /// Gets or sets the name of the test file.
        /// </summary>
        /// <remarks>The default name is "test.json" located in the current working directory.</remarks>
        [JsonIgnore]
        public FileInfo FileName { get; set; }

        /// <summary>
        /// Gets or sets the connection to the Test SQL-Server.
        /// </summary>
        /// <remarks>
        /// The selected User login should have enough permissions to execute the test for the selected test object.
        /// In case of using a dacpac-file the selected User login should have the permission to create a database.
        /// </remarks>
        [JsonIgnore]
        public SqlConnection SqlConn { get; set; }

        /// <summary>
        /// Gets or sets the dacpac-file to be used for this test.
        /// The database will be published to the SQL-Server in advance of the test.
        /// </summary>
        /// <remarks>
        /// If a dacpac file is set, the databse property inside the <see cref="SqlConn"/>-property will be replaced.
        /// The name of the new database will be the application name of the dacpac plus '-test'.
        /// It is recommended to perform the test under an empty and clean database, by using a dacpac-package.
        /// If the property <see cref="DropDatabaseBeforePublish"/> is set to <c>true</c>, the database will be published new again in advance of each test run.
        /// </remarks>
        [JsonIgnore]
        public FileInfo? DacpacFile { get; set; }

        /// <summary>
        /// <para>
        /// Gets or sets the instruction for dropping the database before publishing the dacpac-file to the SQL-Server,
        /// if already the test database exists.
        /// </para>
        /// <para>
        /// Set <c>true</c>, if the database should be dropped first before each test run and then a clean database will be published.
        /// Set <c>false</c>, if the existing database should be reused for this test.
        /// In this case an update from the dacpac-file will be performed.
        /// The default value is <c>false</c>.
        /// </para>
        /// </summary>
        /// <remarks>
        /// This property will be affected only, if a <see cref="DacpacFile"/> is set.
        /// </remarks>
        [JsonIgnore]
        public bool DropDatabaseBeforePublish { get; set; }

        /// <summary>
        /// <para>
        /// Gets or sets the instruction, if the database should be kept after the test run is finished or failed.
        /// </para>
        /// <para>
        /// The default value is <see cref="KeepDatabaseEnum.DropAlways"/>.
        /// </para>
        /// </summary>
        /// <remarks>
        /// This property will be affected only, if a <see cref="DacpacFile"/> is set.
        /// </remarks>
        [JsonIgnore]
        public KeepDatabaseEnum KeepDatabase { get; set; }
    }
}