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
            ApiVersion = ApiVersionEnum.None;
            TestObjectType = TestObjectTypeEnum.None;
            SchemaName = string.Empty;
            TestObjectName = string.Empty;
            MaxExecutionTime = 1;
            Inputs = [];
            FileName = new("test.json");
        }

        /// <summary>
        /// Gets or sets the used API-version of this librabry.
        /// </summary>
        /// <remarks>This property is required!</remarks>
        [JsonPropertyName("SQL-Matrix-Api-Version")]
        [JsonPropertyOrder(-10)]
        [JsonRequired]
        public ApiVersionEnum ApiVersion { get; set; }

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
    }
}