using BiemannT.MUT.MsSql.Def.Common;
using System.Text.Json.Serialization;

namespace BiemannT.MUT.MsSql.Def.JSON
{
    /// <summary>
    /// This class provides properties and methods to represent one expected result of the test in JSON-format.
    /// </summary>
    public class JsonExpectedResult : ExcpectedResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonExpectedResult"/>-class.
        /// </summary>
        public JsonExpectedResult() { }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        [JsonPropertyName("ExpectedResultType")]
        [JsonPropertyOrder(0)]
        [JsonConverter(typeof(JsonStringEnumConverter<ExpectedResultTypes>))]
        public override ExpectedResultTypes ResultType { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <remarks><inheritdoc/></remarks>
        [JsonPropertyName("ExpectedErrorNumber")]
        [JsonPropertyOrder(1)]
        public override int? ErrorNumber { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <remarks><inheritdoc/></remarks>
        // TODO: Converter implementieren.
        // Beispiel: https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/converters-how-to#deserialize-inferred-types-to-object-properties
        [JsonPropertyName("ExpectedValue")]
        [JsonPropertyOrder(2)]
        public override object? Value { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <value><inheritdoc/></value>
        [JsonPropertyName("LastExecutionDateTime")]
        [JsonPropertyOrder(3)]
        public override DateTime? LastExecution { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <value><inheritdoc/></value>
        [JsonPropertyName("LastExecutionDuration")]
        [JsonPropertyOrder(4)]
        public override TimeSpan? LastDuration { get; set; }
    }
}
