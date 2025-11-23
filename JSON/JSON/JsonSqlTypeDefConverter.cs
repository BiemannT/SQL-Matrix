using BiemannT.MUT.MsSql.Def.Common;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BiemannT.MUT.MsSql.Def.JSON
{
    /// <summary>
    /// Provides a custom JSON converter for serializing and deserializing <see cref="SqlTypeDefinition"/> objects as
    /// JSON strings.
    /// </summary>
    public class JsonSqlTypeDefConverter : JsonConverter<SqlTypeDefinition>
    {
        /// <summary>
        /// Reads a JSON string value and converts it to a <see cref="SqlTypeDefinition"/> instance.
        /// </summary>
        /// <param name="reader">The reader positioned at the JSON value to convert. Must be at a string token representing the SQL type
        /// definition.</param>
        /// <param name="typeToConvert">The type of the object to convert. This parameter is ignored; the method always returns a <see
        /// cref="SqlTypeDefinition"/>.</param>
        /// <param name="options">Options to control the behavior of the JSON serializer. This parameter is not used by this method.</param>
        /// <returns>A <see cref="SqlTypeDefinition"/> instance created from the JSON string value.</returns>
        /// <exception cref="JsonException">Thrown if the current JSON token is not a string.</exception>
        public override SqlTypeDefinition Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // SQL-Typ Definition wird als String erwartet
            if (reader.TokenType == JsonTokenType.String)
            {
                string sqlTypeDefString = reader.GetString() ?? string.Empty;
                return new SqlTypeDefinition(sqlTypeDefString);
            }
            else
            {
                throw new JsonException("Expected a string value for SqlTypeDefinition.");
            }
        }

        /// <summary>
        /// Writes the string representation of the specified SQL type definition to the JSON output using the provided
        /// writer.
        /// </summary>
        /// <param name="writer">The writer to which the JSON value will be written. Must not be null.</param>
        /// <param name="value">The SQL type definition to serialize as a JSON string. Must not be null.</param>
        /// <param name="options">Options to control the serialization behavior. This parameter is not used by this method.</param>
        public override void Write(Utf8JsonWriter writer, SqlTypeDefinition value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
