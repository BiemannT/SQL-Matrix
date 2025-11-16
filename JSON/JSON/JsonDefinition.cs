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
            ApiVersion = new(1, 0);
            SchemaName = string.Empty;
            ObjectName = string.Empty;
            _maxExecutionTime = 10;
            _inputs = [];
        }

        /// <summary>
        /// Internal representation for the JSON Property of the API-Version.
        /// </summary>
        /// <exception cref="NotSupportedException">Will be thrown in case of an invalid version number.</exception>
        [JsonPropertyName("SQL-Matrix-Api-Version")]
        [JsonPropertyOrder(-10)]
        [JsonRequired]
        [JsonInclude]
        private string ApiVersionJson
        {
            get
            {
                return ApiVersion switch
                {
                    { Major: 1, Minor: 0 } => "v1",
                    _ => string.Empty
                };
            }

            set
            {
                ApiVersion = value switch
                {
                    "v1" => new(1, 0),
                    _ => throw new NotSupportedException($"The API-Version '{value}' is not supported.")
                };
            }
        }

        /// <summary>
        /// Gets the API version for the current instance.
        /// </summary>
        /// <value>The API version. The default value is 1.0</value>
        [JsonIgnore]
        public override Version ApiVersion { get; protected set; }

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
        [JsonPropertyName("MaxExecutionTime")]
        [JsonPropertyOrder(-7)]
        public override int MaxExecutionTime
        {
            get => _maxExecutionTime;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(MaxExecutionTime), value, "MaxExecutionTime must be a positive integer.");
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

        /// <summary>
        /// Loads the content of the JSON definition file, defined in <see cref="FileName"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">Will be thrown, if no <see cref="FileName"/> is defined - OR - the content of the file is not valid - OR - the required properties are not defined in the file.</exception>
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
                this.ApiVersion = _definition.ApiVersion;
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
