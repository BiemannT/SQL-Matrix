using System.Collections.ObjectModel;

namespace BiemannT.MUT.MsSql.Def.Common.Test
{
    [TestClass]
    public sealed class SqlTypeDefinitionTest
    {
        /// <summary>
        /// Verifies that the validation logic correctly identifies invalid property combinations for various SQL data
        /// types and returns the expected error count and severity.
        /// </summary>
        /// <remarks>Each test case provides a combination of SQL type and property values that are
        /// considered invalid according to the type's constraints. The method asserts that the validation result
        /// contains the expected number of errors and that the error severity matches the provided value.</remarks>
        /// <param name="type">The SQL data type to validate. Determines which set of property constraints are applied during validation.</param>
        /// <param name="precision">The precision value to use for the SQL type. Represents the total number of digits for numeric types;
        /// ignored for types where precision is not applicable.</param>
        /// <param name="scale">The scale value to use for the SQL type. Represents the number of digits to the right of the decimal point
        /// for numeric types; ignored for types where scale is not applicable.</param>
        /// <param name="size">The size value to use for the SQL type. Specifies the length or storage size in bytes or characters,
        /// depending on the type.</param>
        /// <param name="errorCount">The expected number of validation errors that should be returned for the given property values.</param>
        /// <param name="errorSeverity">The expected severity of the validation error. Used to verify that the validation result matches the
        /// anticipated error level.</param>
        [TestMethod]

        // Testdaten für Typ Binary
        [DataRow(SupportedSqlType.Binary, (byte)0, (byte)0, -1, 1, ValidationResultSeverity.Error)]
        [DataRow(SupportedSqlType.Binary, (byte)0, (byte)0, 9000, 1, ValidationResultSeverity.Error)]

        // Testdaten für Typ Char
        [DataRow(SupportedSqlType.Char, (byte)0, (byte)0, -1, 1, ValidationResultSeverity.Error)]
        [DataRow(SupportedSqlType.Char, (byte)0, (byte)0, 9000, 1, ValidationResultSeverity.Error)]

        // Testdaten für Typ Varbinary
        [DataRow(SupportedSqlType.VarBinary, (byte)0, (byte)0, -2, 1, ValidationResultSeverity.Error)]
        [DataRow(SupportedSqlType.VarBinary, (byte)0, (byte)0, 0, 1, ValidationResultSeverity.Error)]
        [DataRow(SupportedSqlType.VarBinary, (byte)0, (byte)0, 9000, 1, ValidationResultSeverity.Error)]

        // Testdaten für Typ VarChar
        [DataRow(SupportedSqlType.VarChar, (byte)0, (byte)0, -2, 1, ValidationResultSeverity.Error)]
        [DataRow(SupportedSqlType.VarChar, (byte)0, (byte)0, 0, 1, ValidationResultSeverity.Error)]
        [DataRow(SupportedSqlType.VarChar, (byte)0, (byte)0, 9000, 1, ValidationResultSeverity.Error)]

        // Testdaten für Typ NChar
        [DataRow(SupportedSqlType.NChar, (byte)0, (byte)0, 0, 1, ValidationResultSeverity.Error)]
        [DataRow(SupportedSqlType.NChar, (byte)0, (byte)0, 4001, 1, ValidationResultSeverity.Error)]

        // Testdaten für Typ NVarChar
        [DataRow(SupportedSqlType.NVarChar, (byte)0, (byte)0, -2, 1, ValidationResultSeverity.Error)]
        [DataRow(SupportedSqlType.NVarChar, (byte)0, (byte)0, 0, 1, ValidationResultSeverity.Error)]
        [DataRow(SupportedSqlType.NVarChar, (byte)0, (byte)0, 9000, 1, ValidationResultSeverity.Error)]

        // Testdaten für Typ Decimal
        [DataRow(SupportedSqlType.Decimal, (byte)0, (byte)0, 0, 1, ValidationResultSeverity.Error)]
        [DataRow(SupportedSqlType.Decimal, (byte)39, (byte)0, 0, 1, ValidationResultSeverity.Error)]
        [DataRow(SupportedSqlType.Decimal, (byte)10, (byte)11, 0, 1, ValidationResultSeverity.Error)]
        [DataRow(SupportedSqlType.Decimal, (byte)39, (byte)40, 0, 2, ValidationResultSeverity.Error)]

        // Testdaten für Typ Time
        [DataRow(SupportedSqlType.Time, (byte)0, (byte)8, 0, 1, ValidationResultSeverity.Error)]

        //Testdaten für Typ DateTime2
        [DataRow(SupportedSqlType.DateTime2, (byte)0, (byte)8, 0, 1, ValidationResultSeverity.Error)]

        // Testdaten für Typ DateTimeOffset
        [DataRow(SupportedSqlType.DateTimeOffset, (byte)0, (byte)8, 0, 1, ValidationResultSeverity.Error)]
        public void Test_Validate_InvalidValues(SupportedSqlType type, byte precision, byte scale, int size, int errorCount, ValidationResultSeverity errorSeverity)
        {
            // SQL-Datentyp mit ungültigen Eigenschaften

            SqlTypeDefinition typeTest = new()
            {
                SqlType = type,
                Precision = precision,
                Scale = scale,
                Size = size
            };

            ReadOnlyCollection<ValidationResult> results = typeTest.Validate();

            Assert.HasCount(errorCount, results);
            Assert.AreEqual(errorSeverity, results[0].Severity);

        }

        /// <summary>
        /// Verifies that the ToString method of the SqlTypeDefinition class returns the expected SQL type string
        /// representations for various type definitions.
        /// </summary>
        /// <remarks>This test covers a range of supported SQL types and their variations, including size,
        /// precision, and scale where applicable. It ensures that the string output matches the expected SQL syntax for
        /// each type definition.</remarks>
        [TestMethod]

        // Testdaten für NotSupported
        [DataRow(SupportedSqlType.NotSupported, (byte)0, (byte)0, 0, "")]

        // Testdaten für Binary
        [DataRow(SupportedSqlType.Binary, (byte)0, (byte)0, 0, "BINARY")]
        [DataRow(SupportedSqlType.Binary, (byte)0, (byte)0, 2000, "BINARY(2000)")]

        // Testdaten für Varbinary
        [DataRow(SupportedSqlType.VarBinary, (byte)0, (byte)0, 0, "VARBINARY")]
        [DataRow(SupportedSqlType.VarBinary, (byte)0, (byte)0, 2000, "VARBINARY(2000)")]
        [DataRow(SupportedSqlType.VarBinary, (byte)0, (byte)0, -1, "VARBINARY(MAX)")]

        // Testdaten für Char
        [DataRow(SupportedSqlType.Char, (byte)0, (byte)0, 0, "CHAR")]
        [DataRow(SupportedSqlType.Char, (byte)0, (byte)0, 2000, "CHAR(2000)")]

        // Testdaten für Varchar
        [DataRow(SupportedSqlType.VarChar, (byte)0, (byte)0, 0, "VARCHAR")]
        [DataRow(SupportedSqlType.VarChar, (byte)0, (byte)0, 2000, "VARCHAR(2000)")]
        [DataRow(SupportedSqlType.VarChar, (byte)0, (byte)0, -1, "VARCHAR(MAX)")]

        // Testdaten für Nchar
        [DataRow(SupportedSqlType.NChar, (byte)0, (byte)0, 0, "NCHAR")]
        [DataRow(SupportedSqlType.NChar, (byte)0, (byte)0, 2000, "NCHAR(2000)")]

        // Testdaten für Nvarchar
        [DataRow(SupportedSqlType.NVarChar, (byte)0, (byte)0, 0, "NVARCHAR")]
        [DataRow(SupportedSqlType.NVarChar, (byte)0, (byte)0, 2000, "NVARCHAR(2000)")]
        [DataRow(SupportedSqlType.NVarChar, (byte)0, (byte)0, -1, "NVARCHAR(MAX)")]

        // Testdaten für Uniqueidentifier
        [DataRow(SupportedSqlType.UniqueIdentifier, (byte)0, (byte)0, 0, "UNIQUEIDENTIFIER")]

        // Testdaten für Bit
        [DataRow(SupportedSqlType.Bit, (byte)0, (byte)0, 0, "BIT")]

        // Testdaten für Tinyint
        [DataRow(SupportedSqlType.TinyInt, (byte)0, (byte)0, 0, "TINYINT")]

        // Testdaten für Smallint
        [DataRow(SupportedSqlType.SmallInt, (byte)0, (byte)0, 0, "SMALLINT")]

        // Testdaten für Int
        [DataRow(SupportedSqlType.Int, (byte)0, (byte)0, 0, "INT")]

        // Testdaten für Bigint
        [DataRow(SupportedSqlType.BigInt, (byte)0, (byte)0, 0, "BIGINT")]

        // Testdaten für Smallmoney
        [DataRow(SupportedSqlType.SmallMoney, (byte)0, (byte)0, 0, "SMALLMONEY")]

        // Testdaten für Money
        [DataRow(SupportedSqlType.Money, (byte)0, (byte)0, 0, "MONEY")]

        // Testdaten für Decimal
        [DataRow(SupportedSqlType.Decimal, (byte)0, (byte)0, 0, "DECIMAL")]
        [DataRow(SupportedSqlType.Decimal, (byte)10, (byte)0, 0, "DECIMAL(10)")]
        [DataRow(SupportedSqlType.Decimal, (byte)10, (byte)3, 0, "DECIMAL(10, 3)")]

        // Testdaten für Real
        [DataRow(SupportedSqlType.Real, (byte)0, (byte)0, 0, "REAL")]

        // Testdaten für Float
        [DataRow(SupportedSqlType.Float, (byte)0, (byte)0, 0, "FLOAT")]
        [DataRow(SupportedSqlType.Float, (byte)10, (byte)0, 0, "FLOAT(10)")]

        // Testdaten für Time
        [DataRow(SupportedSqlType.Time, (byte)0, (byte)0, 0, "TIME")]
        [DataRow(SupportedSqlType.Time, (byte)0, (byte)3, 0, "TIME(3)")]

        // Testdaten für Date
        [DataRow(SupportedSqlType.Date, (byte)0, (byte)0, 0, "DATE")]

        // Testdaten für Smalldatetime
        [DataRow(SupportedSqlType.SmallDateTime, (byte)0, (byte)0, 0, "SMALLDATETIME")]

        // Testdaten für Datetime
        [DataRow(SupportedSqlType.DateTime, (byte)0, (byte)0, 0, "DATETIME")]

        // Testdaten für Datetime2
        [DataRow(SupportedSqlType.DateTime2, (byte)0, (byte)0, 0, "DATETIME2")]
        [DataRow(SupportedSqlType.DateTime2, (byte)0, (byte)5, 0, "DATETIME2(5)")]

        // Testdaten für DateTimeOffset
        [DataRow(SupportedSqlType.DateTimeOffset, (byte)0, (byte)0, 0, "DATETIMEOFFSET")]
        [DataRow(SupportedSqlType.DateTimeOffset, (byte)0, (byte)3, 0, "DATETIMEOFFSET(3)")]
        public void Test_ToString(SupportedSqlType testType, byte testPrecision, byte testScale, int testSize, string expectedResult)
        {
            // Text Rückgabe testen
            SqlTypeDefinition actual = new() { SqlType = testType, Precision = testPrecision, Scale = testScale, Size = testSize };

            Assert.AreEqual(expectedResult, actual.ToString());
        }

        /// <summary>
        /// Verifies that parsing a SQL type definition string produces the expected type, precision, scale, and size
        /// values.
        /// </summary>
        /// <remarks>This test method uses multiple data rows to validate parsing for a variety of SQL
        /// Server type definitions, including edge cases and unsupported types.</remarks>
        /// <param name="testDefinition">The SQL type definition string to parse. This may include type name and optional parameters such as size,
        /// precision, or scale.</param>
        /// <param name="expectedType">The expected SQL type, as determined by parsing the definition string.</param>
        /// <param name="expectedPrecision">The expected precision value to be extracted from the type definition. Used for types that support
        /// precision; otherwise, 0.</param>
        /// <param name="expectedScale">The expected scale value to be extracted from the type definition. Used for types that support scale;
        /// otherwise, 0.</param>
        /// <param name="expectedSize">The expected size value to be extracted from the type definition. Used for types that support size;
        /// otherwise, 0 or -1 for 'max'.</param>
        [TestMethod]

        // Testdaten für einen Unknown Typ
        [DataRow("UnknownType", SupportedSqlType.NotSupported, (byte)0, (byte)0, 0)]
        [DataRow("NChar(abc)", SupportedSqlType.NotSupported, (byte)0, (byte)0, 0)]

        // Testdaten für Binary
        [DataRow("binary", SupportedSqlType.Binary, (byte)0, (byte)0, 0)]
        [DataRow("BINARY(1000)", SupportedSqlType.Binary, (byte)0, (byte)0, 1000)]

        // Testdaten für Varbinary
        [DataRow("VARBINARY", SupportedSqlType.VarBinary, (byte)0, (byte)0, 0)]
        [DataRow("varbinary(max)", SupportedSqlType.VarBinary, (byte)0, (byte)0, -1)]
        [DataRow("varbinary(1000)", SupportedSqlType.VarBinary, (byte)0, (byte)0, 1000)]

        // Testdaten für Char
        [DataRow("char", SupportedSqlType.Char, (byte)0, (byte)0, 0)]
        [DataRow("CHAR(100)", SupportedSqlType.Char, (byte)0, (byte)0, 100)]

        // Testdaten für Varchar
        [DataRow("varchar", SupportedSqlType.VarChar, (byte)0, (byte)0, 0)]
        [DataRow("VARCHAR(MAX)", SupportedSqlType.VarChar, (byte)0, (byte)0, -1)]
        [DataRow("varchar(100)", SupportedSqlType.VarChar, (byte)0, (byte)0, 100)]

        // Testdaten für Nchar
        [DataRow("nchar", SupportedSqlType.NChar, (byte)0, (byte)0, 0)]
        [DataRow("NCHAR(100)", SupportedSqlType.NChar, (byte)0, (byte)0, 100)]

        // Testdaten für Nvarchar
        [DataRow("NVARCHAR", SupportedSqlType.NVarChar, (byte)0, (byte)0, 0)]
        [DataRow("nvarchar(MAX)", SupportedSqlType.NVarChar, (byte)0, (byte)0, -1)]
        [DataRow("NvarChar(100)", SupportedSqlType.NVarChar, (byte)0, (byte)0, 100)]

        // Testdaten für Uniqueidentifier
        [DataRow("uniqueidentifier", SupportedSqlType.UniqueIdentifier, (byte)0, (byte)0, 0)]

        // Testdaten für Bit
        [DataRow("BIT", SupportedSqlType.Bit, (byte)0, (byte)0, 0)]

        // Testdaten für Tinyint
        [DataRow("TinyInt", SupportedSqlType.TinyInt, (byte)0, (byte)0, 0)]

        // Testdaten für Smallint
        [DataRow("smallint", SupportedSqlType.SmallInt, (byte)0, (byte)0, 0)]

        // Testdaten für Int
        [DataRow("INT", SupportedSqlType.Int, (byte)0, (byte)0, 0)]

        // Testdaten für Bigint
        [DataRow("BigInt", SupportedSqlType.BigInt, (byte)0, (byte)0, 0)]

        // Testdaten für Smallmoney
        [DataRow("SMALLMONEY", SupportedSqlType.SmallMoney, (byte)0, (byte)0, 0)]

        // Testdaten für Money
        [DataRow("MONEY", SupportedSqlType.Money, (byte)0, (byte)0, 0)]

        // Testdaten für Decimal und Numeric
        [DataRow("decimal", SupportedSqlType.Decimal, (byte)0, (byte)0, 0)]
        [DataRow("decimal(20)", SupportedSqlType.Decimal, (byte)20, (byte)0, 0)]
        [DataRow("DECIMAL(20, 10)", SupportedSqlType.Decimal, (byte)20, (byte)10, 0)]
        [DataRow("decimal(20,10)", SupportedSqlType.Decimal, (byte)20, (byte)10, 0)]
        [DataRow("numeric", SupportedSqlType.Decimal, (byte)0, (byte)0, 0)]
        [DataRow("numeric(20)", SupportedSqlType.Decimal, (byte)20, (byte)0, 0)]
        [DataRow("NUMERIC(20, 10)", SupportedSqlType.Decimal, (byte)20, (byte)10, 0)]
        [DataRow("Numeric(20,10)", SupportedSqlType.Decimal, (byte)20, (byte)10, 0)]

        // Testdaten für Real
        [DataRow("real", SupportedSqlType.Real, (byte)0, (byte)0, 0)]

        // Testdaten für Float
        [DataRow("Float", SupportedSqlType.Float, (byte)0, (byte)0, 0)]
        [DataRow("FLOAT(10)", SupportedSqlType.Float, (byte)10, (byte)0, 0)]

        // Testdaten für Time
        [DataRow("time", SupportedSqlType.Time, (byte)0, (byte)0, 0)]
        [DataRow("TIME(2)", SupportedSqlType.Time, (byte)0, (byte)2, 0)]

        // Testdaten für Date
        [DataRow("Date", SupportedSqlType.Date, (byte)0, (byte)0, 0)]

        // Testdaten für Smalldatetime
        [DataRow("smallDateTime", SupportedSqlType.SmallDateTime, (byte)0, (byte)0, 0)]

        // Testdaten für Datetime
        [DataRow("dateTime", SupportedSqlType.DateTime, (byte)0, (byte)0, 0)]

        // Testdaten für Datetime2
        [DataRow("DateTime2", SupportedSqlType.DateTime2, (byte)0, (byte)0, 0)]
        [DataRow("datetime2(5)", SupportedSqlType.DateTime2, (byte)0, (byte)5, 0)]

        // Testdaten für DateTimeOffset
        [DataRow("datetimeoffset", SupportedSqlType.DateTimeOffset, (byte)0, (byte)0, 0)]
        [DataRow("DATETIMEOFFSET(3)", SupportedSqlType.DateTimeOffset, (byte)0, (byte)3, 0)]
        public void Test_ParseSqlTypeDefinition(string testDefinition, SupportedSqlType expectedType, byte expectedPrecision, byte expectedScale, int expectedSize)
        {
            // Test ausführen
            SqlTypeDefinition expected = new() { SqlType = expectedType, Precision = expectedPrecision, Scale = expectedScale, Size = expectedSize };
            SqlTypeDefinition actual = new(testDefinition);

            Assert.AreEqual(expected, actual);
        }
    }
}
