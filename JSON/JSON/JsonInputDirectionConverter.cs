using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BiemannT.MUT.MsSql.Def.JSON
{
    /// <summary>
    /// Converts between JSON string representations and <see cref="ParameterDirection"/> values
    /// for serialization and deserialization.
    /// </summary>
    /// <remarks>
    /// Supports only the string values "INPUT" and "OUTPUT" (case-insensitive) when reading from JSON.
    /// </remarks>
    public class JsonInputDirectionConverter : JsonConverter<ParameterDirection>
    {
        /// <summary>
        /// Reads a JSON string value and converts it to a corresponding <see cref="ParameterDirection"/> value.
        /// </summary>
        /// <remarks>Only the string values "INPUT" and "OUTPUT" (case-insensitive) are supported. Any
        /// other value, including null or unrecognized strings, defaults to <see cref="ParameterDirection.Input"/>.</remarks>
        /// <param name="reader">The reader positioned at the JSON token to read. The reader's current token must be a <see langword="string"/> representing
        /// the parameter direction.</param>
        /// <param name="typeToConvert">The type of the object to convert. This parameter is not used.</param>
        /// <param name="options">Options to control the behavior of the JSON serializer. This parameter is not used.</param>
        /// <returns>A <see cref="ParameterDirection"/> value corresponding to the JSON string. Returns <see cref="ParameterDirection.Input"/> for
        /// unrecognized or missing values.</returns>
        /// <exception cref="JsonException">Thrown if the current JSON token is not a <see langword="string"/>.</exception>
        public override ParameterDirection Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Direction als String erwartet
            if (reader.TokenType == JsonTokenType.String)
            {
                string directionString = reader.GetString() ?? string.Empty;
                return directionString.ToUpper() switch
                {
                    // Nur Input und Output unterstützt
                    "INPUT" => ParameterDirection.Input,
                    "OUTPUT" => ParameterDirection.Output,
                    _ => ParameterDirection.Input
                };
            }
            else
            {
                throw new JsonException("Expected a string value for input parameter direction.");
            }
        }

        /// <summary>
        /// Writes the JSON string representation of the specified <see cref="ParameterDirection"/> value using the
        /// provided <see cref="Utf8JsonWriter"/>.
        /// </summary>
        /// <remarks>The <see cref="ParameterDirection.Input"/> value is written as "INPUT" and <see
        /// cref="ParameterDirection.Output"/> as "OUTPUT". Any other value is treated as <see
        /// cref="ParameterDirection.Input"/> and written as "INPUT".</remarks>
        /// <param name="writer">The <see cref="Utf8JsonWriter"/> to which the JSON string value will be written.</param>
        /// <param name="value">The <see cref="ParameterDirection"/> value to convert and write as a JSON string.</param>
        /// <param name="options">Options to control the behavior of the JSON serialization. This parameter is not used by this method.</param>
        public override void Write(Utf8JsonWriter writer, ParameterDirection value, JsonSerializerOptions options)
        {
            string directionString = value switch
            {
                ParameterDirection.Input => "INPUT",
                ParameterDirection.Output => "OUTPUT",
                _ => "INPUT"
            };
            writer.WriteStringValue(directionString);
        }
    }
}
