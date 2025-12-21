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
    }
}
