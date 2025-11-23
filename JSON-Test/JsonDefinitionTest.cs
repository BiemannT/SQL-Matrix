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

            Exception result = Assert.ThrowsException<InvalidOperationException>(jsonTest.Load);

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
            Exception result = Assert.ThrowsException<InvalidOperationException>(jsonTest.Load);

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
            Exception result = Assert.ThrowsException<NotSupportedException>(jsonTest.Load);

            TestContext.WriteLine("Actual load exception message: {0}", result.Message);

            StringAssert.StartsWith(result.Message, "The API-Version 'v0' is not supported.");
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
            Exception result = Assert.ThrowsException<ArgumentOutOfRangeException>(jsonTest.Load);

            TestContext.WriteLine("Actual load exception message: {0}", result.Message);

            StringAssert.StartsWith(result.Message, "MaxExecutionTime must be a positive integer.");
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
            Assert.AreEqual(0, jsonTest.Inputs.Count);
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
            Assert.AreEqual(4, jsonTest.ExpectedResults.Count);

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
            Assert.IsTrue(jsonTest.ExpectedResults[3].LastExecution is null);
            Assert.IsTrue(jsonTest.ExpectedResults[3].LastDuration is null);
            Assert.AreEqual(2, jsonTest.ExpectedResults[3].ResultSets.Tables.Count);

            // Erste Tabelle des Resultsets prüfen
            // Überschriften prüfen
            Assert.AreEqual(3, jsonTest.ExpectedResults[3].ResultSets.Tables[0].Columns.Count);
            Assert.AreEqual("ID", jsonTest.ExpectedResults[3].ResultSets.Tables[0].Columns[0].ColumnName);
            Assert.AreEqual(typeof(int) ,jsonTest.ExpectedResults[3].ResultSets.Tables[0].Columns[0].DataType);
            Assert.AreEqual("Name", jsonTest.ExpectedResults[3].ResultSets.Tables[0].Columns[1].ColumnName);
            Assert.AreEqual(typeof(string), jsonTest.ExpectedResults[3].ResultSets.Tables[0].Columns[1].DataType);
            Assert.AreEqual("Date", jsonTest.ExpectedResults[3].ResultSets.Tables[0].Columns[2].ColumnName);
            Assert.AreEqual(typeof(DateTime), jsonTest.ExpectedResults[3].ResultSets.Tables[0].Columns[2].DataType);

            // Zeilen prüfen
            Assert.AreEqual(3, jsonTest.ExpectedResults[3].ResultSets.Tables[0].Rows.Count);

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
    }
}
