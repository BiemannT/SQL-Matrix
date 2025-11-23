using System.Text.RegularExpressions;

namespace BiemannT.MUT.MsSql.Def.Common
{
    /// <summary>
    /// Represents the definition of a SQL data type, including its type, size, precision, and scale as parsed from a
    /// SQL type declaration string.
    /// </summary>
    public class SqlTypeDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlTypeDefinition"/>-class with default values indicating an unsupported SQL type.
        /// </summary>
        public SqlTypeDefinition()
        {
            SqlType = SupportedSqlType.NotSupported;
            Size = 0;
            Precision = 0;
            Scale = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlTypeDefinition"/>-class by parsing the specified SQL type definition string.
        /// </summary>
        /// <param name="sqlTypeDefinition">A string containing the SQL type definition to be parsed.
        /// An empty string - OR - a not supported type will return an instance with <see cref="SupportedSqlType.NotSupported"/>.</param>
        public SqlTypeDefinition(string sqlTypeDefinition): this()
        {
            // Datentyp analysieren
            try
            {
                ParseSqlTypeDefinition(sqlTypeDefinition);
            }
            catch { }
        }

        /// <summary>
        /// Gets or sets a supported SQL type.
        /// </summary>
        public SupportedSqlType SqlType { get; set; }

        /// <summary>
        /// Gets or sets the size of this SQL type.
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of digits allowed in the fractional part of a numeric value.
        /// </summary>
        public byte Precision { get; set; }

        /// <summary>
        /// Gets or sets the number of decimal places to use for numeric values.
        /// </summary>
        public byte Scale { get; set; }

        /// <summary>
        /// Returns a <see langword="string"/> that represents the SQL data type definition, including type name and any applicable size,
        /// precision, or scale parameters.
        /// </summary>
        /// <remarks>The returned string follows SQL Server syntax conventions. For types that support a
        /// size, precision, or scale, these values are included in parentheses. For variable-length types supporting
        /// 'MAX', '(MAX)' is appended when appropriate. If the type is 'DECIMAL', both precision and scale are included
        /// if specified. For time-related types, scale is included if greater than zero.</remarks>
        /// <returns>
        /// A <see langword="string"/> containing the SQL type name and parameters formatted as a SQL type declaration.
        /// In case the parameters size, precision or scale are invalid, the type name without parameters is returned.
        /// </returns>
        public override string ToString()
        {
            string output = SqlType switch
            {
                SupportedSqlType.Binary => "BINARY",
                SupportedSqlType.VarBinary => "VARBINARY",
                SupportedSqlType.Char => "CHAR",
                SupportedSqlType.VarChar => "VARCHAR",
                SupportedSqlType.NChar => "NCHAR",
                SupportedSqlType.NVarChar => "NVARCHAR",
                SupportedSqlType.UniqueIdentifier => "UNIQUEIDENTIFIER",
                SupportedSqlType.Bit => "BIT",
                SupportedSqlType.TinyInt => "TINYINT",
                SupportedSqlType.SmallInt => "SMALLINT",
                SupportedSqlType.Int => "INT",
                SupportedSqlType.BigInt => "BIGINT",
                SupportedSqlType.SmallMoney => "SMALLMONEY",
                SupportedSqlType.Money => "MONEY",
                SupportedSqlType.Decimal => "DECIMAL",
                SupportedSqlType.Real => "REAL",
                SupportedSqlType.Float => "FLOAT",
                SupportedSqlType.Time => "TIME",
                SupportedSqlType.Date => "DATE",
                SupportedSqlType.SmallDateTime => "SMALLDATETIME",
                SupportedSqlType.DateTime => "DATETIME",
                SupportedSqlType.DateTime2 => "DATETIME2",
                SupportedSqlType.DateTimeOffset => "DATETIMEOFFSET",
                _ => string.Empty
            };

            // Bei einem nicht unterstützten Typ gleich string.Empty zurückgeben
            if (string.IsNullOrEmpty(output)) return output;

            // Optional noch die Größenparameter anhängen
            // Folgende Typen unterstützen MAX:
            // VARBINARY, VARCHAR, NVARCHAR
            if (Size == -1 && 
                SqlType == SupportedSqlType.VarBinary ||
                SqlType == SupportedSqlType.VarChar ||
                SqlType == SupportedSqlType.NVarChar)
            {
                output = string.Concat(output, "(MAX)");
                return output;
            }

            // Folgende Typen unterstützen Size:
            // BINARY, VARBINARY, CHAR, VARCHAR, NCHAR, NVARCHAR, FLOAT
            else if (Size > 0 &&
                    SqlType == SupportedSqlType.Binary ||
                    SqlType == SupportedSqlType.VarBinary ||
                    SqlType == SupportedSqlType.Char ||
                    SqlType == SupportedSqlType.VarChar ||
                    SqlType == SupportedSqlType.NChar ||
                    SqlType == SupportedSqlType.NVarChar ||
                    SqlType == SupportedSqlType.Float)
            {
                output = string.Concat(output, "(", Size.ToString(), ")");
                return output;
            }

            // Beim Decimal-Typ können optional noch Precision und Scale angegeben werden
            // Scale kann nur angegeben werden, wenn auch Precision angegeben wurde
            if (SqlType == SupportedSqlType.Decimal)
            {
                if (Precision > 0)
                {
                    if (Scale > 0)
                    {
                        output = string.Concat(output, "(", Precision.ToString(), ", ", Scale.ToString(), ")");
                    }
                    else if (Scale == 0)
                    {
                        output = string.Concat(output, "(", Precision.ToString(), ")");
                    }
                    return output;
                }
            }

            // Bei den Zeit-Datentypen kann optional noch die Scale angegeben werden
            if (Scale > 0 &&
                SqlType == SupportedSqlType.Time ||
                SqlType == SupportedSqlType.DateTime2 ||
                SqlType == SupportedSqlType.DateTimeOffset)
            {
                output = string.Concat(output, "(", Scale.ToString(), ")");
                return output;
            }

            // Als Fallback wird nur der Typname zurückgegeben
            return output;
        }

        /// <summary>
        /// Parses a SQL Server type definition string and updates the corresponding type, size, precision, and scale
        /// properties based on the parsed values.
        /// </summary>
        /// <remarks>Supported SQL Server types include character, binary, numeric, and date/time types.
        /// If the type definition includes parameters such as size, precision, or scale, these will be extracted and
        /// set accordingly. For types that support the "MAX" keyword, the size will be set to -1. No changes are made
        /// if the input is null, empty, or consists only of whitespace.</remarks>
        /// <param name="sqlTypeDefinition">The SQL Server type definition to parse, such as "VARCHAR(100)", "DECIMAL(18,2)", or "DATETIME". The value
        /// must be a non-empty string representing a valid SQL Server data type.</param>
        /// <exception cref="InvalidOperationException">Thrown if the specified SQL type definition is not supported or cannot be parsed.</exception>
        public void ParseSqlTypeDefinition(string sqlTypeDefinition)
        {
            // Link zu den Datentypen:
            // https://learn.microsoft.com/en-us/sql/t-sql/data-types/data-types-transact-sql
            // Spezielle Hinweise für Zeit-Datentypen:
            // https://learn.microsoft.com/en-us/sql/connect/ado-net/sql/date-time-data

            // Fortsetzung nur, wenn ein Wert übergeben wurde
            if (string.IsNullOrWhiteSpace(sqlTypeDefinition)) return;

            // Mithilfe eines regulären Ausdrucks den SQL-Typ und die optionalen Parameter extrahieren
            string pattern = @"^(\b\w+)(\s*\((\d+|MAX),?\s*(\d+)?\))?$";
            Match analyze = Regex.Match(sqlTypeDefinition, pattern, RegexOptions.IgnoreCase);

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
                throw new InvalidOperationException($"SQL type '{sqlTypeDefinition}' is not supported.");
            }

            switch (analyze.Groups[1].Value.ToUpper())
            {
                case "BINARY":
                    SqlType = SupportedSqlType.Binary;
                    Size = analyzeSize;
                    break;

                case "VARBINARY":
                    SqlType = SupportedSqlType.VarBinary;
                    Size = analyzeMAX ? -1 : analyzeSize;
                    break;

                case "CHAR":
                    SqlType = SupportedSqlType.Char;
                    Size = analyzeSize;
                    break;

                case "VARCHAR":
                    SqlType = SupportedSqlType.VarChar;
                    Size = analyzeMAX ? -1 : analyzeSize;
                    break;

                case "NCHAR":
                    SqlType = SupportedSqlType.NChar;
                    Size = analyzeSize;
                    break;

                case "NVARCHAR":
                    SqlType = SupportedSqlType.NVarChar;
                    Size = analyzeMAX ? -1 : analyzeSize;
                    break;

                case "UNIQUEIDENTIFIER":
                    SqlType = SupportedSqlType.UniqueIdentifier;
                    break;

                case "BIT":
                    SqlType = SupportedSqlType.Bit;
                    break;

                case "TINYINT":
                    SqlType = SupportedSqlType.TinyInt;
                    break;

                case "SMALLINT":
                    SqlType = SupportedSqlType.SmallInt;
                    break;

                case "INT":
                    SqlType = SupportedSqlType.Int;
                    break;

                case "BIGINT":
                    SqlType = SupportedSqlType.BigInt;
                    break;

                case "SMALLMONEY":
                    SqlType = SupportedSqlType.SmallMoney;
                    break;

                case "MONEY":
                    SqlType = SupportedSqlType.Money;
                    break;

                case "DECIMAL":
                case "NUMERIC":
                    SqlType = SupportedSqlType.Decimal;
                    Precision = analyzePrecision;
                    Scale = analyzeScale;
                    break;

                case "REAL":
                    SqlType = SupportedSqlType.Real;
                    break;

                case "FLOAT":
                    SqlType = SupportedSqlType.Float;
                    break;

                case "TIME":
                    SqlType = SupportedSqlType.Time;
                    Scale = analyzePrecision;
                    break;

                case "DATE":
                    SqlType = SupportedSqlType.Date;
                    break;

                case "SMALLDATETIME":
                    SqlType = SupportedSqlType.SmallDateTime;
                    break;

                case "DATETIME":
                    SqlType = SupportedSqlType.DateTime;
                    break;

                case "DATETIME2":
                    SqlType = SupportedSqlType.DateTime2;
                    Scale = analyzePrecision;
                    break;

                case "DATETIMEOFFSET":
                    SqlType = SupportedSqlType.DateTimeOffset;
                    Scale = analyzePrecision;
                    break;

                default:
                    throw new InvalidOperationException($"SQL type '{sqlTypeDefinition}' is not supported.");
            }

        }
    }
}
