using Matrix.MsSql.Common;
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
            TestCases = new([]);
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

        /// <summary>
        /// Gets the collection of all possible test cases.
        /// </summary>
        /// <remarks>Execute <see cref="BuildTestCases"/> method before getting the collection. Otherwise the collection is empty.</remarks>
        [JsonIgnore]
        public TestCaseCollection TestCases { get; private set; }

        // TODO: Load-Prozedur erstellen
        // Statische Prozedur um eine neue Testdefinitions Instanz zu erstellen

        // TODO: Save-Prozedur erstellen
        // Instanz Prozedur erstellen, um den Inhalt dieser Instanz in einer Testdefinitionsdatei zu speichern.

        /// <summary>
        /// Generates the test cases based on the defined input parameters.
        /// </summary>
        /// <remarks>This method processes the input parameters of the instance to create a comprehensive
        /// set of test cases. Each test case represents a unique combination of parameter values.
        /// If the test definition has no input parameters, a single default test case is created.
        /// The generated test cases are stored in the <see cref="TestCases"/> property.</remarks>
        /// <exception cref="InvalidOperationException">Thrown if an error occurs while analyzing the input parameters.</exception>
        public void BuildTestCases()
        {
            TestCaseParameter[][] parameterSet = new TestCaseParameter[this.Inputs.Count][];
            List<TestCase> testCases = [];
            Combinator combinator = new();

            // Alle Inputs dieser Instanz durchlaufen und die TestCaseParameter generieren.
            // In der ersten Dimension sind alle Parameter enthalten und in der zweiten Dimension alle Werte.

            for (int i = 0; i < this.Inputs.Count; i++)
            {
                try
                {
                    parameterSet[i] = TestCaseParameter.GenerateParameters(this.Inputs[i]);
                    combinator.AddDimension(parameterSet[i].Length);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"An error occured during analyzing the parameter {this.Inputs[i].ParameterName}.", ex);
                }
            }

            // Parameter kombinieren und damit Testfälle erstellen
            if (combinator.TotalCombinations == 0)
            {
                // Nur ein Testfall
                testCases.Add(new TestCase() { 
                    ExecutionTimeout = this.MaxExecutionTime,
                    TestObjectName = string.Concat(this.SchemaName, ".", this.TestObjectName)
                });
            }
            else
            {
                // Mehrere Testfälle
                foreach (int[] combination in combinator)
                {
                    TestCaseParameter[] parameters = new TestCaseParameter[combination.Length];

                    for (int j = 0; j < combination.Length; j++)
                    {
                        parameters[j] = parameterSet[j][combination[j]];
                    }

                    testCases.Add(new TestCase(parameters)
                    {
                        ExecutionTimeout = this.MaxExecutionTime,
                        TestObjectName = string.Concat(this.SchemaName, ".", this.TestObjectName)
                    });
                }
            }

            // Testfälle in der Eigenschaft speichern
            this.TestCases = new(testCases);
        }
    }
}