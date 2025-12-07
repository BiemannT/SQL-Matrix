using BiemannT.MUT.MsSql.Def.Common;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BiemannT.MUT.MsSql.Def.JSON
{
    /// <summary>
    /// This class provides properties and methods to interact with JSON-definition files.
    /// </summary>
    public class JsonDefinition : Definition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonDefinition"/> class with default values.
        /// </summary>
        public JsonDefinition() : base() { }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <remarks>
        /// <inheritdoc/>
        /// This property is required!
        /// </remarks>
        [JsonPropertyName("SQL-Matrix-Api-Version")]
        [JsonPropertyOrder(-10)]
        [JsonRequired]
        [JsonInclude]
        [JsonConverter(typeof(JsonApiVersionConverter))]
        public override Version ApiVersion
        {
            get => base.ApiVersion;
            protected set => base.ApiVersion = value;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <remarks>
        /// <inheritdoc/>
        /// This property is required!
        /// </remarks>
        [JsonPropertyName("SchemaName")]
        [JsonPropertyOrder(-9)]
        [JsonRequired]
        public override string SchemaName
        {
            get => base.SchemaName;
            set => base.SchemaName = value;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <remarks>
        /// <inheritdoc/>
        /// This property is required!
        /// </remarks>
        [JsonPropertyName("TestObjectName")]
        [JsonPropertyOrder(-8)]
        [JsonRequired]
        public override string ObjectName
        {
            get => base.ObjectName;
            set => base.ObjectName = value;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <remarks><inheritdoc/></remarks>
        [JsonPropertyName("MaxExecutionTime")]
        [JsonPropertyOrder(-7)]
        public override int MaxExecutionTime
        {
            get => base.MaxExecutionTime;
            set => base.MaxExecutionTime = value;
        }

        [JsonPropertyName("Inputs")]
        [JsonPropertyOrder(-6)]
        [JsonInclude]
        private List<JsonInput> JsonInputs
        {
            get
            {
                // Get Methode wird aufgerufen, wenn der JSON-Serializer die Inputs auslesen möchte.
                // Dabei müssen die Basisklassen-Input-Objekte in JsonInput-Objekte umgewandelt werden.
                List<JsonInput> inputs = [];
                foreach (var input in base.Inputs)
                {
                    if (input is JsonInput jsonInput)
                    {
                        // Falls es bereits ein JsonInput-Objekt ist, einfach hinzufügen
                        inputs.Add(jsonInput);
                    }
                    else
                    {
                        // Ansonsten ein neues JsonInput-Objekt erstellen und die Werte kopieren
                        JsonInput newJsonInput = new()
                        {
                            ParameterName = input.ParameterName,
                            SqlTypeDef = input.SqlTypeDef,
                            IsNullable = input.IsNullable,
                            HasDefaultValue = input.HasDefaultValue,
                            Direction = input.Direction
                        };
                        newJsonInput.UserValues.AddRange(input.UserValues);
                        inputs.Add(newJsonInput);
                    }
                }
                return inputs;
            }

            set
            {
                // Set Methode wird aufgerufen, wenn der JSON-Deserializer die Inputs setzen möchte.
                base.Inputs.Clear();
                foreach (var input in value)
                {
                    base.Inputs.Add(input);
                }
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <remarks><inheritdoc/></remarks>
        [JsonIgnore]
        public override List<Input> Inputs { get => base.Inputs; }

        /// <summary>
        /// Gets or sets the name of the JSON-definition file.
        /// </summary>
        [JsonIgnore]
        public FileInfo? FileName { get; set; }

        [JsonPropertyName("ExpectedResults")]
        [JsonPropertyOrder(0)]
        [JsonInclude]
        private List<JsonExpectedResult> _expectedResults;

        /// <summary>
        /// Gets the collection of expected results.
        /// </summary>
        [JsonIgnore]
        public override List<ExpectedResult> ExpectedResults
        {
            get => [.. _expectedResults.Cast<ExpectedResult>()];
        }

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
                this.MaxExecutionTime = _definition.MaxExecutionTime;
                this.Inputs.AddRange(_definition.Inputs);
                this._expectedResults = _definition._expectedResults;
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
