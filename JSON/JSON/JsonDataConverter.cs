using System.Text.Json;

namespace BiemannT.MUT.MsSql.Def.JSON
{
    /// <summary>
    /// This class contains static methods to convert JSON-data to or from a JSON test definition file.
    /// </summary>
    public static class JsonDataConverter
    {
        /// <summary>
        /// Converts a <see cref="JsonElement"/> into a .NET type compatible to the desired <paramref name="expectedSqlType"/>.
        /// </summary>
        /// <param name="data">The <see cref="JsonElement"/> to be converted.</param>
        /// <param name="expectedSqlType">The <paramref name="expectedSqlType"/> will define the .NET return type. If <see cref="Common.SupportedSqlType.NotSupported"/> a <see cref="NotSupportedException"/> will be thrown.</param>
        /// <returns>
        /// If the <see cref="JsonElement.ValueKind"/> of the input <paramref name="data"/> is compatible to the <paramref name="expectedSqlType"/>,
        /// the input data will be converted in an equivalent .NET type.
        /// A JSON null-value will return <see cref="DBNull.Value"/>.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// Will be thrown if the <see cref="JsonElement.ValueKind"/> is not compatible to the <paramref name="expectedSqlType"/>
        /// - OR - if <paramref name="expectedSqlType"/> is <see cref="Common.SupportedSqlType.NotSupported"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Will be thrown, if the input <paramref name="data"/> cannot be parsed into the equivalent .NET type.
        /// See the inner Exception for further details.
        /// </exception>
        public static object JsonToSql(JsonElement data, Common.SupportedSqlType expectedSqlType)
        {
            // Fehlermeldung für eine fehlerhafte Konvertierung
            // Bei überlangen Werten wird nur der Anfangsteil angezeigt
            string ValueErrorMessage = data.ToString().Length > 20
                ? string.Concat("'", data.ToString()[..20], "...'")
                : string.Concat("'", data.ToString(), "'");

            // Fehlermeldung wenn der SQL-Typ "NotSupported" ist
            if (expectedSqlType == Common.SupportedSqlType.NotSupported) throw new NotSupportedException($"The conversation of the value {ValueErrorMessage} is not possible, as the target SQL DB-type is not supported.");

            // Vom JSON-Element die Eigenschaft JsonValueKind zunächst auswerten

            // NULL-Wert
            if (data.ValueKind == JsonValueKind.Null)
            {
                return DBNull.Value;
            }

            // True-Wert, nur beim SQL-Typ Bit sinnvoll
            if (data.ValueKind == JsonValueKind.True)
            {
                if (expectedSqlType == Common.SupportedSqlType.Bit)
                {
                    return true;
                }
                else
                {
                    throw new NotSupportedException($"The conversion of the value 'true' into the requested SQL DB-type '{expectedSqlType}'. It is only supported for the SQL DB-type 'Bit'.");
                }
            }

            // False-Wert, nur beim SQL-Typ Bit sinnvoll
            if (data.ValueKind == JsonValueKind.False)
            {
                if (expectedSqlType == Common.SupportedSqlType.Bit)
                {
                    return false;
                }
                else
                {
                    throw new NotSupportedException($"The conversation of the value 'false' into the requested SQL DB-type '{expectedSqlType}' is not supported. It is only supported for the SQL DB-type 'Bit'.");
                }
            }

            // String-Wert
            if (data.ValueKind == JsonValueKind.String)
            {
                // Versuche die Umwandlung, abhängig vom erwarteten SQL-Datentyp
                try
                {
                    return expectedSqlType switch
                    {
                        Common.SupportedSqlType.Char => data.GetString() ?? string.Empty,
                        Common.SupportedSqlType.VarChar => data.GetString() ?? string.Empty,
                        Common.SupportedSqlType.NChar => data.GetString() ?? string.Empty,
                        Common.SupportedSqlType.NVarChar => data.GetString() ?? string.Empty,
                        Common.SupportedSqlType.UniqueIdentifier => data.GetGuid(),
                        Common.SupportedSqlType.Time => data.GetDateTime().TimeOfDay,
                        Common.SupportedSqlType.Date => data.GetDateTime().Date,
                        Common.SupportedSqlType.SmallDateTime => data.GetDateTime(),
                        Common.SupportedSqlType.DateTime => data.GetDateTime(),
                        Common.SupportedSqlType.DateTime2 => data.GetDateTime(),
                        Common.SupportedSqlType.DateTimeOffset => data.GetDateTimeOffset(),
                        _ => throw new NotSupportedException($"The conversation of the value {ValueErrorMessage} into the requested SQL DB-type '{expectedSqlType}' is not supported.")
                    };
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"An error occured during conversation of the JSON-value {ValueErrorMessage} into the requested SQL DB-type '{expectedSqlType}'.", ex);
                }
            }

            // Zahlen-Wert
            if (data.ValueKind == JsonValueKind.Number)
            {
                // Versuche die Umwandlung, abhängig vom erwarteten SQL-Datentyp
                try
                {
                    return expectedSqlType switch
                    {
                        Common.SupportedSqlType.TinyInt => data.GetByte(),
                        Common.SupportedSqlType.SmallInt => data.GetInt16(),
                        Common.SupportedSqlType.Int => data.GetInt32(),
                        Common.SupportedSqlType.BigInt => data.GetInt64(),
                        Common.SupportedSqlType.SmallMoney => data.GetDecimal(),
                        Common.SupportedSqlType.Money => data.GetDecimal(),
                        Common.SupportedSqlType.Decimal => data.GetDecimal(),
                        Common.SupportedSqlType.Real => data.GetSingle(),
                        Common.SupportedSqlType.Float => data.GetDouble(),
                        _ => throw new NotSupportedException($"The conversation of the value {ValueErrorMessage} into the requested SQL DB-type '{expectedSqlType}' is not supported.")
                    };
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"An error occured during conversation of the JSON-value {ValueErrorMessage} into the requested SQL DB-type '{expectedSqlType}'.", ex);
                }
            }

            // Rückgabe Fehlermeldung wenn bis hierher keine gültige Umwandlung möglich war
            throw new NotSupportedException($"The conversation of the JSON value kind {data.ValueKind} into the requested SQL DB-type '{expectedSqlType}' is not supported.");
        }
    }
}
