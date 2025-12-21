using BiemannT.MUT.MsSql.Def.Common;
using System.Text.Json;

namespace BiemannT.MUT.MsSql.Def.JSON.Test
{
    [TestClass]
    public sealed class JsonDefinitionTest
    {
        public TestContext TestContext { get; set; }

        /// <summary>
        /// Test loading without a given file name.
        /// </summary>
        [TestMethod]
        public void Test_LoadWithoutFile()
        {
            JsonDefinition jsonTest = new();

            // Bei der Methode Load wird eine Exception erwartet.

            Exception result = Assert.ThrowsExactly<InvalidOperationException>(jsonTest.Load);

            TestContext.WriteLine("Actual load exception message: {0}", result.Message);

            Assert.AreEqual("FileName is not set.", result.Message);
        }

        /// <summary>
        /// Test loading of an empty json-file.
        /// </summary>
        [TestMethod]
        public void Test_LoadEmptyFile()
        {
            JsonDefinition jsonTest = new()
            {
                FileName = new("./Definitionen/Empty file.json")
            };

            // Bei der Methode Load wird eine Exception erwartet, mit einer innerException.
            Exception result = Assert.ThrowsExactly<InvalidOperationException>(jsonTest.Load);

            TestContext.WriteLine("Actual load exception message: {0}", result.Message);

            Assert.AreEqual("The test file definition contains invalid JSON code - or - required fields are missing!", result.Message);

            Assert.IsInstanceOfType(result.InnerException, typeof(JsonException));

            TestContext.WriteLine("Inner Exception of type '{0}'. Message: {1}", result.InnerException.GetType().FullName, result.InnerException.Message);

        }

        /// <summary>
        /// Test loading of a definition file with an invalid API version number.
        /// In the test file all required properties are set.
        /// </summary>
        [TestMethod]
        public void Test_LoadFileWithInvalidApiVersion()
        {
            JsonDefinition jsonTest = new()
            {
                FileName = new("./Definitionen/Invalid API version.json")
            };

            // Bei der Methode Load wird eine Exception erwartet mit der Nachricht, dass es sich nicht um eine gültige Version handelt.
            Exception result = Assert.ThrowsExactly<InvalidOperationException>(jsonTest.Load);

            TestContext.WriteLine("Actual load exception message: {0}", result.InnerException!.Message);

            Assert.StartsWith("Unsupported API version: v0", result.InnerException.Message);
        }

        /// <summary>
        /// Test loading of a definition file with a negative MaxExecutionTime.
        /// In the test file all required properties are set.
        /// </summary>
        [TestMethod]
        public void Test_LoadFileWithInvalidExecutionTime()
        {
            JsonDefinition jsonTest = new()
            {
                FileName = new("./Definitionen/Invalid execution time.json")
            };

            // Bei der Methode Load wird eine Exception erwartet mit der Nachricht, dass nur positive Zahlen erlaubt sind.
            Exception result = Assert.ThrowsExactly<ArgumentOutOfRangeException>(jsonTest.Load);

            TestContext.WriteLine("Actual load exception message: {0}", result.Message);

            Assert.StartsWith("MaxExecutionTime must be a positive integer.", result.Message);
            Assert.AreEqual(-10, ((ArgumentOutOfRangeException)result).ActualValue);
        }

        /// <summary>
        /// Test loading of a definition file with valid content.
        /// The file does not contain input parameters and no required table data definition.
        /// </summary>
        [TestMethod]
        public void Test_LoadValid_NoInputs_NoRequiredData()
        {
            Definition jsonTest = new JsonDefinition()
            {
                FileName = new("./Definitionen/Valid - no inputs - no required data.json")
            };

            jsonTest.Load();

            // Bei der Methode Load sollen alle Werte korrekt gelesen werden.
            TestContext.WriteLine("Actual schema name: {0}", jsonTest.SchemaName);
            Assert.AreEqual("dbo", jsonTest.SchemaName);

            TestContext.WriteLine("Actual test object name: {0}", jsonTest.ObjectName);
            Assert.AreEqual("TestProcedure", jsonTest.ObjectName);

            TestContext.WriteLine("Actual MaxExecutionTime: {0}", jsonTest.MaxExecutionTime);
            Assert.AreEqual(2, jsonTest.MaxExecutionTime);

            TestContext.WriteLine("Actual number of inputs: {0}", jsonTest.Inputs.Count);
            Assert.IsEmpty(jsonTest.Inputs);
        }

        /// <summary>
        /// Test loading of a definition file with valid content.
        /// The file does not contain input parameters and no required table data definition,
        /// but it contains different types of expected results.
        /// </summary>
        /// <remarks>
        /// The test file is based on the test <see cref="Test_LoadValid_NoInputs_NoRequiredData"/>.
        /// This test is focused on the correct loading of the different results.
        /// </remarks>
        [TestMethod]
        public void Test_LoadValid_NoInputs_NoRequiredData_Results()
        {
            Definition jsonTest = new JsonDefinition()
            {
                FileName = new("./Definitionen/Valid - no inputs - no required data - results.json")
            };

            jsonTest.Load();

            // Bei der Methode Load sollen alle Werte korrekt gelesen werden.
            // Es wird davon ausgegangen, dass die Basis Definitions-Eigenschaften richtig geladen werden.
            TestContext.WriteLine("Actual count of expected results: {0}", jsonTest.ExpectedResults.Count);
            Assert.HasCount(4, jsonTest.ExpectedResults);

            // Beispiel Exception Result
            TestContext.WriteLine("1st result: Type '{0}' - Error Number: '{1}' - Timestamp: '{2}' - Duration: '{3}'", jsonTest.ExpectedResults[0].ResultType.ToString(), jsonTest.ExpectedResults[0].ErrorNumber.ToString(), jsonTest.ExpectedResults[0].LastExecution.ToString(), jsonTest.ExpectedResults[0].LastDuration.ToString());
            Assert.AreEqual(ExpectedResultTypes.Exception, jsonTest.ExpectedResults[0].ResultType);
            Assert.AreEqual(515, jsonTest.ExpectedResults[0].ErrorNumber);
            Assert.AreEqual(new DateTime(2025, 11, 17, 18, 0, 0), jsonTest.ExpectedResults[0].LastExecution);
            Assert.AreEqual(new TimeSpan(0, 0, 0, 0, 500), jsonTest.ExpectedResults[0].LastDuration);

            // Beispiel Value Result
            TestContext.WriteLine("2nd result: Type '{0}' - Value: '{1}' - Timestamp: '{2}' - Duration: '{3}'", jsonTest.ExpectedResults[1].ResultType.ToString(), jsonTest.ExpectedResults[1].Value?.ToString() ?? "null", jsonTest.ExpectedResults[1].LastExecution.ToString(), jsonTest.ExpectedResults[1].LastDuration.ToString());
            Assert.AreEqual(Common.ExpectedResultTypes.Value, jsonTest.ExpectedResults[1].ResultType);
            Assert.IsInstanceOfType(jsonTest.ExpectedResults[1].Value, typeof(decimal));
            Assert.AreEqual(-10.245m, jsonTest.ExpectedResults[1].Value);
            Assert.AreEqual(new DateTime(2025, 11, 17, 19, 0, 0), jsonTest.ExpectedResults[1].LastExecution);
            Assert.AreEqual(new TimeSpan(0, 0, 0, 1, 100), jsonTest.ExpectedResults[1].LastDuration);

            // Beispiel String Result
            TestContext.WriteLine("3rd result: Type '{0}' - Value: '{1}' - Timestamp: '{2}' - Duration: '{3}'", jsonTest.ExpectedResults[2].ResultType.ToString(), jsonTest.ExpectedResults[2].Value?.ToString() ?? "null", jsonTest.ExpectedResults[2].LastExecution.ToString(), jsonTest.ExpectedResults[2].LastDuration.ToString());
            Assert.AreEqual(ExpectedResultTypes.Value, jsonTest.ExpectedResults[2].ResultType);
            Assert.AreEqual("Test", jsonTest.ExpectedResults[2].Value);
            Assert.AreEqual(new DateTime(2025, 11, 17, 20, 0, 0), jsonTest.ExpectedResults[2].LastExecution);
            Assert.AreEqual(new TimeSpan(0, 0, 1, 0, 200), jsonTest.ExpectedResults[2].LastDuration);

            // Beispiel einzelnes Resultset
            TestContext.WriteLine("4th result: Type '{0}' - Resultsets count: '{1}'", jsonTest.ExpectedResults[3].ResultType.ToString(), jsonTest.ExpectedResults[3].ResultSets.Tables.Count);
            Assert.AreEqual(ExpectedResultTypes.Resultset, jsonTest.ExpectedResults[3].ResultType);
            Assert.IsNull(jsonTest.ExpectedResults[3].LastExecution);
            Assert.IsNull(jsonTest.ExpectedResults[3].LastDuration);
            Assert.HasCount(2, jsonTest.ExpectedResults[3].ResultSets.Tables);

            // Erste Tabelle des Resultsets prüfen
            // Überschriften prüfen
            Assert.HasCount(3, jsonTest.ExpectedResults[3].ResultSets.Tables[0].Columns);
            Assert.AreEqual("ID", jsonTest.ExpectedResults[3].ResultSets.Tables[0].Columns[0].ColumnName);
            Assert.AreEqual(typeof(int) ,jsonTest.ExpectedResults[3].ResultSets.Tables[0].Columns[0].DataType);
            Assert.AreEqual("Name", jsonTest.ExpectedResults[3].ResultSets.Tables[0].Columns[1].ColumnName);
            Assert.AreEqual(typeof(string), jsonTest.ExpectedResults[3].ResultSets.Tables[0].Columns[1].DataType);
            Assert.AreEqual("Date", jsonTest.ExpectedResults[3].ResultSets.Tables[0].Columns[2].ColumnName);
            Assert.AreEqual(typeof(DateTime), jsonTest.ExpectedResults[3].ResultSets.Tables[0].Columns[2].DataType);

            // Zeilen prüfen
            Assert.HasCount(3, jsonTest.ExpectedResults[3].ResultSets.Tables[0].Rows);

            // Erste Zeile prüfen
            Assert.AreEqual(1, jsonTest.ExpectedResults[3].ResultSets.Tables[0].Rows[0]["ID"]);
            Assert.AreEqual("Alice", jsonTest.ExpectedResults[3].ResultSets.Tables[0].Rows[0]["Name"]);
            Assert.AreEqual(new DateTime(2025, 11, 01), jsonTest.ExpectedResults[3].ResultSets.Tables[0].Rows[0]["Date"]);

            // Zweite Zeile prüfen
            Assert.AreEqual(2, jsonTest.ExpectedResults[3].ResultSets.Tables[0].Rows[1]["ID"]);
            Assert.AreEqual("Bob", jsonTest.ExpectedResults[3].ResultSets.Tables[0].Rows[1]["Name"]);
            Assert.AreEqual(new DateTime(2025, 11, 02), jsonTest.ExpectedResults[3].ResultSets.Tables[0].Rows[1]["Date"]);

            // Dritte Zeile prüfen
            Assert.AreEqual(3, jsonTest.ExpectedResults[3].ResultSets.Tables[0].Rows[2]["ID"]);
            Assert.AreEqual("Charlie", jsonTest.ExpectedResults[3].ResultSets.Tables[0].Rows[2]["Name"]);
            Assert.IsTrue(jsonTest.ExpectedResults[3].ResultSets.Tables[0].Rows[2]["Date"] is DBNull);
        }

        /// <summary>
        /// Test loading of a definition file with an invalid expected resultset.
        /// The data type of a column is invalid (number instead of a string).
        /// </summary>
        [TestMethod]
        public void Test_LoadInvalid_ExpectedResultset_InvalidType()
        {
            Definition jsonTest = new JsonDefinition()
            {
                FileName = new("./Definitionen/Invalid expected resultset - Invalid type.json")
            };

            // Bei der Methode Load wird eine Exception erwartet mit der Nachricht, dass im Resultset ein ungültiger Datentyp verwendet wird.
            Exception result = Assert.ThrowsExactly<InvalidOperationException>(jsonTest.Load);
            Assert.IsInstanceOfType(result.InnerException, typeof(JsonException));
            Assert.StartsWith("Expected String token for Column DataType of ExpectedResultSet.", result.InnerException.Message);
            Assert.AreEqual(21, ((JsonException)result.InnerException).LineNumber);
        }

        /// <summary>
        /// Test loading of a definition file with an invalid expected resultset.
        /// The data type of a row value is invalid (string instead of a number).
        /// </summary>
        [TestMethod]
        public void Test_loadInvalid_ExpectedResultset_InvalidRowValue()
        {
            Definition jsonTest = new JsonDefinition()
            {
                FileName = new("./Definitionen/Invalid expected resultset - Invalid Row Value.json")
            };

            // Bei der Methode Load wird eine Exception erwartet mit der Nachricht, dass im Resultset ein ungültiger Wert verwendet wird, der nicht zur Column DataType passt.
            Exception result = Assert.ThrowsExactly<InvalidOperationException>(jsonTest.Load);
            Assert.IsInstanceOfType(result.InnerException, typeof(JsonException));
            Assert.StartsWith("The JSON value 'Fehler' cannot be converted to the target type 'System.Int32'.", result.InnerException.Message);
            Assert.AreEqual(26, ((JsonException)result.InnerException).LineNumber);
        }

        /// <summary>
        /// Test loading of a definition file with valid input parameter definitions.
        /// </summary>
        [TestMethod]
        public void Test_LoadValid_InputsOnly()
        {
            Definition jsonTest = new JsonDefinition()
            {
                FileName = new("./Definitionen/Valid - inputs only.json")
            };

            jsonTest.Load();

            // Bei der Methode Load sollen alle Werte korrekt gelesen werden.
            TestContext.WriteLine($"Actual count of inputs: {jsonTest.Inputs.Count}");
            Assert.HasCount(3, jsonTest.Inputs);

            // Beispiel Input Parameter 1
            TestContext.WriteLine($"Input 1: Name '{jsonTest.Inputs[0].ParameterName}' - Type: '{jsonTest.Inputs[0].SqlTypeDef}' - UserValues: '{jsonTest.Inputs[0].UserValues.Count}'");
            Assert.AreEqual("Input1", jsonTest.Inputs[0].ParameterName);
            Assert.AreEqual("INT", jsonTest.Inputs[0].SqlTypeDef.ToString());
            Assert.AreEqual(System.Data.ParameterDirection.Input, jsonTest.Inputs[0].Direction);
            Assert.IsTrue(jsonTest.Inputs[0].IsNullable);
            Assert.IsFalse(jsonTest.Inputs[0].HasDefaultValue);
            Assert.HasCount(3, jsonTest.Inputs[0].UserValues);
            Assert.AreEqual((byte)10, jsonTest.Inputs[0].UserValues[0]);
            Assert.AreEqual((byte)20, jsonTest.Inputs[0].UserValues[1]);
            Assert.AreEqual((byte)30, jsonTest.Inputs[0].UserValues[2]);

            // Beispiel Input Parameter 2
            TestContext.WriteLine($"Input 2: Name '{jsonTest.Inputs[1].ParameterName}' - Type: '{jsonTest.Inputs[1].SqlTypeDef}' - UserValues: '{jsonTest.Inputs[1].UserValues.Count}'");
            Assert.AreEqual("Input2", jsonTest.Inputs[1].ParameterName);
            Assert.AreEqual("VARCHAR(50)", jsonTest.Inputs[1].SqlTypeDef.ToString());
            Assert.AreEqual(System.Data.ParameterDirection.Output, jsonTest.Inputs[1].Direction);
            Assert.IsEmpty(jsonTest.Inputs[1].UserValues);


            // Beispiel Input Parameter 3
            TestContext.WriteLine($"Input 3: Name '{jsonTest.Inputs[2].ParameterName}' - Type: '{jsonTest.Inputs[2].SqlTypeDef}' - UserValues: '{jsonTest.Inputs[2].UserValues.Count}'");
            Assert.AreEqual("Input3", jsonTest.Inputs[2].ParameterName);
            Assert.AreEqual("DATE", jsonTest.Inputs[2].SqlTypeDef.ToString());
            Assert.IsFalse(jsonTest.Inputs[2].IsNullable);
            Assert.IsFalse(jsonTest.Inputs[2].HasDefaultValue);
            Assert.HasCount(3, jsonTest.Inputs[2].UserValues);
            Assert.AreEqual(new DateTime(2023, 1, 1), jsonTest.Inputs[2].UserValues[0]);
            Assert.AreEqual(new DateTime(2023, 6, 15), jsonTest.Inputs[2].UserValues[1]);
            Assert.AreEqual(new DateTime(2023, 12, 31), jsonTest.Inputs[2].UserValues[2]);
        }
    }
}
