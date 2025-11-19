using System.Text.Json;
using System.Text.Json.Serialization;

namespace BiemannT.MUT.MsSql.Def.JSON
{
    /// <summary>
    /// This Converter-Class provides read and write methods to handle SQL values in JSON data.
    /// </summary>
    public class JsonSqlValueConverter : JsonConverter<object>
    {
        // The content of this class is based on an example of the .NET documentation:
        // https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/converters-how-to#deserialize-inferred-types-to-object-properties


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override bool HandleNull => true;

        /// <summary>
        /// Reads and converts the JSON value into a .NET compatible object type.
        /// </summary>
        /// <remarks>If reading fails, the reader will skip the current node.</remarks>
        /// <param name="reader">The reader.</param>
        /// <param name="typeToConvert">The type to convert.</param>
        /// <param name="options">An object that specifies serialization options to use.</param>
        /// <returns><inheritdoc/></returns>
        public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.TokenType switch
            {
                JsonTokenType.Null => DBNull.Value,
                JsonTokenType.False => false,
                JsonTokenType.True => true,

                JsonTokenType.Number when reader.TryGetByte(out byte value) => value,
                JsonTokenType.Number when reader.TryGetInt16(out short value) => value,
                JsonTokenType.Number when reader.TryGetInt32(out int value) => value,
                JsonTokenType.Number when reader.TryGetInt64(out long value) => value,
                JsonTokenType.Number when reader.TryGetDecimal(out decimal value) => value,
                JsonTokenType.Number when reader.TryGetSingle(out Single value) => value,
                JsonTokenType.Number when reader.TryGetDouble(out double value) => value,

                JsonTokenType.String when reader.TryGetGuid(out Guid value) => value,
                JsonTokenType.String when reader.TryGetDateTime(out DateTime value) => value,
                JsonTokenType.String when reader.TryGetDateTimeOffset(out DateTimeOffset value) => value,
                JsonTokenType.String => reader.GetString(),
                _ => reader.TrySkip()
            };
        }

        /// <summary>
        /// Writes the SQL value as JSON.
        /// </summary>
        /// <param name="writer">The writer to write to.</param>
        /// <param name="value">The value to convert to JSON.</param>
        /// <param name="options">An object that specifies serialization options to use.</param>
        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            var runtimeType = value.GetType();
            if (runtimeType == typeof(object))
            {
                writer.WriteStartObject();
                writer.WriteEndObject();
                return;
            }

            JsonSerializer.Serialize(writer, value, runtimeType, options);
        }
    }
}
