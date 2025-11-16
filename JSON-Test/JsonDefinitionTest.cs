using System.Text.Json;

namespace BiemannT.MUT.MsSql.Def.JSON.Test
{
    [TestClass]
    public sealed class JsonDefinitionTest
    {
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

            Assert.AreEqual("The test file definition contains invalid JSON code - or - required fields are missing!", result.Message);

            Assert.IsInstanceOfType(result.InnerException, typeof(JsonException));

        }
    }
}
