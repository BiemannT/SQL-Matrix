using System.Security;
using System.Text.Json;

namespace Matrix.MsSql.Unit
{
    internal class CLI
    {
        static void Main(string[] args)
        {
            string TestFileName = string.Empty;
            FileInfo TestFile;

            Console.CancelKeyPress += Console_CancelKeyPress;

            Console.Title = "Matrix MS-SQL Unit Test";

            Console.WriteLine("Matrix MS-SQL Unit test started");

            // Dateinamen für die Testdatei abfragen.
            // Wenn im Windows-Explorer die Funkktion "Pfad kopieren" verwendet wird, wird der Dateiname in " eingeschlossen.
            while (string.IsNullOrWhiteSpace(TestFileName) || !File.Exists(TestFileName.Trim('"').ToString()))
            {
                Console.WriteLine("Please enter the filename of the json test-file:");
                TestFileName = Console.ReadLine() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(TestFileName) || !File.Exists(TestFileName.Trim('"').ToString()))
                {
                    Console.WriteLine("Invalid Filename. Try again.");
                } 
            }

            // Datei Infos prüfen
            try
            {
                TestFile = new(TestFileName.Trim('"').ToString());

                if (TestFile.IsReadOnly)
                {
                    Console.WriteLine("NOTICE: The selected File is read only. No Updates on the test file will be performed.");
                }

                Console.Title = string.Concat("Matrix MS-SQL Unit Test - ", TestFile.Name);

                string TestJson = File.ReadAllText(TestFile.FullName);
                TestDefinition test = JsonSerializer.Deserialize<TestDefinition>(TestJson)!;
                test.FileName = TestFile;

                Console.WriteLine($"Test Object: {test.TestObjectType} {test.SchemaName}.{test.TestObjectName}");
            }
            catch (SecurityException)
            {
                Console.WriteLine("WARNING: Access to the test file denied!");
                Environment.Exit(2);
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("WARNING: Access to the test file denied!");
                Environment.Exit(2);
            }
            catch (PathTooLongException)
            {
                Console.WriteLine("WARNING: Full path name to the test file too long!");
                Environment.Exit(2);
            }
            catch (JsonException)
            {
                Console.WriteLine("WARNING: The test file contains invalid json code - or - required fields are missing!");
                Environment.Exit(2);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Environment.Exit(2);
            }

            Console.ReadLine();
        }

        private static void Console_CancelKeyPress(object? sender, ConsoleCancelEventArgs e)
        {
            // Ausführung beenden und Kommandozeilenfenster schließen.
            Environment.Exit(1);
        }
    }
}