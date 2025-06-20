using System.Text.Json.Serialization;

namespace Matrix.MsSql.Unit
{
    /// <summary>
    /// This class provides properties and methods to represent a definition of a parameter requested by the test object.
    /// </summary>
    public class TestInput
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix.MsSql.Unit.TestInput"/>-class.
        /// </summary>
        /// <remarks>
        /// This constructor should be used only from the JSON-serialization-class.
        /// </remarks>
        public TestInput()
        {
            ParameterName = string.Empty;
            SqlType = string.Empty;
            Nullable = true;
            DefaultValue = false;
            UserValues = [];
        }

        /// <summary>
        /// Gets or sets the name of the parameter.
        /// </summary>
        /// <remarks>This property is required!</remarks>
        [JsonPropertyName("ParameterName")]
        [JsonPropertyOrder(-10)]
        [JsonRequired]
        public string ParameterName { get; set; }

        /// <summary>
        /// Gets or sets the SQL-Server parameter type.
        /// </summary>
        [JsonPropertyName("SqlType")]
        [JsonPropertyOrder(-9)]
        public string SqlType { get; set; }

        /// <summary>
        /// Gets or sets if a NULL-value is allowed for this parameter.
        /// </summary>
        [JsonPropertyName("Nullable")]
        [JsonPropertyOrder(-8)]
        public bool Nullable { get; set; }

        /// <summary>
        /// Gets or sets if a DEFAULT-value is available for this parameter.
        /// </summary>
        [JsonPropertyName("Default")]
        [JsonPropertyOrder(-7)]
        public bool DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets a list of user defined values to be tested additionally
        /// to the pre-defined test values according to the <see cref="Matrix.MsSql.Unit.TestInput.SqlType"/>.
        /// </summary>
        [JsonPropertyName("Inputs")]
        public System.Collections.ArrayList UserValues { get; set; }
    }
}