using BiemannT.MUT.MsSql.Def.Common;
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
        /// property (an array of column definitions) and a 'Rows' property (an array of row values). The method expects the
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
                        // Spaltendefinitionen sind in Objects definiert
                        if (reader.TokenType != JsonTokenType.StartObject)
                        {
                            throw new JsonException("Expected StartObject token for Column definition of ExpectedResultSet.");
                        }

                        reader.Read();

                        while (reader.TokenType != JsonTokenType.EndObject)
                        {
                            DataColumn column = new();

                            // Eigenschaft "Name" wird als string erwartet und stellt den Spaltennamen dar
                            if (reader.TokenType == JsonTokenType.PropertyName && reader.GetString() == "Name")
                            {
                                reader.Read();
                                if (reader.TokenType != JsonTokenType.String)
                                {
                                    throw new JsonException("Expected String token for Column name of ExpectedResultSet.");
                                }
                                column.ColumnName = reader.GetString() ?? "UnnamedColumn";
                                reader.Read();
                            }

                            // Eigenschaft "DataType" wird als string erwartet und stellt den Datentyp für die Spalte dar
                            if (reader.TokenType == JsonTokenType.PropertyName && reader.GetString() == "DataType")
                            {
                                reader.Read();
                                if (reader.TokenType != JsonTokenType.String)
                                {
                                    throw new JsonException("Expected String token for Column DataType of ExpectedResultSet.");
                                }

                                // Den Sql Typ aus dem String konvertieren
                                JsonSqlTypeDefConverter typeConverter = new();
                                SqlTypeDefinition sqlTypeDef = typeConverter.Read(ref reader, typeof(SqlTypeDefinition), options);

                                // Den entsprechenden .NET-Datentyp aus der SqlTypeDefinition ableiten
                                Type netType = sqlTypeDef.SqlType switch
                                {
                                    SupportedSqlType.Int => typeof(int),
                                    SupportedSqlType.BigInt => typeof(long),
                                    SupportedSqlType.SmallInt => typeof(short),
                                    SupportedSqlType.TinyInt => typeof(byte),
                                    SupportedSqlType.Bit => typeof(bool),
                                    SupportedSqlType.Decimal => typeof(decimal),
                                    SupportedSqlType.Float => typeof(double),
                                    SupportedSqlType.Real => typeof(Single),
                                    SupportedSqlType.DateTime => typeof(DateTime),
                                    SupportedSqlType.DateTime2 => typeof(DateTime),
                                    SupportedSqlType.DateTimeOffset => typeof(DateTimeOffset),
                                    SupportedSqlType.SmallDateTime => typeof(DateTime),
                                    SupportedSqlType.Date => typeof(DateTime),
                                    SupportedSqlType.Time => typeof(TimeSpan),
                                    SupportedSqlType.SmallMoney => typeof(decimal),
                                    SupportedSqlType.Money => typeof(decimal),
                                    SupportedSqlType.Char => typeof(string),
                                    SupportedSqlType.VarChar => typeof(string),
                                    SupportedSqlType.NChar => typeof(string),
                                    SupportedSqlType.NVarChar => typeof(string),
                                    SupportedSqlType.UniqueIdentifier => typeof(Guid),
                                    SupportedSqlType.Binary => typeof(byte[]),
                                    SupportedSqlType.VarBinary => typeof(byte[]),
                                    _ => typeof(object)
                                };

                                column.DataType = netType;
                                reader.Read();
                            }
                            else
                            {
                                // Unbekannte Eigenschaft innerhalb der Spaltendefinition überspringen
                                reader.Skip();
                            }

                            // Mindestens der Spaltenname muss definiert sein, um die Spalte hinzuzufügen
                            if (!string.IsNullOrEmpty(column.ColumnName))
                            {
                                table.Columns.Add(column);
                            }
                        }

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
                            Type targetType = table.Columns[columnIndex].DataType;
                            try
                            {
                                object cellValue = converter.Read(ref reader, targetType, options);
                                // Versuchen den Wert in die Spalte einzutragen
                                row[columnIndex++] = cellValue;
                            }
                            catch (ArgumentException ex)
                            {
                                throw new JsonException($"Error converting resultset value to type of column '{table.Columns[columnIndex - 1].ColumnName}'.", ex);
                            }
                            catch
                            {
                                throw;
                            }
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
