using System.Security;
using System.Text.Json;

namespace Matrix.MsSql.Unit
{
    internal class CLI
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <remarks>
        ///     <list type="table">
        ///         <listheader>
        ///             <term>Returncodes</term>
        ///         </listheader>
        ///         <item>
        ///             <description>0</description>
        ///             <description>Succesful execution.</description>
        ///         </item>
        ///         <item>
        ///             <description>1</description>
        ///             <description>Execution cancelled by user.</description>
        ///         </item>
        ///         <item>
        ///             <description>2</description>
        ///             <description>Error during reading the json test file.</description>
        ///         </item>
        ///     </list>
        /// </remarks>
        static void Main(string[] args)
        {
            FileInfo TestFile;
            TestDefinition? definition;
            IPEndPoint SqlEndpoint;

            // Trigger für den Abbruch
            Console.CancelKeyPress += Console_CancelKeyPress;

            Console.Title = "Matrix MS-SQL Unit Test";

            Console.WriteLine("Matrix MS-SQL Unit test started");

            // Dateinamen für die Testdatei abfragen.
            // Wenn im Windows-Explorer die Funkktion "Pfad kopieren" verwendet wird, wird der Dateiname in " eingeschlossen.
            string TestFileName;
            do
            {
                Console.WriteLine("Please enter the filename of the json test-file:");
                TestFileName = Console.ReadLine() ?? string.Empty;

                // Datei angegeben?
                if (string.IsNullOrWhiteSpace(TestFileName))
                {
                    Console.WriteLine("No input. Try again.");
                    continue;
                }

                // Existiert die Datei?
                if (!File.Exists(TestFileName.Trim('"').ToString()))
                {
                    Console.WriteLine("File does not exist. Try again.");
                    continue;
                }
                else
                {
                    // Versuche Datei Infos abzufragen
                    try
                    {
                        TestFile = new(TestFileName.Trim('"').ToString());

                    }
                    catch (SecurityException ex)
                    {
                        Console.WriteLine("WARNING: Access to the test file denied! Try again.");
                        Console.WriteLine(ex.Message);
                        continue;
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        Console.WriteLine("WARNING: Access to the test file denied!");
                        Console.WriteLine(ex.Message);
                        continue;
                    }
                    catch (PathTooLongException ex)
                    {
                        Console.WriteLine("WARNING: Full path name to the test file too long!");
                        Console.WriteLine(ex.Message);
                        continue;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        continue;
                    }

                    // Datei vorhanden und lesbar
                    break;
                }
            }
            while (true);

            // Testdatei schreibgeschützt?
            if (TestFile.IsReadOnly)
            {
                Console.WriteLine("NOTICE: The selected File is read only. No Updates on the test file will be performed.");
            }

            Console.Title = string.Concat("Matrix MS-SQL Unit Test - ", TestFile.Name);

            // JSON-Daten lesen
            try
            {
                definition = JsonSerializer.Deserialize<TestDefinition>(TestFile.Open(FileMode.Open, FileAccess.Read));

                if (definition == null)
                {
                    Console.WriteLine("The test file could not be read. Execution is cancelled.");
                    Environment.Exit(2);
                    
                }
                definition.FileName = TestFile;

                Console.WriteLine("Test file is valid.");
                Console.WriteLine($"Test Object: {definition.TestObjectType} {definition.SchemaName}.{definition.TestObjectName}");
            }
            catch (JsonException ex)
            {
                Console.WriteLine("WARNING: The test file contains invalid json code - or - required fields are missing!");
                Console.WriteLine(ex.Message);
                Environment.Exit(2);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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