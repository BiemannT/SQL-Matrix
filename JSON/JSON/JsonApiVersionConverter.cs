using System.Text.Json;
using System.Text.Json.Serialization;

namespace BiemannT.MUT.MsSql.Def.JSON
{
    /// <summary>
    /// Provides custom JSON serialization and deserialization for API version values, mapping between JSON string
    /// representations and <see cref="Version"/> objects.
    /// </summary>
    public class JsonApiVersionConverter : JsonConverter<Version>
    {
        /// <summary>
        /// Reads a JSON string representing an API version and converts it to a corresponding <see cref="Version"/> object.
        /// </summary>
        /// <remarks>
        /// Only the string value "v1" is supported.
        /// Any other value will result in a <see cref="JsonException"/>.
        /// </remarks>
        /// <param name="reader">The <see cref="Utf8JsonReader"/> positioned at the JSON token to read. Must be at a string token
        /// representing the API version.</param>
        /// <param name="typeToConvert">The type of the object to convert. This parameter is ignored; only <see cref="Version"/> is supported.</param>
        /// <param name="options">Options to control the behavior of the JSON serializer. This parameter is not used in this implementation.</param>
        /// <returns>A <see cref="Version"/> object corresponding to the API version specified in the JSON string.</returns>
        /// <exception cref="JsonException">Thrown if the JSON token is not a string or if the string does not represent a supported API version.</exception>
        public override Version Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Version wird als String erwartet
            if (reader.TokenType == JsonTokenType.String)
            {
                string versionString = reader.GetString() ?? string.Empty;
                return versionString.ToLower() switch
                {
                    "v1" => new Version(1, 0),
                    _ => throw new JsonException($"Unsupported API version: {versionString}"),
                };
            }
            else
            {
                throw new JsonException("Expected a string value for SQL-Matrix-Api-Version.");
            }
        }

        /// <summary>
        /// Writes the specified API version as a JSON string using the provided writer.
        /// </summary>
        /// <remarks>Only API version 1.0 is supported.
        /// Attempting to serialize other versions will result in an exception.</remarks>
        /// <param name="writer">The writer to which the JSON string representation of the API version will be written.</param>
        /// <param name="value">The API version to serialize. Only versions with Major equal to 1 and Minor equal to 0 are supported.</param>
        /// <param name="options">Options to control the serialization behavior. This parameter is required but not used in this
        /// implementation.</param>
        /// <exception cref="JsonException">Thrown if the specified API version is not supported.</exception>
        public override void Write(Utf8JsonWriter writer, Version value, JsonSerializerOptions options)
        {
            string versionString = value switch
            {
                { Major: 1, Minor: 0 } => "v1",
                _ => throw new JsonException($"Unsupported API version: {value}"),
            };
            writer.WriteStringValue(versionString);
        }
    }
}
