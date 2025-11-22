using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BiemannT.MUT.MsSql.Def.JSON
{
    /// <summary>
    /// Provides JSON serialization and deserialization support for ExpectedResultSets represented as DataSet.
    /// </summary>
    public class JsonSqlResultsetConverter : JsonConverter<DataSet>
    {
        /// <summary>
        /// Reads a JSON array and deserializes its contents into a <see cref="DataSet"/> instance, where each element
        /// represents a <see cref="DataTable"/> with defined columns and rows.
        /// </summary>
        /// <remarks>The JSON input must be an array of objects, where each object contains a 'Columns'
        /// property (an array of column names) and a 'Rows' property (an array of row values). The method expects the
        /// 'Columns' property to appear first in each object, followed by the 'Rows' property. This method does not
        /// support nested tables or additional properties beyond 'Columns' and 'Rows'.</remarks>
        /// <param name="reader">A reference to the <see cref="Utf8JsonReader"/> positioned at the start of the JSON array to read. The
        /// reader will be advanced as the data is consumed.</param>
        /// <param name="typeToConvert">The type to convert the JSON data to. This parameter is ignored; the method always returns a <see
        /// cref="DataSet"/>.</param>
        /// <param name="options">Options to control the behavior of the JSON deserialization, such as custom converters or naming policies.</param>
        /// <returns>A <see cref="DataSet"/> containing one or more <see cref="DataTable"/> objects populated from the JSON
        /// array. Each table includes columns and rows as defined in the JSON structure.</returns>
        /// <exception cref="JsonException">Thrown if the JSON structure does not match the expected format, such as missing array or object tokens, or
        /// if required properties ('Columns' or 'Rows') are absent or malformed.</exception>
        public override DataSet Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Lokale Variable für das DataSet erstellen
            DataSet resultsets = new();
            resultsets.BeginInit();

            // Start-Array des DataSets lesen
            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonException("Expected StartArray token for ExpectedResultSets.");
            }

            reader.Read();

            // Alle Resultsets (DataTables) lesen, bis das End-Array erreicht ist
            while (reader.TokenType != JsonTokenType.EndArray)
            {
                // Für jedes Resultset ein neues DataTable-Objekt erstellen
                DataTable table = new();

                // Start-Object des DataTables lesen
                if (reader.TokenType != JsonTokenType.StartObject)
                {
                    throw new JsonException("Expected StartObject token for ExpectedResultSet-table.");
                }
                reader.Read();

                // Columns lesen
                if (reader.TokenType == JsonTokenType.PropertyName && reader.GetString() == "Columns")
                {
                    reader.Read();

                    if (reader.TokenType != JsonTokenType.StartArray)
                    {
                        throw new JsonException("Expected StartArray token for ExpectedResultSet column definition.");
                    }

                    reader.Read();

                    while (reader.TokenType != JsonTokenType.EndArray)
                    {
                        // Als Spaltennamen werden String-Werte erwartet
                        if (reader.TokenType != JsonTokenType.String)
                        {
                            throw new JsonException("Expected String token for Column name of ExpectedResultSet.");
                        }

                        string columnName = reader.GetString() ?? "UnnamedColumn";
                        table.Columns.Add(columnName);
                        reader.Read();
                    }

                    reader.Read();
                }
                else
                {
                    throw new JsonException("Expected Columns property as first property in the ExpectedResultSet-definition.");
                }

                // Rows lesen
                if (reader.TokenType == JsonTokenType.PropertyName && reader.GetString() == "Rows")
                {
                    reader.Read();

                    if (reader.TokenType != JsonTokenType.StartArray)
                    {
                        throw new JsonException("Expected StartArray token for ExpectedResultSet rows definition.");
                    }

                    reader.Read();

                    // Jede Row besteht aus einem Array von Werten
                    while (reader.TokenType != JsonTokenType.EndArray)
                    {
                        if (reader.TokenType != JsonTokenType.StartArray)
                        {
                            throw new JsonException("Expected StartArray token for Row.");
                        }

                        reader.Read();
                        DataRow row = table.NewRow();
                        int columnIndex = 0;

                        while (reader.TokenType != JsonTokenType.EndArray)
                        {
                            JsonSqlValueConverter converter = new();
                            object? cellValue = converter.Read(ref reader, typeof(object), options);
                            row[columnIndex++] = cellValue;
                            reader.Read();
                        }
                        table.Rows.Add(row);
                        reader.Read();
                    }
                    reader.Read();
                }
                else
                {
                    throw new JsonException("Expected Rows property as second property in the ExpectedResultSet-definition.");
                }

                // End-Object des DataTables lesen
                if (reader.TokenType != JsonTokenType.EndObject)
                {
                    throw new JsonException("Expected EndObject token for DataTable.");
                }

                resultsets.Tables.Add(table);

                // Reader Position vor dem Verlassen der Methode auf das nächste Element setzen
                reader.Read();

            }

            resultsets.EndInit();
            return resultsets;
        }
        public override void Write(Utf8JsonWriter writer, DataSet value, JsonSerializerOptions options)
        {
            // TODO: Von CoPilot vorgeschlagenen Code überprüfen und ggf. anpassen.
            throw new NotImplementedException();
            writer.WriteStartArray();
            foreach (DataTable table in value.Tables)
            {
                writer.WriteStartObject();
                writer.WriteString("TableName", table.TableName);
                writer.WriteStartArray("Columns");
                foreach (DataColumn column in table.Columns)
                {
                    writer.WriteStringValue(column.ColumnName);
                }
                writer.WriteEndArray();
                writer.WriteStartArray("Rows");
                foreach (DataRow row in table.Rows)
                {
                    writer.WriteStartArray();
                    foreach (var item in row.ItemArray)
                    {
                        JsonSerializer.Serialize(writer, item, options);
                    }
                    writer.WriteEndArray();
                }
                writer.WriteEndArray();
                writer.WriteEndObject();
            }
            writer.WriteEndArray();
        }
    }
}
