using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Dac;

namespace Matrix.MsSql
{
    /// <summary>
    /// This class extends the properties and methods of the <see cref="SetupSqlConnection"/>-class
    /// to setup a DAC service for the SQL-Server intended for the use of the SQL-Matrix-Library.
    /// </summary>
    public class SetupDacService : SetupSqlConnection
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="SetupDacService"/>-class with default values.
        /// </summary>
        public SetupDacService() : base ()
        {
        }

        #region IDisposable

        private bool _disposed =false;

        /// <summary>
        /// <inheritdoc cref="IDisposable.Dispose"/>
        /// </summary>
        /// <param name="disposing">If <see langword="true"/> managed ressources will be disposed also.</param>
        protected override void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    // Bereinigung verwalteter Code
                    this._Dacpac?.Dispose();
                }

                // Manuelle Bereinigung nicht verwalteter Code ohne Dispose Prozedur

                this._disposed = true;
            }

            // Bereinigung Basisklasse ausführen
            base.Dispose(disposing);
        }

        ~SetupDacService()
        {
            Dispose(false);
        }
        #endregion

        #region DacFile

        protected DacPackage? _Dacpac;
        private FileInfo? _DacFile;

        /// <summary>
        /// Gets or sets the name of the dacpac-file to be used by this instance.
        /// If the dacpac-file is valid the content will be loaded.
        /// </summary>
        /// <value>Represents the dacpac-file to be used in this instance.</value>
        /// <exception cref="InvalidOperationException">Will be thrown, if an error occured during reading the dacpac-file.</exception>
        public FileInfo? DacFile
        {
            get => this._DacFile;
            set
            {
                // Datei laden, falls verfügbar
                if (value != null && value.Exists)
                {
                    try
                    {
                        // Eventuell schon geladene dacpac-Datei entladen
                        this._Dacpac = null;

                        // Neue Datei laden
                        this._Dacpac = DacPackage.Load(value.OpenRead());

                        this._DacFile = value;
                    }
                    catch (DacServicesException ex)
                    {
                        throw new InvalidOperationException($"Error occured during reading the dacpac-file {value.FullName}", ex);
                    }
                    catch (IOException ex)
                    {
                        throw new InvalidOperationException("The dacpac-file is already opened in another application.", ex);
                    }
                    catch
                    {
                        throw;
                    }
                }
                else
                {
                    throw new InvalidOperationException("Dac file does not exist.");
                }
            }
        }

        /// <summary>
        /// Gets the summary of the dacpac-file.
        /// </summary>
        /// <value>Returns the summary of the dacpac-file, if <see cref="DacFile"/> is set and the content is valid. Otherwise the summary is <see langword="null"/>.</value>
        public string? DacDescription
        {
            get
            {
                return this._Dacpac?.Description;
            }
        }

        /// <summary>
        /// Gets the name of the dacpac-project.
        /// </summary>
        /// <value>Returns the project name of the dacpac-file, if <see cref="DacFile"/> is set and the content is valid. Otherwise the name is <see langword="null"/>.</value>
        public string? DacName
        {
            get
            {
                return this._Dacpac?.Name;
            }
        }

        /// <summary>
        /// Gets the version number of the dacpac-file.
        /// </summary>
        /// <value>Returns the version of the dacpac-file, if <see cref="DacFile"/> is set and the content is valid. Otherwise the version is <see langword="null"/>.</value>
        public Version? DacVersion
        {
            get
            {
                return this._Dacpac?.Version;
            }
        }

        #endregion

        #region PublishDacPackage

        public void PublishDacPackage (string Databasename)
        {
            SqlConnectionStringBuilder builder;
            DacServices DacService;
            PublishOptions DacOptions = new()
            {
                DeployOptions = new()
                {
                    DropObjectsNotInSource = true,
                    DropPermissionsNotInSource = true,
                    RegisterDataTierApplication = true
                }
            };

            // Prüfen, ob ein Datenbankname angegeben wurde
            if (string.IsNullOrWhiteSpace(Databasename))
            {
                throw new ArgumentNullException(nameof(Databasename), "A database name is required for publishing the database.");
            }

            // Prüfen, ob eine dacpac-Datei angegeben wurde und diese geladen ist
            if (this._Dacpac == null)
            {
                throw new NullReferenceException($"No dacpac-file loaded. Use the property {nameof(this.DacFile)} in advance to set the dacpac-file.");
            }

            // SQL-Server Verbindung testen und Verbindungszeichen abrufen
            try
            {
                builder = base.GetConnectionString();
                builder.UserID = base.SQLServerLoginName;
            }
            catch
            {
                throw;
            }

            // DacService einrichten
            DacService = new(builder.ConnectionString, base.SQLServerLoginPassword);
            DacService.Message += DacService_Message;

            // Datenbank veröffentlichen
            try
            {
                DacService.Publish(this._Dacpac, Databasename, DacOptions);
                base.Database = Databasename;
            }
            catch (DacServicesException ex)
            {
                throw new InvalidOperationException("Error occured during publishing the dacpac-file.", ex);
            }
        }

        /// <summary>
        /// This event will be thrown every time when the dacpac-publishing process produces a message.
        /// </summary>
        public event DacPublishEventHandler? ProgressMessage;

        /// <summary>
        /// Handles the <see cref="DacServices.Message"/> event and invokes the <see cref="SetupDacService.ProgressMessage"/> event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="DacMessageEventArgs"/> instance with detailed information.</param>
        private void DacService_Message(object? sender, DacMessageEventArgs e)
        {
            // Übersetzung der Enum-Werte von MessageType
            var MsgType = e.Message.MessageType switch
            {
                DacMessageType.Message => DacPublishMessageTypeEnum.Message,
                DacMessageType.Error => DacPublishMessageTypeEnum.Error,
                DacMessageType.Warning => DacPublishMessageTypeEnum.Warning,
                _ => DacPublishMessageTypeEnum.Message,
            };

            ProgressMessage?.Invoke(this, new(e.Message.Message, MsgType));
        }

        #endregion
    }
}