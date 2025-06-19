using Microsoft.Data.SqlClient;
using System.Net;
using System.Security;
using System.Text.Json;

namespace Matrix.MsSql.Unit
{
    internal partial class CLI
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
        ///         <item>
        ///             <description>3</description>
        ///             <description>Error during SQL connections.</description>
        ///         </item>
        ///     </list>
        /// </remarks>
        static void Main(string[] args)
        {
            FileInfo TestFile;
            TestDefinition? definition = new();
            SqlCredential SqlCredential;
            SqlConnectionStringBuilder SqlConnBuilder;
            SetupSqlConnection SqlConn;

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
            } while (true);

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

            // Verbindung zum SQL-Server herstellen
            Console.WriteLine("Setup SQL-Connection");
            SqlConn = PrepareSqlConnetion();

            // SQL-Connection aufbereiten
            SqlCredential = new(SqlConn.SQLServerLoginName, SqlConn.SQLServerLoginPassword);
            SqlConnBuilder = new()
            {
                Authentication = SqlAuthenticationMethod.SqlPassword,
                ConnectRetryCount = 3,
                ConnectRetryInterval = 5,
                ConnectTimeout = 10,
                DataSource = $"tcp:{SqlConn.SQLServerIP.AddressList[0]},{SqlConn.SQLServerPort}{(string.IsNullOrWhiteSpace(SqlConn.SQLServerInstance) ? "" : string.Concat("\\", SqlConn.SQLServerInstance))}",
                IPAddressPreference = SqlConnectionIPAddressPreference.IPv4First,
                IntegratedSecurity = false,
                PersistSecurityInfo = false,
                TrustServerCertificate = true
            };

            definition.SqlConn = new(SqlConnBuilder.ConnectionString, SqlCredential);
            using (definition.SqlConn)
            {
                string TestConnCmd = "SELECT GETDATE();";
                SqlCommand cmd = new(TestConnCmd, definition.SqlConn)
                {
                    CommandTimeout = 10
                };

                try
                {
                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("WARNING: Failed to connect to the SQL-Server!");
                    Console.WriteLine(ex.Message);
                    Environment.Exit(3);
                }
            }
            Console.WriteLine($"Connection to the SQL-Server {SqlConn.SQLServerIP.HostName} established successfully.");

            // Dateinamen der DACPAC-Datei abfragen.
            string DacFileName;
            do
            {
                Console.WriteLine("Enter the filename of the DACPAC-package file:");
                DacFileName = Console.ReadLine() ?? string.Empty;

                // Datei angegeben?
                if (string.IsNullOrWhiteSpace(DacFileName))
                {
                    Console.WriteLine("No input. Try again.");
                    continue;
                }

                // Existiert die Datei?
                if (!File.Exists(DacFileName.Trim('"').ToString()))
                {
                    Console.WriteLine("File does not exist. Try again.");
                    continue;
                }
                else
                {
                    // Versuche Datei Infos abzufragen
                    try
                    {
                        definition.DacpacFile = new(DacFileName.Trim('"').ToString());
                    }
                    catch (SecurityException ex)
                    {
                        Console.WriteLine("WARNING: Access to the dacpac file denied! Try again.");
                        Console.WriteLine(ex.Message);
                        continue;
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        Console.WriteLine("WARNING: Access to the dacpac file denied! Try again.");
                        Console.WriteLine(ex.Message);
                        continue;
                    }
                    catch (PathTooLongException ex)
                    {
                        Console.WriteLine("WARNING: Full path name to the dacpac file too long!");
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

            Console.ReadLine();
        }

        private static void Console_CancelKeyPress(object? sender, ConsoleCancelEventArgs e)
        {
            // Ausführung beenden und Kommandozeilenfenster schließen.
            Environment.Exit(1);
        }
    }
}