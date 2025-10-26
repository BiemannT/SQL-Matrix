using Microsoft.Data.SqlClient;
using System.Collections;

namespace Matrix.MsSql.Unit
{
    /// <summary>
    /// This class provides properties and methods to represent one input parameter for a test case.
    /// </summary>
    public class TestCaseParameter
    {
        /// <summary>
        /// Initialize an instance of <see cref="TestCaseParameter"/>.
        /// </summary>
        /// <param name="parameterBase">General base information used by this parameter instance.</param>
        /// <remarks>It is not foreseen to create this instance by public calls.</remarks>
        protected TestCaseParameter (TestCaseParameterBase parameterBase)
        {
            this.BaseInfo = parameterBase;
        }

        /// <summary>
        /// Gets the base information for this specific test case parameter.
        /// </summary>
        /// <value>An instance of type <see cref="TestCaseParameterBase"/>.</value>
        public TestCaseParameterBase BaseInfo { get; private set; }

        /// <summary>
        /// Gets the value associated with the parameter.
        /// </summary>
        /// <remarks>
        /// The value is <see langword="null"/>, if the properties <see cref="IsDefaultValue"/> is <see langword="true"/>.
        /// If <see cref="IsNull"/> is <see langword="true"/>, then the value is <see cref="DBNull.Value"/>.
        /// </remarks>
        /// <value>
        /// The value of this parameter as <see langword="object"/>-type.
        /// The type of the value is defined in the property <see cref="Type"/>.
        /// </value>
        public object? Value { get; private set; }

        /// <summary>
        /// Determines whether the <see cref="Value"/> is generated or user specific.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if the <see cref="Value"/> is generated based on the parameter definition.
        /// Otherwise <see langword="false"/> if the <see cref="Value"/> is user specific.
        /// </value>
        public bool IsBuiltinValue { get; private set; }

        /// <summary>
        /// Determines whether the <see cref="Value"/> represents SQL DEFAULT input.
        /// </summary>
        /// <value><see langword="true"/> if this parameter represents a DEFAULT value, otherwise <see langword="false"/>.</value>
        /// <remarks>If <see langword="true"/> the property <see cref="Value"/> is <see langword="null"/>.</remarks>
        public bool IsDefaultValue { get; private set; }

        /// <summary>
        /// Determines whether the <see cref="Value"/> represents SQL NULL input.
        /// </summary>
        /// <value><see langword="true"/> if this parameter represents a NULL value, otherwise <see langword="false"/>.</value>
        /// <remarks>If <see langword="true"/> the property <see cref="Value"/> is <see langword="null"/>.</remarks>
        public bool IsNull { get; private set; }

        /// <summary>
        /// Prepare the <see cref="SqlParameter"/> to perform the test case.
        /// </summary>
        /// <returns>Returns an instance of <see cref="SqlParameter"/> based on the properties from this instance.</returns>
        public SqlParameter GetSqlParameter()
        {
            // Parameter initialisieren
            SqlParameter parameter = new()
            {
                SqlDbType = this.BaseInfo.Type,
                ParameterName = this.BaseInfo.ParameterName,
                Direction = System.Data.ParameterDirection.Input,
                IsNullable = this.BaseInfo.IsNullable,
                Size = this.BaseInfo.Size,
                Precision = this.BaseInfo.Precision,
                Scale = this.BaseInfo.Scale,
                SqlValue = this.Value
            };

            return parameter;
        }

        /// <summary>
        /// Generates a set of <see cref="TestCaseParameter"/> for one test case input parameter as defined in the <see cref="TestInput"/>-instance.
        /// </summary>
        /// <param name="input">An instance of <see cref="TestInput"/> as template for the generation of test parameters.</param>
        /// <returns>Returns an array of <see cref="TestCaseParameter"/> with built-in test values and user specific test values.</returns>
        /// <exception cref="InvalidOperationException">The content of the <paramref name="input"/> is not valid.</exception>
        /// <exception cref="NotSupportedException">The user defined values for the specific SqlDbType are not implemented yet.</exception>"
        public static TestCaseParameter[] GenerateParameters(TestInput input)
        {
            TestCaseParameterBase parameterBase;
            Array? builtinValues;
            ArrayList parameters = [];

            try
            {
                // Input Definition analysieren und Parameter Basis-Klasse erzeugen
                parameterBase = new(input);

                // Built-in Testwerte einlesen
                builtinValues = parameterBase.BuiltinTestValue?.Invoke(parameterBase.Size, parameterBase.Precision, parameterBase.Scale);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("The Test Definition Input Parameter is not valid.", ex);
            }

            // Parameter mit Standard-Werten erstellen
            
            // NULL-Wert
            if (input.Nullable)
            {
                TestCaseParameter test = new(parameterBase)
                {
                    IsBuiltinValue = true,
                    IsNull = true,
                    IsDefaultValue = false,
                    Value = DBNull.Value
                };

                parameters.Add(test);
            }

            // DEFAULT-Wert
            if (input.DefaultValue)
            {
                TestCaseParameter test = new(parameterBase)
                {
                    IsBuiltinValue = true,
                    IsNull = false,
                    IsDefaultValue = true,
                    Value = null
                };

                parameters.Add(test);
            }

            // Built-in test Werte
            if (builtinValues != null)
            {
                foreach (object wert in builtinValues)
                {
                    TestCaseParameter test = new(parameterBase)
                    {
                        IsBuiltinValue = true,
                        IsNull = false,
                        IsDefaultValue = false,
                        Value = wert
                    };

                    parameters.Add(test);
                }
            }

            // Benutzerdefinierte Werte liegen als "JsonElement" im Array vor und müssen in den entsprechenden Datentyp konvertiert werden.
            // Dabei werden die korrekten Datentypen überprüft.
            // Es findet aber keine Prüfung statt, ob der Wert den Definitionsbereich des SQL-Typs überschreitet, da diese Prüfungsaufgabe eventuell erwünscht ist.
            // Standardmäßig werden vom SQL-Server die Daten abgeschnitten, wenn sie den Definitionsbereich überschreiten ohne eine Fehlermeldung zu erzeugen.
            if (input.UserValues != null)
            {
                foreach (System.Text.Json.JsonElement benutzerWert in input.UserValues)
                {
                    // Zielobjekt für den konvertierten Parameterwert
                    object convertedValue;

                    // Fehlermeldung für eine fehlerhafte Konvertierung
                    // Bei überlangen Werten wird nur der Anfangsteil angezeigt.
                    string ValueErrorMessage = benutzerWert.ToString().Length > 20
                        ? string.Concat("'", benutzerWert.ToString()[..20], "...'")
                        : string.Concat("'", benutzerWert.ToString(), "'");
                    string conversionErrorMessage = $"The user-defined value {ValueErrorMessage} cannot be converted to the SQL {parameterBase.Type} type.";
                    string NotSupportedErrorMessage = $"The user-defined values for the SQL {parameterBase.Type} type are not supported yet.";

                    // NULL-Werte werden als benutzerdefinierte Werte nicht unterstützt, da diese bereits in den builtin-Werten berücksichtigt werden, wenn der Nullable-Parameter gesetzt ist.
                    if (benutzerWert.ValueKind == System.Text.Json.JsonValueKind.Null)
                    {
                        throw new InvalidOperationException("The user-defined NULL values are not supported. Please use the 'Nullable' property to include NULL test values.");
                    }

                    // Prüfen ob der benutzerdefinierte Wert mit dem erwarteten Datentyp übereinstimmt
                    switch (parameterBase.Type)
                    {
                        case System.Data.SqlDbType.Bit:
                            // Beim Datentyp Bit sind nur die Werte 0, 1, true, false zulässig und diese sind bereits von den builtin-Werten abgedeckt.
                            throw new NotSupportedException("The user-defined values for the SQL Bit type are not supported. The test values 'true' and 'false' are included in the built-in test values by default.");

                        #region SQL-Typen die im JSON-Element einen String erwarten
                        case System.Data.SqlDbType.Char:
                        case System.Data.SqlDbType.VarChar:
                        case System.Data.SqlDbType.NChar:
                        case System.Data.SqlDbType.NVarChar:
                        case System.Data.SqlDbType.UniqueIdentifier:
                        case System.Data.SqlDbType.Time:
                        case System.Data.SqlDbType.Date:
                        case System.Data.SqlDbType.SmallDateTime:
                        case System.Data.SqlDbType.DateTime:
                        case System.Data.SqlDbType.DateTime2:
                        case System.Data.SqlDbType.DateTimeOffset:
                            if (benutzerWert.ValueKind != System.Text.Json.JsonValueKind.String)
                            {
                                throw new InvalidOperationException(conversionErrorMessage);
                            }

                            try
                            {
                                convertedValue = parameterBase.Type switch
                                {
                                    System.Data.SqlDbType.Char => benutzerWert.GetString() ?? string.Empty,
                                    System.Data.SqlDbType.VarChar => benutzerWert.GetString() ?? string.Empty,
                                    System.Data.SqlDbType.NChar => benutzerWert.GetString() ?? string.Empty,
                                    System.Data.SqlDbType.NVarChar => benutzerWert.GetString() ?? string.Empty,
                                    System.Data.SqlDbType.UniqueIdentifier => benutzerWert.GetGuid(),
                                    System.Data.SqlDbType.Time => benutzerWert.GetDateTime().TimeOfDay,
                                    System.Data.SqlDbType.Date => benutzerWert.GetDateTime().Date,
                                    System.Data.SqlDbType.SmallDateTime => benutzerWert.GetDateTime(),
                                    System.Data.SqlDbType.DateTime => benutzerWert.GetDateTime(),
                                    System.Data.SqlDbType.DateTime2 => benutzerWert.GetDateTime(),
                                    System.Data.SqlDbType.DateTimeOffset => benutzerWert.GetDateTimeOffset(),
                                    _ => DBNull.Value
                                };
                            }
                            catch
                            {
                                throw new InvalidOperationException(conversionErrorMessage);
                            }
                            break;
                        #endregion

                        #region SQL-Typen die im JSON-Element eine Number erwarten
                        case System.Data.SqlDbType.TinyInt:
                        case System.Data.SqlDbType.SmallInt:
                        case System.Data.SqlDbType.Int:
                        case System.Data.SqlDbType.BigInt:
                        case System.Data.SqlDbType.SmallMoney:
                        case System.Data.SqlDbType.Money:
                        case System.Data.SqlDbType.Decimal:
                        case System.Data.SqlDbType.Real:
                        case System.Data.SqlDbType.Float:
                            if (benutzerWert.ValueKind != System.Text.Json.JsonValueKind.Number)
                            {
                                throw new InvalidOperationException(conversionErrorMessage);
                            }

                            try
                            {
                                convertedValue = parameterBase.Type switch
                                {
                                    System.Data.SqlDbType.TinyInt => benutzerWert.GetByte(),
                                    System.Data.SqlDbType.SmallInt => benutzerWert.GetInt16(),
                                    System.Data.SqlDbType.Int => benutzerWert.GetInt32(),
                                    System.Data.SqlDbType.BigInt => benutzerWert.GetInt64(),
                                    System.Data.SqlDbType.SmallMoney => benutzerWert.GetDecimal(),
                                    System.Data.SqlDbType.Money => benutzerWert.GetDecimal(),
                                    System.Data.SqlDbType.Decimal => benutzerWert.GetDecimal(),
                                    System.Data.SqlDbType.Real => benutzerWert.GetSingle(),
                                    System.Data.SqlDbType.Float => benutzerWert.GetDouble(),
                                    _ => DBNull.Value
                                };
                            }
                            catch
                            {
                                throw new InvalidOperationException(conversionErrorMessage);
                            }
                            break;
                        #endregion

                        default:
                            throw new NotSupportedException(NotSupportedErrorMessage);
                    }

                    TestCaseParameter test = new(parameterBase)
                    {
                        IsBuiltinValue = false,
                        IsNull = false,
                        IsDefaultValue = false,
                        Value = convertedValue,
                    };
                    parameters.Add(test);
                }
            }

            return (TestCaseParameter[])parameters.ToArray(typeof(TestCaseParameter));
        }
    }
}