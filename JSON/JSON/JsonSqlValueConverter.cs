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
        /// <remarks>If reading fails a JsonException will be thrown.</remarks>
        /// <param name="reader">The reader.</param>
        /// <param name="typeToConvert">The target type this converter should try to convert the current <paramref name="reader"/>-value.
        /// In case of <see langword="object"/> the converter will try to convert the value to the nearest applicable .NET-type.</param>
        /// <param name="options">An object that specifies serialization options to use.</param>
        /// <exception cref="JsonException">Will be thrown if the JSON value cannot be converted to the target type.</exception>"
        /// <returns><inheritdoc/></returns>
        public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // TODO: Die Variable typeToConvert nutzen um eine zielgerichtete Konvertierung durchzuführen.
            // Nur wenn typeToConvert == typeof(object) ist, dann die hier implementierte Logik verwenden.
            // Wenn jedoch der Reader-Wert nicht in den typeToConvert passt, dann eine Exception werfen.
            // Dadurch soll die statische Methode JsonDataConverter.JsonToSql() ersetzt werden.

            // Je nach TokenType den passenden .NET-Typ zurückgeben
            // Im Fall von null soll bei jedem Datentyp DBNull.Value zurückgegeben werden
            if (reader.TokenType == JsonTokenType.Null)
            {
                return DBNull.Value;
            }

            // true und false nur beim Typ Boolean sinnvoll
            if (typeToConvert == typeof(bool) || typeToConvert == typeof(object))
            {
                if (reader.TokenType == JsonTokenType.True)
                {
                    return true;
                }

                if (reader.TokenType == JsonTokenType.False)
                {
                    return false;
                }
            }

            // Zahlentypen
            if (reader.TokenType == JsonTokenType.Number)
            {
                // Nacheinander prüfen, ob der Wert in den jeweiligen Typ passt
                // Dabei mit dem kleinsten Typ anfangen
                if (typeToConvert == typeof(byte) || typeToConvert == typeof(object))
                {
                    if (reader.TryGetByte(out byte byteValue))
                    {
                        return byteValue;
                    }
                }

                if (typeToConvert == typeof(short) || typeToConvert == typeof(object))
                {
                    if (reader.TryGetInt16(out short shortValue))
                    {
                        return shortValue;
                    }
                }

                if (typeToConvert == typeof(int) || typeToConvert == typeof(object))
                {
                    if (reader.TryGetInt32(out int intValue))
                    {
                        return intValue;
                    }
                }

                if (typeToConvert == typeof(long) || typeToConvert == typeof(object))
                {
                    if (reader.TryGetInt64(out long longValue))
                    {
                        return longValue;
                    }
                }

                if (typeToConvert == typeof(decimal) || typeToConvert == typeof(object))
                {
                    if (reader.TryGetDecimal(out decimal decimalValue))
                    {
                        return decimalValue;
                    }
                }

                if (typeToConvert == typeof(float) || typeToConvert == typeof(object))
                {
                    if (reader.TryGetSingle(out float floatValue))
                    {
                        return floatValue;
                    }
                }

                if (typeToConvert == typeof(double) || typeToConvert == typeof(object))
                {
                    if (reader.TryGetDouble(out double doubleValue))
                    {
                        return doubleValue;
                    }
                }
            }

            // String-Typen
            if (reader.TokenType == JsonTokenType.String)
            {
                // Nacheinander prüfen, ob der Wert in den jeweiligen Typ passt
                // Dabei mit den speziellen Typen anfangen
                if (typeToConvert == typeof(Guid) || typeToConvert == typeof(object))
                {
                    if (reader.TryGetGuid(out Guid guidValue))
                    {
                        return guidValue;
                    }
                }

                if (typeToConvert == typeof(DateTime) || typeToConvert == typeof(object))
                {
                    if (reader.TryGetDateTime(out DateTime dateTimeValue))
                    {
                        return dateTimeValue;
                    }
                }

                if (typeToConvert == typeof(DateTimeOffset) || typeToConvert == typeof(object))
                {
                    if (reader.TryGetDateTimeOffset(out DateTimeOffset dateTimeOffsetValue))
                    {
                        return dateTimeOffsetValue;
                    }
                }

                if (typeToConvert == typeof(string) || typeToConvert == typeof(object))
                {
                    return reader.GetString();
                }
            }

            // Wenn bis hierher keine Konvertierung möglich war, Fehler werfen
            string ValueErrorMessage = reader.GetString()?.Length > 20
                ? string.Concat("'", reader.GetString()?[..20], "...'")
                : string.Concat("'", reader.GetString(), "'");

            throw new JsonException($"The JSON value '{ValueErrorMessage}' cannot be converted to the target type '{typeToConvert.FullName}'.");
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
