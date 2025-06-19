using System.Net;
using System.Security;

namespace Matrix.MsSql.Unit
{
    internal partial class CLI
    {
        /// <summary>
        /// Requests on the console the necessary information to setup the SQL-Server connection.
        /// </summary>
        /// <returns>Returns an instance of <see cref="SetupSqlConnection"/> with the gathered information.</returns>
        private static SetupSqlConnection PrepareSqlConnetion ()
        {
            SetupSqlConnection SqlConn = new();
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

            return SqlConn;
        }
    }
}