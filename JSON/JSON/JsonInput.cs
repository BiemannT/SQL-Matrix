using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BiemannT.MUT.MsSql.Def.JSON
{
    /// <summary>
    /// This class provides properties and methods to represent a definition of a parameter requested by the test object in JSON-format.
    /// </summary>
    public class JsonInput : Common.Input
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonInput"/>-class.
        /// </summary>
        public JsonInput()
        {
            ParameterName = string.Empty;
            _sqlTypeDefinition = string.Empty;
            IsNullable = true;
            HasDefaultValue = false;
            Direction = ParameterDirection.Input;
            UserValues = [];
            UserValuesJson = [];
        }

        /// <summary>
        /// Gets or sets the name of the parameter.
        /// </summary>
        [JsonPropertyName("ParameterName")]
        [JsonPropertyOrder(-10)]
        [JsonRequired]
        public override string ParameterName { get; set; }

        private string _sqlTypeDefinition;

        /// <summary>
        /// Gets or sets the SQL-Server parameter type.
        /// </summary>
        /// <remarks>If the parameter type will be changed the <see cref="UserValues"/>-list will be cleared.</remarks>
        [JsonPropertyName("SqlType")]
        [JsonPropertyOrder(-9)]
        [JsonRequired]
        public override string SqlTypeDefinition
        {
            get => _sqlTypeDefinition;
            set
            {
                // Liste mit benutzerdefinierten Werten zurücksetzen wenn der Parameter-Typ geändert wird.
                if (this._sqlTypeDefinition != value) this.UserValues.Clear();

                _sqlTypeDefinition = value;
                // SQL-Typ analysieren
                try
                {
                    base.IdentifySqlType();
                }
                catch
                {
                    // Fehler bei der Analyse ignorieren
                    // Im Fehlerfall bleibt der SqlType auf NotSupported
                    // TODO: Mögliche Fehlermeldungen in die Ausgabe integrieren
                }
            }
        }

        /// <summary>
        /// Gets or sets if a NULL-value is allowed for this parameter.
        /// </summary>
        /// <remarks>Default is <see langword="true"/>.</remarks>
        [JsonPropertyName("Nullable")]
        [JsonPropertyOrder(-8)]
        public override bool IsNullable { get; set; }

        /// <summary>
        /// Gets or sets if a DEFAULT-value is available for this parameter.
        /// </summary>
        /// <remarks>Default is <see langword="false"/>.</remarks>
        [JsonPropertyName("Default")]
        [JsonPropertyOrder(-7)]
        public override bool HasDefaultValue { get; set; }

        /// <summary>
        /// Gets or sets the direction of the parameter.
        /// </summary>
        /// <remarks>Default is <see cref="ParameterDirection.Input"/>.</remarks>
        [JsonPropertyName("Direction")]
        [JsonPropertyOrder(-6)]
        public override ParameterDirection Direction { get; set; }

        /// <summary>
        /// Gets a list of user defined values to be tested additionally
        /// to the pre-defined test values according to the <see cref="JsonInput.SqlTypeDefinition"/>.
        /// </summary>
        /// <remarks>
        /// Each value shall be of a .NET type compatible to the current <see cref="JsonInput.SqlTypeDefinition"/>.
        /// The list will be cleared if the <see cref="JsonInput.SqlTypeDefinition"/> changed.
        /// User values will be ignored, if the <see cref="Direction"/> is not <see cref="ParameterDirection.Input"/>.
        /// </remarks>
        [JsonIgnore]
        public override List<object> UserValues { get; }

        /// <summary>
        /// Internal representaiton of <see cref="UserValues"/> to interact with the JSON-file.
        /// The getter will return a list of <see cref="JsonElement"/> with JSON compatible value kinds.
        /// The setter will convert each user value in a compatible .NET type and store the value into the <see cref="UserValues"/>-list.
        /// </summary>
        [JsonPropertyName("UserValues")]
        [JsonPropertyOrder(-5)]
        [JsonInclude]
        private List<JsonElement> UserValuesJson
        {
            get
            {
                var jsonElements = new List<JsonElement>();
                foreach (var value in UserValues)
                {
                    var json = JsonSerializer.SerializeToElement(value);
                    // TODO: Verwendung dieser FUnktion nochmal überprüfen
                    jsonElements.Add(json);
                }
                return jsonElements;
            }

            set
            {
                // Der setter wird nur während der Deserialisierung aufgerufen.
                // Die JsonElement-Liste in die UserValues-Liste umwandeln und dabei prüfen, ob die Werte zum definierten SQL-Typ passen.
                // Ungültige Werte werden ignoriert.
                // Zudem sollen null-Werte, sowie true und false ignoriert werden, da diese bereits bei den eingebauten Testwerten enthalten sind.
                foreach (var jsonElement in value)
                {
                    if (jsonElement.ValueKind == JsonValueKind.Null || jsonElement.ValueKind == JsonValueKind.False || jsonElement.ValueKind == JsonValueKind.True) continue;

                    try
                    {
                        UserValues.Add(JsonDataConverter.JsonToSql(jsonElement, SqlType));
                    }
                    catch
                    {
                        //TODO: Mögliche Fehlermeldungen im Ausgabedokument ausgeben
                    }
                }

            }
        }
    }
}
