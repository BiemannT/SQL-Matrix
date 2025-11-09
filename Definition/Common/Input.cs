using System.Data;
using System.Text.RegularExpressions;

namespace BiemannT.MUT.MsSql.Def.Common
{
    /// <summary>
    /// This class provides the base properties and methods to represent a definition of a parameter requested by the test object.
    /// </summary>
    /// <remarks>All definition formats must inherit from this class.</remarks>
    public abstract class Input
    {
        /// <summary>
        /// Gets or sets the name of the parameter.
        /// </summary>
        public abstract string ParameterName { get; set; }

        /// <summary>
        /// Gets or sets the SQL-Server parameter type, including size, precision, and scale if applicable.
        /// </summary>
        /// <remarks>
        /// In the derived class the property setter should execute the <see cref="IdentifySqlType"/>-method to identify the parameter
        /// and to fill the protected values <see cref="SqlTypeDefinition"/>, <see cref="SqlSize"/>, <see cref="SqlPrecision"/> and <see cref="SqlScale"/>.
        /// If the type definition will be changed, the <see cref="UserValues"/>-list should be resetted in the derived class.
        /// </remarks>
        public abstract string SqlTypeDefinition { get; set; }

        /// <summary>
        /// Gets or sets if a NULL-value is allowed for this parameter.
        /// </summary>
        public abstract bool IsNullable { get; set; }

        /// <summary>
        /// Gets or sets if the parameter has a default value defined in the database.
        /// </summary>
        public abstract bool HasDefaultValue { get; set; }

        /// <summary>
        /// Gets or sets the direction of the parameter.
        /// </summary>
        public abstract ParameterDirection Direction { get; set; }

        /// <summary>
        /// Gets a list of user defined values to be tested additionally.
        /// </summary>
        /// <remarks>The type of each user value should be a type compatible to the <see cref="SqlType"/>.
        /// The content of the list should be resetted, if <see cref="SqlTypeDefinition"/> will be changed.</remarks>
        public abstract List<object> UserValues { get; }

        #region SQL-Type identification
        /// <summary>
        /// Gets the SQL precision of the parameter type.
        /// </summary>
        /// <remarks>In case the <see cref="SqlTypeDefinition"/> is not supported the value is 0.</remarks>
        protected byte SqlPrecision { get; private set; } = 0;

        /// <summary>
        /// Gets the SQL scale of the parameter type.
        /// </summary>
        /// <remarks>In case the <see cref="SqlTypeDefinition"/> is not supported the value is 0.</remarks>
        protected byte SqlScale { get; private set; } = 0;

        /// <summary>
        /// Gets the SQL size of the parameter type.
        /// </summary>
        /// <remarks>In case the <see cref="SqlTypeDefinition"/> is not supported the value is 0. A value of -1 indicates MAX-size.</remarks>
        protected int SqlSize { get; private set; } = 0;

        /// <summary>
        /// Gets the identified SQL type of the parameter.
        /// </summary>
        /// <remarks>In case the <see cref="SqlTypeDefinition"/> is not supported the value is <see cref="SupportedSqlType.NotSupported"/>.</remarks>
        protected SupportedSqlType SqlType { get; private set; } = SupportedSqlType.NotSupported;

        /// <summary>
        /// Identifys the SQL type from the SqlTypeDefinition property and sets the SqlType, SqlPrecision, SqlScale, and SqlSize properties accordingly.
        /// </summary>
        /// <exception cref="InvalidOperationException">Will be thrown, if the <see cref="SqlTypeDefinition"/> is not a valid SQL-type or the type is not supported.</exception>
        protected void IdentifySqlType()
        {
            // Die SQL-Metadaten Funktion "COLUMNPROPERTY" und die Systemsicht "sys.columns"
            // liefern nützliche Informationen über Spalten und Parameter für alle Datenbankobjekte.
            // Link zu den Datentypen:
            // https://learn.microsoft.com/en-us/sql/t-sql/data-types/data-types-transact-sql
            // Spezielle Hinweise für Zeit-Datentypen:
            // https://learn.microsoft.com/en-us/sql/connect/ado-net/sql/date-time-data

            // Zuerst die Basisinformationen zurücksetzen
            this.SqlType = SupportedSqlType.NotSupported;
            this.SqlPrecision = 0;
            this.SqlScale = 0;
            this.SqlSize = 0;

            // Fortsetzung nur, wenn der Parameter SqlTypeDefinition gesetzt ist
            if (string.IsNullOrWhiteSpace(this.SqlTypeDefinition)) return;

            // Mithilfe eines regulären Ausdrucks den Typ identifizieren

            string pattern = @"^(\b\w+)(\s*\((\d+|MAX),?\s*(\d+)?\))?$";
            Match analyze = Regex.Match(this.SqlTypeDefinition, pattern, RegexOptions.IgnoreCase);

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
                throw new InvalidOperationException($"Parameter type {this.SqlTypeDefinition} is not supported.");
            }

            switch (analyze.Groups[1].Value.ToUpper())
            {
                case "BINARY":
                    this.SqlType = SupportedSqlType.Binary;
                    this.SqlSize = analyzeSize;
                    break;

                case "VARBINARY":
                    this.SqlType = SupportedSqlType.VarBinary;
                    this.SqlSize = analyzeMAX ? -1 : analyzeSize;
                    break;

                case "CHAR":
                    this.SqlType = SupportedSqlType.Char;
                    this.SqlSize = analyzeSize;
                    break;

                case "VARCHAR":
                    this.SqlType = SupportedSqlType.VarChar;
                    this.SqlSize = analyzeMAX ? -1 : analyzeSize;
                    break;

                case "NCHAR":
                    this.SqlType = SupportedSqlType.NChar;
                    this.SqlSize = analyzeSize;
                    break;

                case "NVARCHAR":
                    this.SqlType = SupportedSqlType.NVarChar;
                    this.SqlSize = analyzeMAX ? -1 : analyzeSize;
                    break;

                case "UNIQUEIDENTIFIER":
                    this.SqlType = SupportedSqlType.UniqueIdentifier;
                    break;

                case "BIT":
                    this.SqlType = SupportedSqlType.Bit;
                    break;

                case "TINYINT":
                    this.SqlType = SupportedSqlType.TinyInt;
                    break;

                case "SMALLINT":
                    this.SqlType = SupportedSqlType.SmallInt;
                    break;

                case "INT":
                    this.SqlType = SupportedSqlType.Int;
                    break;

                case "BIGINT":
                    this.SqlType = SupportedSqlType.BigInt;
                    break;

                case "SMALLMONEY":
                    this.SqlType = SupportedSqlType.SmallMoney;
                    break;

                case "MONEY":
                    this.SqlType = SupportedSqlType.Money;
                    break;

                case "DECIMAL":
                case "NUMERIC":
                    this.SqlType = SupportedSqlType.Decimal;
                    this.SqlPrecision = analyzePrecision;
                    this.SqlScale = analyzeScale;
                    break;

                case "REAL":
                    this.SqlType = SupportedSqlType.Real;
                    break;

                case "FLOAT":
                    this.SqlType = SupportedSqlType.Float;
                    this.SqlSize = analyzeSize;
                    break;

                case "TIME":
                    this.SqlType = SupportedSqlType.Time;
                    this.SqlScale = analyzePrecision;
                    break;

                case "DATE":
                    this.SqlType = SupportedSqlType.Date;
                    break;

                case "SMALLDATETIME":
                    this.SqlType = SupportedSqlType.SmallDateTime;
                    break;

                case "DATETIME":
                    this.SqlType = SupportedSqlType.DateTime;
                    break;

                case "DATETIME2":
                    this.SqlType = SupportedSqlType.DateTime2;
                    this.SqlScale = analyzePrecision;
                    break;

                case "DATETIMEOFFSET":
                    this.SqlType = SupportedSqlType.DateTimeOffset;
                    this.SqlScale = analyzePrecision;
                    break;

                default:
                    throw new InvalidOperationException($"Parameter type {this.SqlTypeDefinition} is not supported.");
            }
        }
        #endregion
    }
}
