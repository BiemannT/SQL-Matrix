using System.Text.Json;
using System.Text.Json.Serialization;

namespace BiemannT.MUT.MsSql.Def.JSON
{
    /// <summary>
    /// Converts between JSON arrays representing SQL values and lists of objects for serialization and deserialization
    /// operations.
    /// </summary>
    public class JsonSqlValueListConverter : JsonConverter<List<object>>
    {
        /// <summary>
        /// Reads a JSON array of SQL values from the specified reader and returns them as a list of objects.
        /// </summary>
        /// <param name="reader">The reader positioned at the start of the JSON array to be read. The reader must reference a JSON array
        /// containing SQL values.</param>
        /// <param name="typeToConvert">The type of objects to convert the JSON values to. This parameter is not used for individual value
        /// conversion in this implementation.</param>
        /// <param name="options">Options to control the behavior of the JSON serializer during reading, such as custom converters or
        /// formatting settings.</param>
        /// <returns>A list of objects representing the SQL values read from the JSON array. The list will be empty if the array
        /// contains no values.</returns>
        /// <exception cref="JsonException">Thrown if the reader is not positioned at a JSON array or if the JSON is malformed.</exception>
        public override List<object> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Liste für die Werte erstellen
            List<object> values = [];

            // Start des Arrays
            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonException("Expected StartArray token for SQL value list.");
            }

            reader.Read();

            JsonSqlValueConverter sqlValueConverter = new();

            // Alle Werte bis zum Ende des Arrays verarbeiten
            while (reader.TokenType != JsonTokenType.EndArray)
            {
                // Wert mit dem SqlValueConverter lesen
                object value = sqlValueConverter.Read(ref reader, typeof(object), options);
                values.Add(value);
                reader.Read();
            }

            return values;
        }

        public override void Write(Utf8JsonWriter writer, List<object> value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
