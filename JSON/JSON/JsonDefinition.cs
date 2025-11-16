using System.Text.Json;
using System.Text.Json.Serialization;

namespace BiemannT.MUT.MsSql.Def.JSON
{
    /// <summary>
    /// This class provides properties and methods to interact with JSON-definition files.
    /// </summary>
    public class JsonDefinition : Common.Definition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonDefinition"/> class with default values.
        /// </summary>
        public JsonDefinition() {
            // Initialize the class with default values.
            _apiVersionJson = "v1";
            SchemaName = string.Empty;
            ObjectName = string.Empty;
            _maxExecutionTime = 10;
            _inputs = [];
        }

        [JsonPropertyName("SQL-Matrix-Api-Version")]
        [JsonPropertyOrder(-10)]
        [JsonRequired]
        [JsonInclude]
        private string _apiVersionJson;

        /// <summary>
        /// Gets or sets the API version for the current instance.
        /// </summary>
        /// <value>The API version. The default value is 1.0</value>
        /// <remarks>This property is required!</remarks>
        /// <exception cref="NotSupportedException">Currently version 1.0 is supported.</exception>
        [JsonIgnore]
        public override Version ApiVersion
        {
            get => _apiVersionJson switch
            {
                "v1" => new Version(1, 0),
                _ => throw new NotSupportedException($"The API-Version '{_apiVersionJson}' is not supported."),
            };

            protected set
            {
                _apiVersionJson = value.Major switch
                {
                    1 => "v1",
                    _ => throw new NotSupportedException($"The API-Version '{value}' is not supported."),
                };
            }
            
        }

        /// <summary>
        /// Gets or sets the schema name of the tested object.
        /// </summary>
        /// <value>The schema name of the tested object. The default value is <see cref="string.Empty"/>.</value>
        /// <remarks>This property is required!</remarks>
        [JsonPropertyName("SchemaName")]
        [JsonPropertyOrder(-9)]
        [JsonRequired]
        public override string SchemaName { get; set; }

        /// <summary>
        /// Gets or sets the name of the tested object.
        /// </summary>
        /// <value>The name of the tested object. The default value is <see cref="string.Empty"/>.</value>
        /// <remarks>This property is required!</remarks>
        [JsonPropertyName("TestObjectName")]
        [JsonPropertyOrder(-8)]
        [JsonRequired]
        public override string ObjectName { get; set; }

        [JsonPropertyName("MaxExecutionTime")]
        [JsonPropertyOrder(-7)]
        [JsonInclude]
        private int _maxExecutionTime;

        /// <summary>
        /// Gets or sets the maximum execution time for each test case in seconds.
        /// </summary>
        /// <value>
        /// The maximum execution time in seconds.
        /// The default value is 10 seconds. The value 0 means infinite execution time.
        /// It is not recommended to set the value to 0 for test cases as the test case can freeze the overall testing.
        /// </value>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value is less than zero.</exception>"
        [JsonIgnore]
        public override int MaxExecutionTime
        {
            get => _maxExecutionTime;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "MaxExecutionTime must be a positive integer.");
                }
                _maxExecutionTime = value;
            }
        }

        [JsonPropertyName("Inputs")]
        [JsonPropertyOrder(-6)]
        [JsonInclude]
        private List<JsonInput> _inputs;

        /// <summary>
        /// Gets the collection of input parameters.
        /// </summary>
        [JsonIgnore]
        public override List<Common.Input> Inputs
        {
            get => [.. _inputs.Cast<Common.Input>()];
        }

        /// <summary>
        /// Gets or sets the name of the JSON-definition file.
        /// </summary>
        [JsonIgnore]
        public FileInfo? FileName { get; set; }

        public override void Load()
        {
            // Prüfen, ob ein Dateiname angegeben ist
            if (FileName == null)
            {
                throw new InvalidOperationException("FileName is not set.");
            }

            try
            {
                // Zunächst in eine lokale Variable des gleichen Typs deserialisieren
                JsonDefinition _definition;
                _definition = JsonSerializer.Deserialize<JsonDefinition>(FileName.Open(FileMode.Open, FileAccess.Read)) ?? throw new InvalidOperationException("Failure during reading the test file occured. Deserialization returned null.");

                // Wenn erfolgreich, die Werte in die aktuelle Instanz übernehmen
                this._apiVersionJson = _definition._apiVersionJson;
                this.SchemaName = _definition.SchemaName;
                this.ObjectName = _definition.ObjectName;
                this._maxExecutionTime = _definition._maxExecutionTime;
                this._inputs = _definition._inputs;
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException("The test file definition contains invalid JSON code - or - required fields are missing!", ex);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override void Save()
        {
            throw new NotImplementedException();
        }
    }
}
