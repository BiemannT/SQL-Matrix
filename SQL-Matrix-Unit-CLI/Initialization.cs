using System.Net;
using System.Security;
using System.Text.Json;

namespace Matrix.MsSql.Unit
{
    internal partial class CLI
    {
        /// <summary>
        /// Requests on the console the json test-file and read the content.
        /// </summary>
        /// <returns>Returns an instance of <see cref="TestDefinition"/> with the information readed from the test file.</returns>
        /// <exception cref="Exception">If an error occured during reading the json file an exception will be thrown.</exception>
        private static TestDefinition PrepareTestDefinition()
        {
            TestDefinition definition;
            string input;

            // Dateinamen für die Testdatei abfragen
            // Wenn im Windows Explorer die Funktion "Pfad kopieren" verwendet wird, wird der Dateipfad in '"' eingeschlossen.
            do
            {
                Console.WriteLine("Enter the filename of the json test-file:");
                input = Console.ReadLine() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("No input. Try again.");
                    continue;
                }

                // Existiert die Datei
                if (File.Exists(input.Trim('"').ToString()))
                {
                    FileInfo testFile;

                    try
                    {
                        testFile = new(input.Trim('"').ToString());
                    }
                    catch (SecurityException)
                    {
                        Console.WriteLine("WARNING: Access to the test file denied! Try again.");
                        continue;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        Console.WriteLine("WARNING: Access to the test file denied!");
                        continue;
                    }
                    catch (PathTooLongException)
                    {
                        Console.WriteLine("WARNING: Full path name to the test file too long!");
                        continue;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        continue;
                    }

                    // Testdatei schreibgeschützt?
                    if (testFile.IsReadOnly)
                    {
                        Console.WriteLine("NOTICE: The selected test file is read-only. No updates on the test file will be performed.");
                    }

                    // JSON-Daten lesen
                    try
                    {
                        definition = JsonSerializer.Deserialize<TestDefinition>(testFile.Open(FileMode.Open, FileAccess.Read)) ?? throw new Exception("Failure during reading the test file occured. Execution is cancelled.");
                        definition.FileName = testFile;

                        Console.WriteLine($"Test file valid. Test Object: {definition.TestObjectType} {definition.SchemaName}.{definition.TestObjectName}");

                        // Testfälle generieren
                        definition.BuildTestCases();
                        Console.WriteLine($"Test definition with {definition.TestCases.Count} test cases prepared successfully.");
                    }
                    catch (JsonException ex)
                    {
                        Console.WriteLine(ex.Message);
                        throw new Exception("WARNING: The test file contains invalid json code - or - required fields are missing!");
                    }
                    catch (Exception)
                    {
                        throw;
                    }

                    // Datei existiert und lesbar
                    break;
                }
                else
                {
                    Console.WriteLine("File does not exist. Try again.");
                    continue;
                }
            } while (true);

            return definition;
        }

        /// <summary>
        /// Requests on the console the necessary information to setup the SQL-Server connection.
        /// </summary>
        /// <returns>Returns an instance of <see cref="SetupDacService"/> with the gathered information.</returns>
        /// <exception cref="InvalidOperationException">Will be thrown if it is not possible to establish a connection to the SQL-Server with the given parameters.</exception>
        private static SetupDacService PrepareSqlConnetion ()
        {
            SetupDacService SqlConn = new();
            string input;

            #region Server name
            // Server Namen oder IP-Adresse abfragen und validieren
            do
            {
                Console.WriteLine("Enter the Server Host name or IPv4 address of the SQL-Server. Left blank, if the SQL-Server is installed on the local machine.");
                input = Console.ReadLine () ?? string.Empty;

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine($"Use local host: {SqlConn.SQLServerIP.HostName}");
                    break;
                }
                else
                {
                    if (SqlConn.TryParseSQLServerIP(input, true))
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine($"Failed to connect to {input}! Try again.");
                        continue;
                    }
                }

            } while (true);
            #endregion

            #region Server port
            // Server Port Nummer abfragen und validieren
            do
            {
                Console.WriteLine($"Enter the port number where the SQL-Server is listening. Left blank to use the default port number {SqlConn.SQLServerPort}.");
                input = Console.ReadLine() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine($"Default port number {SqlConn.SQLServerPort} will be used.");
                    break;
                }
                else
                {
                    if (int.TryParse(input, out int result))
                    {
                        try
                        {
                            SqlConn.SQLServerPort = result;
                            break;
                        }
                        catch
                        {
                            Console.WriteLine($"The port number {input} is not valid. The port number must be between {IPEndPoint.MinPort} and {IPEndPoint.MaxPort}! Try again.");
                            continue;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"The entered value is not a number. Try again.");
                        continue;
                    }
                }

            } while (true);
            #endregion

            #region Instance name
            // Optional den Server Instanznamen abfragen
            Console.WriteLine("Enter optionally the SQL-Server instance name. Left blank if no named instances installed.");
            input = Console.ReadLine() ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(input))
            {
                SqlConn.SQLServerInstance = input;
            }
            #endregion

            #region Login name
            // Login Namen abfragen
            do
            {
                Console.WriteLine($"Enter the login name for the connection with the SQL-Server. Left blank to use the default login name \"{SqlConn.SQLServerLoginName}\".");
                input = Console.ReadLine() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine($"Default login name \"{SqlConn.SQLServerLoginName}\" will be used for the connection.");
                    break;
                }
                else
                {
                    SqlConn.SQLServerLoginName = input;
                    break;
                }

            } while (true);
            #endregion

            #region Login password
            // Login Passwort abfragen
            SecureString secure = new();
            ConsoleKeyInfo key;
            const string AllowedSpecialChars = "^!\"§$%&/()=?{[]}\\+*~#',;.:-_<>|@";
            Console.WriteLine($"Enter the password for the login name \"{SqlConn.SQLServerLoginName}\":");

            do
            {
                key = Console.ReadKey(true);
                // Nur druckbare Zeichen zulassen
                if (char.IsLetterOrDigit(key.KeyChar) || AllowedSpecialChars.Contains(key.KeyChar))
                {
                    secure.AppendChar(key.KeyChar);
                    Console.Write("*");
                }

                if (key.Key == ConsoleKey.Escape)
                {
                    Console.Write(Environment.NewLine);
                    Console.WriteLine("Escape pressed. Enter the password again:");
                    secure.Clear();
                }
            } while (key.Key != ConsoleKey.Enter);

            secure.MakeReadOnly();
            SqlConn.SQLServerLoginPassword = secure;
            Console.Write(Environment.NewLine);
            #endregion

            // Test the connection
            try
            {
                SqlConn.CheckConnection();
            }
            catch (InvalidOperationException)
            {
                throw;
            }

            return SqlConn;
        }

        private static void PrepareDacServices (ref SetupDacService sqlSetup)
        {
            string input;

            // Dacpac-Datei abfragen und validieren
            do
            {
                Console.WriteLine("Enter the filename of the DACPAC-package file:");
                input = Console.ReadLine() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("No input. Try again.");
                    continue;
                }
                else
                {
                    // Datei laden
                    try
                    {
                        sqlSetup.DacFile = new(input.Trim('"').ToString());
                    }
                    catch (SecurityException)
                    {
                        Console.WriteLine("WARNING: Access to the dacpac file denied!. Try again.");
                        continue;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        Console.WriteLine("WARNING: Access to the dacpac file denied! Try again.");
                        continue;
                    }
                    catch (PathTooLongException)
                    {
                        Console.WriteLine("WARNING: Full path name to the dacpac file too long!");
                        continue;
                    }
                    catch (InvalidOperationException ex)
                    {
                        Console.WriteLine(ex.Message);
                        continue;
                    }
                    catch
                    {
                        throw;
                    }

                    Console.WriteLine($"DACPAC-Package {sqlSetup.DacName} loaded.");
                    break;
                }
            } while (true);
        }
    }
}