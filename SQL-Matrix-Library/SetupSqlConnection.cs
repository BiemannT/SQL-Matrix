using Microsoft.Data.SqlClient;
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
            this._SQLServerIP = new()
            {
                HostName = Dns.GetHostName(),
                AddressList = [IPAddress.Loopback, IPAddress.IPv6Loopback]
            };

            this._SQLServerPort = 1433;

            this._SQLServerInstance = string.Empty;

            this._SQLServerLoginName = "sa";

            this._SQLServerLoginPassword = new();
            this._SQLServerLoginPassword.MakeReadOnly();

            this._ConnectRetryCount = 3;
            this._ConnectRetryInterval = new(0, 0, 5);
            this._ConnectTimeout = new(0, 0, 10);

            this._Database = "master";
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

        #region CheckConnection

        /// <summary>
        /// Represents if the Sql-Connection is already tested.
        /// </summary>
        /// <remarks>Will be set to false, if any property of this class will be changed.</remarks>
        private bool _CheckConnection = false;

        /// <summary>
        /// Checks if a connection to the SQL-Server can be established with the given parameters.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Will be thrown, if no connection can be established to the SQL-Server.
        /// Check, if all parameters are valid and try this method again.
        /// </exception>
        public void CheckConnection()
        {
            // Check bereits durchgeführt?
            if (this._CheckConnection) { return; }

            // Passwort gesetzt?
            if (this._SQLServerLoginPassword.Length == 0)
            {
                throw new InvalidOperationException($"Property {nameof(this.SQLServerLoginPassword)} not set!");
            }

            // Mindestens 1 IP-Adresse verfügbar?
            if (this.SQLServerIP.AddressList.Length == 0)
            {
                throw new InvalidOperationException($"The IP-Adress list in the Property {nameof(this.SQLServerIP)} is empty!");
            }

            // Verfügbare IP-Adresse durchtesten
            foreach (var item in this._SQLServerIP.AddressList)
            {
                SqlConnectionStringBuilder builder = new()
                {
                    Authentication = SqlAuthenticationMethod.SqlPassword,
                    ConnectRetryCount = 2,
                    ConnectRetryInterval = 10,
                    ConnectTimeout = 10,
                    CommandTimeout = 10,
                    DataSource = $"tcp:{item},{this._SQLServerPort}{(string.IsNullOrWhiteSpace(this._SQLServerInstance) ? "" : string.Concat("\\", this._SQLServerInstance))}",
                    InitialCatalog = this._Database,
                    IPAddressPreference = SqlConnectionIPAddressPreference.IPv4First,
                    IntegratedSecurity = false,
                    PersistSecurityInfo = false,
                    TrustServerCertificate = true
                };

                SqlCredential cred = new(this._SQLServerLoginName, this._SQLServerLoginPassword);

                using SqlConnection SqlConn = new(builder.ConnectionString, cred);

                try
                {
                    SqlConn.Open();

                    // Verbindung zum SQL-Server konnte hergestellt werden.
                    this._SQLServerIP.AddressList = [item];
                    this._CheckConnection = true;
                    break;
                }
                catch
                {
                    // Unter dieser IP-Adresse antwortet der SQL-Server nicht. Nächste Adresse versuchen.
                    continue;
                }
            }

            // Wenn bis hierhin keine IP-Adresse gültig war Fehlermeldung ausgeben
            if (this._CheckConnection == false)
            {
                throw new InvalidOperationException("Failed to connect to the SQL-Server.");
            }
        }

        #endregion

        #region SQLServerIP

        private IPHostEntry _SQLServerIP;

        /// <summary>
        /// Gets or sets a DNS Host Entry of the SQL-Server.
        /// </summary>
        /// <value>
        /// The IP-Adress of the SQL-Server. The default is the loopback-address.
        /// </value>
        /// <remarks>
        /// Use the methods <see cref="ParseSQLServerIP(string, bool)"/> or <see cref="TryParseSQLServerIP(string, bool)"/> to setup the IP-Adress.
        /// </remarks>
        public IPHostEntry SQLServerIP
        {
            get => this._SQLServerIP;
            protected set
            {
                // Wurde die IP-Adressliste geändert?
                if (!value.AddressList.Equals(this._SQLServerIP.AddressList))
                {
                    this._CheckConnection = false;
                }
                this._SQLServerIP = value;
            }
        }

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
            get => this._SQLServerPort;
            set
            {
                // Prüfen, ob die Portnummer innerhalb des gültigen Bereiches liegt.
                if (value >= IPEndPoint.MinPort && value <= IPEndPoint.MaxPort)
                {
                    // Wurde die Port-Nummer geändert?
                    if (!value.Equals(this._SQLServerPort))
                    {
                        this._CheckConnection = false;
                    }
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

        private string _SQLServerInstance;

        /// <summary>
        /// Gets or sets the name of the SQL-Server instance. This property is required only, if multiple instances are installed on the SQL-Server.
        /// </summary>
        /// <value>The name of the SQL-Server instance.</value>
        public string SQLServerInstance
        {
            get => this._SQLServerInstance;
            set
            {
                // Wurde die Instanz geändert?
                if (!value.Equals(this._SQLServerInstance))
                {
                    this._CheckConnection = false;
                }
                this._SQLServerInstance = value;
            }
        }

        #endregion

        #region SQLServerLoginName

        private string _SQLServerLoginName;

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
        public string SQLServerLoginName
        {
            get => this._SQLServerLoginName;
            set
            {
                // Login Namen geändert?
                if (!value.Equals(this._SQLServerLoginName))
                {
                    this._CheckConnection = false;
                }
                this._SQLServerLoginName = value;
            }
        }

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
            get => this.SQLServerLoginPassword;
            set
            {
                if (value.IsReadOnly())
                {
                    _SQLServerLoginPassword = value;
                    this._CheckConnection = false;
                }
                else
                {
                    throw new ArgumentException("The login password must be marked as read-only!");
                }
            }
        }

        #endregion

        #region ConnectionFailureParameters

        private int _ConnectRetryCount;

        /// <summary>
        /// The number of reconnections attempted after identifying that there was an idle connection failure.
        /// This must be an integer between 0 and 255.
        /// Default value is 3.
        /// </summary>
        /// <value>Number of retry connections.</value>
        /// <exception cref="ArgumentException">The number must be between 0 and 255.</exception>
        public int ConnectRetryCount
        {
            get => this._ConnectRetryCount;
            set
            {
                if (value < 0 || value > 255)
                {
                    throw new ArgumentException($"The value of the {nameof(this.ConnectRetryCount)} must be between 0 and 255.");
                }
                else
                {
                    // Eine Änderung dieses Parameters erfordert nicht die erneute Prüfung, ob die Verbindung noch gültig ist.
                    this._ConnectRetryCount = value;
                }
            }
        }

        private TimeSpan _ConnectRetryInterval;

        /// <summary>
        /// Amount of time between each reconnection attempt after identifying that there was an idle connection failure.
        /// This must be a timespan between 1 and 60 seconds. The default is 5 seconds.
        /// </summary>
        /// <value>Amount of time (in seconds) between each reconnection attempt after identifying that there was an idle connection failure.</value>
        /// <exception cref="ArgumentException">The time span must be between 1 and 60 seconds.</exception>
        public TimeSpan ConnectRetryInterval
        {
            get => this._ConnectRetryInterval;
            set
            {
                if (Math.Floor(value.TotalSeconds) < 1 || Math.Ceiling(value.TotalSeconds) > 60)
                {
                    throw new ArgumentException($"The value of the {nameof(this.ConnectRetryInterval)} must be between 1 and 60 seconds.");
                }
                else
                {
                    // Eine Änderung dieses Parameters erfordert nicht die erneute Prüfung, ob die Verbindung noch gültig ist.
                    this._ConnectRetryInterval = value;
                }
            }
        }

        private TimeSpan _ConnectTimeout;

        /// <summary>
        /// Gets or sets the length of time (in seconds) to wait for a connection to the server before terminating the attempt and generating an error.
        /// </summary>
        /// <value>The timespan to wait for a connection. The default value is 10 seconds.</value>
        /// <exception cref="ArgumentException">The time span must be greater than 1 second.</exception>
        public TimeSpan ConnectTimeout
        {
            get => this._ConnectTimeout;
            set
            {
                if (Math.Floor(value.TotalSeconds) < 1)
                {
                    throw new ArgumentException($"The value of the {nameof(this.ConnectTimeout)} must be greater than 1 second.");
                }
                else
                {
                    // Eine Änderung dieses Parameters erfordert nicht die erneute Prüfung, ob die Verbindung noch gültig ist.
                    this._ConnectTimeout = value;
                }
            }
        }

        #endregion

        #region Database

        private string _Database;

        /// <summary>
        /// Gets or sets the initial database name for the connection with the SQL-Server.
        /// </summary>
        /// <value>
        /// The name of the database to be connected.
        /// The default database is "master".
        /// </value>
        public string Database
        {
            get => this._Database;
            set
            {
                // Datenbanknamen geändert?
                if (!value.Equals(this._Database))
                {
                    this._CheckConnection = false;
                }
                this._Database = value;
            }
        }

        #endregion

        #region GetConnectionString

        /// <summary>
        /// Prepare a Sql Connection String with the given parameters from this instance.
        /// </summary>
        /// <returns>Returns an instance of <see cref="SqlConnectionStringBuilder"/> with the parameters from this instance, if the connection is valid.</returns>
        /// <exception cref="InvalidOperationException">Will be thrown, if the connection to the SQL-Server failed with the given parameter.</exception>
        protected SqlConnectionStringBuilder GetConnectionString()
        {
            // Falls noch keine Prüfung durchgeführt wurde diese zuerst ausführen.
            if (this._CheckConnection)
            {
                try
                {
                    this.CheckConnection();
                }
                catch (InvalidOperationException)
                {
                    throw;
                }
            }

            return new SqlConnectionStringBuilder()
            {
                Authentication = SqlAuthenticationMethod.SqlPassword,
                ConnectRetryCount = this._ConnectRetryCount,
                ConnectRetryInterval = (int)this._ConnectRetryInterval.TotalSeconds,
                ConnectTimeout = (int)this._ConnectTimeout.TotalSeconds,
                DataSource = $"tcp:{this._SQLServerIP.AddressList[0]},{this._SQLServerPort}{(string.IsNullOrWhiteSpace(this._SQLServerInstance) ? "" : string.Concat("\\", this._SQLServerInstance))}",
                InitialCatalog = _Database,
                IPAddressPreference = SqlConnectionIPAddressPreference.IPv4First,
                IntegratedSecurity = false,
                PersistSecurityInfo = false,
                TrustServerCertificate = true
            };
        }

        #endregion
    }
}