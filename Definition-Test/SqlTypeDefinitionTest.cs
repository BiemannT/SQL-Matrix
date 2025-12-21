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
        public void Test_ToString()
        {
            // Typdefinitionen erstellen
            Dictionary<SqlTypeDefinition, string> definitions = [];

            definitions.Add(new SqlTypeDefinition { SqlType = SupportedSqlType.NotSupported }, string.Empty);
            definitions.Add(new SqlTypeDefinition { SqlType = SupportedSqlType.Binary }, "BINARY");
            definitions.Add(new SqlTypeDefinition { SqlType = SupportedSqlType.Binary, Size = 2000 }, "BINARY(2000)");
            definitions.Add(new SqlTypeDefinition { SqlType = SupportedSqlType.VarBinary, Size = 2000 }, "VARBINARY(2000)");
            definitions.Add(new SqlTypeDefinition { SqlType = SupportedSqlType.VarBinary, Size = -1 }, "VARBINARY(MAX)");
            definitions.Add(new SqlTypeDefinition { SqlType = SupportedSqlType.Char }, "CHAR");
            definitions.Add(new SqlTypeDefinition { SqlType = SupportedSqlType.Char, Size = 2000 }, "CHAR(2000)");
            definitions.Add(new SqlTypeDefinition { SqlType = SupportedSqlType.VarChar }, "VARCHAR");
            definitions.Add(new SqlTypeDefinition { SqlType = SupportedSqlType.VarChar, Size = 2000 }, "VARCHAR(2000)");
            definitions.Add(new SqlTypeDefinition { SqlType = SupportedSqlType.VarChar, Size = -1 }, "VARCHAR(MAX)");
            definitions.Add(new SqlTypeDefinition { SqlType = SupportedSqlType.NChar }, "NCHAR");
            definitions.Add(new SqlTypeDefinition { SqlType = SupportedSqlType.NChar, Size = 2000 }, "NCHAR(2000)");
            definitions.Add(new SqlTypeDefinition { SqlType = SupportedSqlType.NVarChar }, "NVARCHAR");
            definitions.Add(new SqlTypeDefinition { SqlType = SupportedSqlType.NVarChar, Size = 2000 }, "NVARCHAR(2000)");
            definitions.Add(new SqlTypeDefinition { SqlType = SupportedSqlType.NVarChar, Size = -1 }, "NVARCHAR(MAX)");
            definitions.Add(new SqlTypeDefinition { SqlType = SupportedSqlType.UniqueIdentifier }, "UNIQUEIDENTIFIER");
            definitions.Add(new SqlTypeDefinition { SqlType = SupportedSqlType.Bit }, "BIT");
            definitions.Add(new SqlTypeDefinition { SqlType = SupportedSqlType.TinyInt }, "TINYINT");
            definitions.Add(new SqlTypeDefinition { SqlType = SupportedSqlType.SmallInt }, "SMALLINT");
            definitions.Add(new SqlTypeDefinition { SqlType = SupportedSqlType.Int }, "INT");
            definitions.Add(new SqlTypeDefinition { SqlType = SupportedSqlType.BigInt }, "BIGINT");
            definitions.Add(new SqlTypeDefinition { SqlType = SupportedSqlType.SmallMoney }, "SMALLMONEY");
            definitions.Add(new SqlTypeDefinition { SqlType = SupportedSqlType.Money }, "MONEY");
            definitions.Add(new SqlTypeDefinition { SqlType = SupportedSqlType.Decimal }, "DECIMAL");
            definitions.Add(new SqlTypeDefinition { SqlType = SupportedSqlType.Decimal, Precision = 10 }, "DECIMAL(10)");
            definitions.Add(new SqlTypeDefinition { SqlType = SupportedSqlType.Decimal, Precision = 10, Scale = 3 }, "DECIMAL(10, 3)");
            definitions.Add(new SqlTypeDefinition { SqlType = SupportedSqlType.Real }, "REAL");
            definitions.Add(new SqlTypeDefinition { SqlType = SupportedSqlType.Float }, "FLOAT");
            definitions.Add(new SqlTypeDefinition { SqlType = SupportedSqlType.Float, Size = 10 }, "FLOAT(10)");
            definitions.Add(new SqlTypeDefinition { SqlType = SupportedSqlType.Time }, "TIME");
            definitions.Add(new SqlTypeDefinition { SqlType = SupportedSqlType.Time, Scale = 2 }, "TIME(2)");
            definitions.Add(new SqlTypeDefinition { SqlType = SupportedSqlType.Date }, "DATE");
            definitions.Add(new SqlTypeDefinition { SqlType = SupportedSqlType.SmallDateTime }, "SMALLDATETIME");
            definitions.Add(new SqlTypeDefinition { SqlType = SupportedSqlType.DateTime }, "DATETIME");
            definitions.Add(new SqlTypeDefinition { SqlType = SupportedSqlType.DateTime2 }, "DATETIME2");
            definitions.Add(new SqlTypeDefinition { SqlType = SupportedSqlType.DateTime2, Scale = 2 }, "DATETIME2(2)");
            definitions.Add(new SqlTypeDefinition { SqlType = SupportedSqlType.DateTimeOffset }, "DATETIMEOFFSET");
            definitions.Add(new SqlTypeDefinition { SqlType = SupportedSqlType.DateTimeOffset, Scale = 2 }, "DATETIMEOFFSET(2)");

            // Text Rückgabe testen
            foreach (var def in definitions)
            {
                Assert.AreEqual(def.Value, def.Key.ToString());
            }

        }
    }
}
