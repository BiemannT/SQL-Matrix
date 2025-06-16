using Microsoft.Data.SqlClient;
using System.Net;
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
            IPEndPoint SqlEndpoint;
            string SqlInstance;
            string SqlLoginName;
            SecureString SqlLoginPassword;
            SqlCredential SqlCredential;
            SqlConnectionStringBuilder SqlConnBuilder;

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

            // Verbindung zum SQL-Server herstellen
            Console.WriteLine("Setup SQL-Connection");

            // Server Hostname
            string SqlHost;
            IPHostEntry SqlHostEntry = new();

            do
            {
                Console.WriteLine("Enter Server Hostname or IPv4 Address:");
                SqlHost = Console.ReadLine() ?? string.Empty;
                    
                if (!string.IsNullOrWhiteSpace(SqlHost))
                {
                    try
                    {
                        SqlHostEntry = Dns.GetHostEntry(SqlHost, System.Net.Sockets.AddressFamily.InterNetwork);
                    }
                    catch
                    {
                        Console.WriteLine($"Could not find {SqlHost}.");
                        SqlHost = string.Empty;
                    }
                }
                else
                {
                    Console.WriteLine("No input. Try again.");
                    SqlHost = string.Empty;
                }
            }
            while (string.IsNullOrWhiteSpace(SqlHost));

            // Server Port
            do
            {
                Console.WriteLine("Enter the port number on which the SQL-Server is listening (default: 1433):");
                if (int.TryParse(Console.ReadLine(), out int SqlPort))
                {
                    try
                    {
                        SqlEndpoint = new IPEndPoint(SqlHostEntry.AddressList[0], SqlPort);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        Console.WriteLine($"The port number {SqlPort} is not supported. Try again.");
                        continue;
                    }

                    // SQL-Server Adresse erfolgreich gesetzt
                    break;
                }
                else
                {
                    Console.WriteLine("The entered value is not a valid number. Try again.");
                    continue;
                }

            } while (true);

            // Optional: Server Instanz Name
            Console.WriteLine("Enter optionally the SQL-Server instance name. Left blank if no named instances installed.");
            SqlInstance = Console.ReadLine() ?? string.Empty;

            // SQL Anmeldename
            Console.WriteLine("Enter the login name (Default name 'sa', if left blank):");
            SqlLoginName = Console.ReadLine() ?? "sa";

            // SQL Login Passwort
            ConsoleKeyInfo key;
            SqlLoginPassword = new();
            Console.WriteLine($"Enter the password for the login name {SqlLoginName}:");
            do
            {
                key = Console.ReadKey(true);
                // Nur druckbare Zeichen zulassen
                if (((int) key.Key) >= 32)
                {
                    SqlLoginPassword.AppendChar(key.KeyChar);
                    Console.Write("*");
                }

            } while (key.Key != ConsoleKey.Enter);
            SqlLoginPassword.MakeReadOnly();
            Console.Write(Environment.NewLine);

            // SQL-Connection aufbereiten
            SqlCredential = new(SqlLoginName, SqlLoginPassword);
            SqlConnBuilder = new()
            {
                Authentication = SqlAuthenticationMethod.SqlPassword,
                ConnectRetryCount = 3,
                ConnectRetryInterval = 5,
                ConnectTimeout = 10,
                DataSource = $"tcp:{SqlEndpoint.Address},{SqlEndpoint.Port}{(string.IsNullOrWhiteSpace(SqlInstance) ? "" : string.Concat("\\", SqlInstance))}",
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

                cmd.Connection.Open();
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch
                {
                    Console.WriteLine("WARNING: Failed to connect to the SQL-Server!");
                    Environment.Exit(3);
                }
            }
            Console.WriteLine($"Connection to the SQL-Server {SqlHostEntry.HostName} established successfully.");

            SqlLoginPassword.Dispose();
            Console.ReadLine();
        }

        private static void Console_CancelKeyPress(object? sender, ConsoleCancelEventArgs e)
        {
            // Ausführung beenden und Kommandozeilenfenster schließen.
            Environment.Exit(1);
        }
    }
}