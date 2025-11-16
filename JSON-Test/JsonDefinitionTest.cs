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
    }
}
