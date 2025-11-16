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
    }
}
