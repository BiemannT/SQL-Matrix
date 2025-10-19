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
        ///         <item>
        ///             <description>4</description>
        ///             <description>Error during publishing dacpac-file.</description>
        ///         </item>
        ///     </list>
        /// </remarks>
        static void Main(string[] args)
        {
            TestDefinition Definition;
            SetupDacService SqlConn;

            // Trigger für den Abbruch
            Console.CancelKeyPress += Console_CancelKeyPress;

            Console.Title = "Matrix MS-SQL Unit Test";

            Console.WriteLine("Matrix MS-SQL Unit test started");

            // Dateinamen für die Testdatei abfragen und einlesen.
            try
            {
                Definition = PrepareTestDefinition();
                Console.Title = string.Concat("Matrix MS-SQL Unit Test - ", Definition.FileName.Name);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Environment.Exit(2);
                return;
            }

            // Test aufbereiten
            try
            {
                Definition.BuildTestCases();
                Console.WriteLine($"Test definition with {Definition.TestCases.Count} test cases prepared successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            // Verbindung zum SQL-Server herstellen
            try
            {
                Console.WriteLine("Setup SQL-Connection");
                SqlConn = PrepareSqlConnetion();
                Console.WriteLine($"Connection to the SQL-Server {SqlConn.SQLServerIP.HostName} established successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Environment.Exit(3);
                return;
            }

            // DACPAC-Datei laden und auf dem SQL-Server veröffentlichen
            try
            {
                PrepareDacServices(ref SqlConn);
                Console.WriteLine("Setup test-database on the SQL-Server.");
                SqlConn.ProgressMessage += SqlConn_ProgressMessage;
                SqlConn.PublishDacPackage(string.Concat(SqlConn.DacName, " test"));
                Console.WriteLine($"Database {SqlConn.Database} published successsfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Environment.Exit(4);
                return;
            }

            SqlConn.Dispose();
            Console.ReadLine();
        }

        private static void SqlConn_ProgressMessage(object sender, DacPublishEventArgs e)
        {
            // Dacpac Veröffentlichung protokollieren
            Console.WriteLine(e.ToString());
        }

        private static void Console_CancelKeyPress(object? sender, ConsoleCancelEventArgs e)
        {
            // Ausführung beenden und Kommandozeilenfenster schließen.
            Environment.Exit(1);
        }
    }
}