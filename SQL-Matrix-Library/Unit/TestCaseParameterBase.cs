using System.Data;
using System.Text.RegularExpressions;

namespace Matrix.MsSql.Unit
{
    /// <summary>
    /// This class provides base properties for several <see cref="TestCaseParameter"/>-instances.
    /// </summary>
    public class TestCaseParameterBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="TestCaseParameterBase"/> based on the information given in the <paramref name="input"/>-parameter.
        /// </summary>
        /// <param name="input">An instance of <see cref="TestInput"/>.</param>
        /// <exception cref="ArgumentNullException">Will be thrown if the <see cref="TestInput.ParameterName"/> or <see cref="TestInput.SqlType"/> is not set.</exception>
        /// <exception cref="InvalidOperationException">Will be thrown if the <see cref="TestInput.SqlType"/> can not be identified or the type is not supported by the Matrix Unit Test.</exception>
        public TestCaseParameterBase(TestInput input)
        {
            // Parameter Name
            if (string.IsNullOrWhiteSpace(input.ParameterName))
            {
                throw new ArgumentNullException(nameof(input), "No parameter name defined.");
            }
            else
            {
                this.ParameterName = input.ParameterName;
            }

            // Datentyp identifizieren
            if (string.IsNullOrWhiteSpace(input.SqlType))
            {
                throw new ArgumentNullException(nameof(input), "No SQL type defined.");
            }
            else
            {
                try
                {
                    IdentifyTypeDefinition(input.SqlType);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Identifying the SQL data type for the parameter \"{input.ParameterName}\" failed.", ex);
                }
            }

            // Nullable
            this.IsNullable = input.Nullable;
        }

        /// <summary>
        /// Gets the name of the SQL-Parameter.
        /// </summary>
        /// <value>The name of the parameter.</value>
        public string ParameterName { get; private set; }

        /// <summary>
        /// Gets the SQL-Server specific type of this parameter.
        /// </summary>
        /// <value>The type of this parameter as an element of the <see cref="SqlDbType"/>-enumeration.</value>
        /// <seealso cref="Microsoft.Data.SqlClient.SqlParameter.SqlDbType"/>
        public SqlDbType Type { get; private set; }

        /// <summary>
        /// Gets the maximum size, in bytes, of data handled with this parameter.
        /// </summary>
        /// <value>
        /// The maximum size of this parameter in bytes.
        /// In case of large value data types defined with "MAX", the value is -1.
        /// If the size is not defined or not required for the <see cref="Type"/>, the value is 0.
        /// </value>
        /// <seealso cref="Microsoft.Data.SqlClient.SqlParameter.Size"/>
        public int Size { get; private set; }

        /// <summary>
        /// Gets the maximum number of digits used to represent this parameter.
        /// </summary>
        /// <value>
        /// The maximum number of overall digits in case of <see cref="SqlDbType.Decimal"/>.
        /// If the precision is not defined or not required for the <see cref="Type"/>, the value is 0.
        /// </value>
        /// <seealso cref="Microsoft.Data.SqlClient.SqlParameter.Precision"/>
        public byte Precision { get; private set; }

        /// <summary>
        /// Gets the number of decimal places to which this parameter is resolved.
        /// </summary>
        /// <value>
        /// The number of decimal places to which this parameter is resolved.
        /// In case of time specific <see cref="Type"/>, the scale represents the time portion used by this parameter.
        /// If the scale is not defined or not required for the <see cref="Type"/>, the value is 0.
        /// </value>
        /// <seealso cref="Microsoft.Data.SqlClient.SqlParameter.Scale"/>
        public byte Scale { get; private set; }

        /// <summary>
        /// Gets a value that indicates whether the parameter accepts null values.
        /// </summary>
        /// <value><see langword="true"/> if null values are accepted, otherwise <see langword="false"/>.</value>
        public bool IsNullable { get; private set; }

        /// <summary>
        /// Gets the method handler to retrieve the array with built-in test values, if available for the actual SQL-type.
        /// </summary>
        /// <value>Returns a delegate of <see cref="BuiltinTestValueHandler"/> to the desired static method which will return the built-in test values.</value>
        public BuiltinTestValueHandler? BuiltinTestValue { get; private set; }

        /// <summary>
        /// Identify the parameter type, size, precision and scale based on the SQL type definition.
        /// </summary>
        /// <param name="typeName">A SQL-Parameter definition as <see langword="string"/>.</param>
        /// <exception cref="InvalidOperationException">Will be thrown, if the <paramref name="typeName"/> is not a valid SQL-type or the type is not supported by this Unit test version.</exception>
        private void IdentifyTypeDefinition (string typeName)
        {
            // Die SQL-Metadaten Funktion "COLUMNPROPERTY" und die Systemsicht "sys.columns"
            // liefern nützliche Informationen über Spalten und Parameter für alle Datenbankobjekte.
            // Link zu den Datentypen:
            // https://learn.microsoft.com/en-us/sql/t-sql/data-types/data-types-transact-sql
            // Spezielle Hinweise für Zeit-Datentypen:
            // https://learn.microsoft.com/en-us/sql/connect/ado-net/sql/date-time-data
            // Mithilfe eines regulären Ausdrucks den Typ identifizieren

            string pattern = @"^(\b\w+)(\s*\((\d+|MAX),?\s*(\d+)?\))?$";
            Match analyze = Regex.Match(typeName, pattern, RegexOptions.IgnoreCase);

            // Gruppe 1: Name vom Parametertyp
            // Gruppe 3: Precision oder Size oder MAX
            // Gruppe 4: Scale

            // Hilfsvariablen zur Auswertung
            byte analyzePrecision = 0;
            byte analyzeScale = 0;
            int analyzeSize = 0;
            bool analyzeMAX = false;

            if (analyze.Success)
            {
                if (analyze.Groups[3].Success)
                {
                    if (analyze.Groups[3].Value == "MAX")
                    {
                        analyzeMAX = true;
                    }
                    else
                    {
                        _ = int.TryParse(analyze.Groups[3].Value, out analyzeSize);
                        _ = byte.TryParse(analyze.Groups[3].Value, out analyzePrecision);
                    }
                }

                if (analyze.Groups[4].Success)
                {
                    _ = byte.TryParse(analyze.Groups[4].Value, out analyzeScale);
                }
            }
            else
            {
                throw new InvalidOperationException($"Parameter type {typeName} is not supported.");
            }

            switch (analyze.Groups[1].Value.ToUpper())
            {
                case "BINARY":
                    this.Type = SqlDbType.Binary;
                    this.Size = analyzeSize;
                    this.BuiltinTestValue = new BuiltinTestValueHandler(BuiltinTestValues.BuiltinBinary);
                    break;

                case "VARBINARY":
                    this.Type = SqlDbType.VarBinary;
                    if (analyzeMAX)
                    {
                        this.Size = -1;
                    }
                    else
                    {
                        this.Size = analyzeSize;
                    }
                    this.BuiltinTestValue = new BuiltinTestValueHandler(BuiltinTestValues.BuiltinVarbinary);
                    break;

                case "CHAR":
                    this.Type = SqlDbType.Char;
                    this.Size = analyzeSize;
                    this.BuiltinTestValue = new BuiltinTestValueHandler(BuiltinTestValues.BuiltinChar);
                    break;

                case "VARCHAR":
                    this.Type = SqlDbType.VarChar;
                    if (analyzeMAX)
                    {
                        this.Size = -1;
                    }
                    else
                    {
                        this.Size = analyzeSize;
                    }
                    this.BuiltinTestValue = new BuiltinTestValueHandler(BuiltinTestValues.BuiltinVarchar);
                    break;

                case "NCHAR":
                    this.Type = SqlDbType.NChar;
                    this.Size = analyzeSize;
                    this.BuiltinTestValue = new BuiltinTestValueHandler(BuiltinTestValues.BuiltinNChar);
                    break;

                case "NVARCHAR":
                    this.Type = SqlDbType.NVarChar;
                    if (analyzeMAX)
                    {
                        this.Size = -1;
                    }
                    else
                    {
                        this.Size = analyzeSize;
                    }
                    this.BuiltinTestValue = new BuiltinTestValueHandler(BuiltinTestValues.BuiltinNVarchar);
                    break;

                case "UNIQUEIDENTIFIER":
                    this.Type = SqlDbType.UniqueIdentifier;
                    this.BuiltinTestValue = new BuiltinTestValueHandler(BuiltinTestValues.BuiltinUniqueidentifier);
                    break;

                case "BIT":
                    this.Type = SqlDbType.Bit;
                    this.BuiltinTestValue = new BuiltinTestValueHandler(BuiltinTestValues.BuiltinBit);
                    break;

                case "TINYINT":
                    this.Type = SqlDbType.TinyInt;
                    this.BuiltinTestValue = new BuiltinTestValueHandler(BuiltinTestValues.BuiltinTinyInt);
                    break;

                case "SMALLINT":
                    this.Type = SqlDbType.SmallInt;
                    this.BuiltinTestValue = new BuiltinTestValueHandler(BuiltinTestValues.BuiltinSmallInt);
                    break;

                case "INT":
                    this.Type = SqlDbType.Int;
                    this.BuiltinTestValue = new BuiltinTestValueHandler(BuiltinTestValues.BuiltinInt);
                    break;

                case "BIGINT":
                    this.Type = SqlDbType.BigInt;
                    this.BuiltinTestValue = new BuiltinTestValueHandler(BuiltinTestValues.BuiltinBigInt);
                    break;

                case "SMALLMONEY":
                    this.Type = SqlDbType.SmallMoney;
                    this.BuiltinTestValue = new BuiltinTestValueHandler(BuiltinTestValues.BuiltinSmallmoney);
                    break;

                case "MONEY":
                    this.Type = SqlDbType.Money;
                    this.BuiltinTestValue = new BuiltinTestValueHandler(BuiltinTestValues.BuiltinMoney);
                    break;

                case "DECIMAL":
                case "NUMERIC":
                    this.Type = SqlDbType.Decimal;
                    this.Precision = analyzePrecision;
                    this.Scale = analyzeScale;
                    this.BuiltinTestValue = new BuiltinTestValueHandler(BuiltinTestValues.BuiltinDecimal);
                    break;

                case "REAL":
                    this.Type = SqlDbType.Real;
                    this.BuiltinTestValue = new BuiltinTestValueHandler(BuiltinTestValues.BuiltinReal);
                    break;

                case "FLOAT":
                    this.Type = SqlDbType.Float;
                    this.Size = analyzeSize;
                    this.BuiltinTestValue = new BuiltinTestValueHandler(BuiltinTestValues.BuiltinFloat);
                    break;

                case "TIME":
                    this.Type = SqlDbType.Time;
                    this.Scale = analyzePrecision;
                    this.BuiltinTestValue = new BuiltinTestValueHandler(BuiltinTestValues.BuiltinTime);
                    break;

                case "DATE":
                    this.Type = SqlDbType.Date;
                    this.BuiltinTestValue = new BuiltinTestValueHandler(BuiltinTestValues.BuiltinDate);
                    break;

                case "SMALLDATETIME":
                    this.Type = SqlDbType.SmallDateTime;
                    this.BuiltinTestValue = new BuiltinTestValueHandler(BuiltinTestValues.BuiltinSmalldatetime);
                    break;

                case "DATETIME":
                    this.Type = SqlDbType.DateTime;
                    this.BuiltinTestValue = new BuiltinTestValueHandler(BuiltinTestValues.BuiltinDatetime);
                    break;

                case "DATETIME2":
                    this.Type = SqlDbType.DateTime2;
                    this.Scale = analyzePrecision;
                    this.BuiltinTestValue = new BuiltinTestValueHandler(BuiltinTestValues.BuiltinDatetime2);
                    break;

                case "DATETIMEOFFSET":
                    this.Type = SqlDbType.DateTimeOffset;
                    this.Scale = analyzePrecision;
                    this.BuiltinTestValue = new BuiltinTestValueHandler(BuiltinTestValues.BuiltinDatetimeoffset);
                    break;

                case "XML":
                    this.Type = SqlDbType.Xml;
                    this.Size = -1;
                    break;

                case "SQL_VARIANT":
                    this.Type = SqlDbType.Variant;
                    break;

                default:
                    throw new InvalidOperationException($"Parameter type {typeName} is not supported.");

            }
        }
    }
}