using System.Net;
using System.Security;

namespace Matrix.MsSql
{
    /// <summary>
    /// This class provides properties and methods to setup a SQL-Server connection intended for the use of the SQL-Matrix-Library.
    /// </summary>
    /// <remarks>
    /// This class encapsulate the essential properties of the Microsoft.Data.SqlClient library.
    /// </remarks>
    public class SetupSqlConnection : IDisposable
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="SetupSqlConnection"/>-class with default values.
        /// </summary>
        public SetupSqlConnection()
        {
            this.SQLServerIP = new()
            {
                HostName = Dns.GetHostName(),
                AddressList = [IPAddress.Loopback, IPAddress.IPv6Loopback]
            };

            this.SQLServerPort = 1433;

            this.SQLServerInstance = string.Empty;

            this.SQLServerLoginName = "sa";

            this._SQLServerLoginPassword = new();
            this._SQLServerLoginPassword.MakeReadOnly();
        }

        #region Implementations

        #region IDisposable
        private bool _disposed = false;

        /// <summary>
        /// <inheritdoc cref="IDisposable.Dispose"/>
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// <inheritdoc cref="IDisposable.Dispose"/>
        /// </summary>
        /// <param name="disposing">If <see langword="true"/> managed resources will be disposed also.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    // Bereinigung verwalteter Code
                    this._SQLServerLoginPassword.Dispose();
                }

                // Manuelle Bereinigung nicht verwalteter Code ohne Dispose Prozedur

                this._disposed = true;
            }
        }

        /// <summary>
        /// Destructor function of this class.
        /// </summary>
        ~SetupSqlConnection()
        {
            Dispose(false);
        }
        #endregion

        #endregion

        #region SQLServerIP

        /// <summary>
        /// Gets or sets a DNS Host Entry of the SQL-Server.
        /// </summary>
        /// <value>
        /// The IP-Adress of the SQL-Server. The default is the loopback-address.
        /// </value>
        /// <remarks>
        /// Use the methods <see cref="ParseSQLServerIP(string, bool)"/> or <see cref="TryParseSQLServerIP(string, bool)"/> to setup the IP-Adress.
        /// </remarks>
        public IPHostEntry SQLServerIP { get; protected set; }

        /// <summary>
        /// Sets the IP-Adress of the SQL-Server.
        /// </summary>
        /// <param name="ServerNameOrIP">
        /// The host-name or IP-Address of the SQL-Server to be evaluated.
        /// If a host-name is given, the option <paramref name="CheckDNS"/> has to set to <see langword="true"/>. Otherwise the evaluation will fail.
        /// If multiple IP-Addresses are available the first IP-Adress where the SQL-Server is listening will be used.
        /// </param>
        /// <param name="CheckDNS">
        /// If <see langword="true"/> a DNS lookup during evaluation will be performed, if the <paramref name="ServerNameOrIP"/> is valid and exists in the network.
        /// </param>
        /// <exception cref="ArgumentException">The parameter <paramref name="ServerNameOrIP"/> is not valid.</exception>
        /// <exception cref="ArgumentNullException">The parameter <paramref name="ServerNameOrIP"/> is empty.</exception>
        public void ParseSQLServerIP (string ServerNameOrIP, bool CheckDNS = false)
        {
            if (!string.IsNullOrWhiteSpace(ServerNameOrIP))
            {
                if (CheckDNS)
                {
                    // Mit DNS Prüfung
                    try
                    {
                        this.SQLServerIP = Dns.GetHostEntry(ServerNameOrIP, System.Net.Sockets.AddressFamily.InterNetwork);
                    }
                    catch (Exception ex)
                    {
                        throw new ArgumentException($"The host {ServerNameOrIP} is not available in the local area network.", nameof(ServerNameOrIP), ex);
                    }
                }
                else
                {
                    // Ohne DNS Prüfung
                    try
                    {
                        IPAddress ServerIP = IPAddress.Parse(ServerNameOrIP);

                        this.SQLServerIP = new()
                        {
                            AddressList = [ServerIP]
                        };
                    }
                    catch (Exception ex)
                    {
                        throw new ArgumentException($"The host {ServerNameOrIP} is not a valid IP-Adress.", nameof(ServerNameOrIP), ex);
                    }
                }
            }
            else
            {
                throw new ArgumentNullException(nameof(ServerNameOrIP), "The requested the server name is empty.");
            }
        }

        /// <summary>
        /// Try to set the IP-Address of the SQL-Server.
        /// </summary>
        /// <param name="ServerNameOrIP"><inheritdoc cref="ParseSQLServerIP(string, bool)" path="/param[@name='ServerNameOrIP']"/></param>
        /// <param name="CheckDNS"><inheritdoc cref="ParseSQLServerIP(string, bool)" path="/param[@name='CheckDNS']"/></param>
        /// <returns><see langword="true"/> if the parameter <paramref name="ServerNameOrIP"/> contains a valid IP-Address or host-name, otherwise <see langword="false"/>.</returns>
        public bool TryParseSQLServerIP (string ServerNameOrIP, bool CheckDNS = false)
        {
            try
            {
                this.ParseSQLServerIP(ServerNameOrIP, CheckDNS);
                return true;
            }
            catch { return false; }
        }

        #endregion

        #region SQLServerPort

        private int _SQLServerPort;

        /// <summary>
        /// Gets or sets the port number where the SQL-Server is listening.
        /// </summary>
        /// <value>
        /// A port number between <see cref="IPEndPoint.MinPort"/> and <see cref="IPEndPoint.MaxPort"/>.
        /// The default value is 1433.
        /// </value>
        /// <exception cref="ArgumentOutOfRangeException">The port number is not in the valid range.</exception>
        public int SQLServerPort
        {
            get { return _SQLServerPort; }
            set
            {
                // Prüfen, ob die Portnummer innerhalb des gültigen Bereiches liegt.
                if (value >= IPEndPoint.MinPort && value <= IPEndPoint.MaxPort)
                {
                    _SQLServerPort = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, $"The port number must be a number between {IPEndPoint.MinPort} and {IPEndPoint.MaxPort}.");
                }
            }
        }

        #endregion

        #region SQLServerInstance
        
        /// <summary>
        /// Gets or sets the name of the SQL-Server instance. This property is required only, if multiple instances are installed on the SQL-Server.
        /// </summary>
        /// <value>The name of the SQL-Server instance.</value>
        public string SQLServerInstance { get; set; }

        #endregion

        #region SQLServerLoginName

        /// <summary>
        /// Gets or sets the login name for the connection with the SQL-Server.
        /// </summary>
        /// <value>
        /// The login name for the connection with the SQL-Server.
        /// The default login name is "sa".
        /// </value>
        /// <remarks>
        /// Only the SQL-Server authentication mode is supported for login.
        /// The selected login name should have sufficient permissions on the SQL-Server.
        /// </remarks>
        public string SQLServerLoginName { get; set; }

        #endregion

        #region SQLServerLoginPassword

        private SecureString _SQLServerLoginPassword;

        /// <summary>
        /// Gets or sets the password for the login to the SQL-Server.
        /// </summary>
        /// <value>A read-only secure string of the login password.</value>
        /// <exception cref="ArgumentException">Will be thrown if the login password is not marked as read-only.</exception>
        public SecureString SQLServerLoginPassword
        {
            get { return _SQLServerLoginPassword; }
            set
            {
                if (value.IsReadOnly())
                {
                    _SQLServerLoginPassword = value;
                }
                else
                {
                    throw new ArgumentException("The login password must be marked as read-only!");
                }
            }
        }

        #endregion
    }
}