using Microsoft.Data.SqlClient;

namespace Matrix.MsSql.Unit
{
    /// <summary>
    /// This class provides properties and methods to represent one testcase.
    /// </summary>
    public class TestCase
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="TestCase"/>-class with default values and without parameters.
        /// </summary>
        public TestCase()
        {
            this._State = TestCaseStateEnum.Initialized;
            this.Parameters = [];
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="TestCase"/>-class with default values and the given parameter set.
        /// </summary>
        /// <param name="parameters">An array of <see cref="TestCaseParameter"/>-instances which represents the used input parameters for this test case.</param>
        public TestCase(TestCaseParameter[] parameters)
        {
            this._State = TestCaseStateEnum.Initialized;
            this.Parameters = parameters;
        }

        // TODO: Testcase definieren
        // Ein Testcase besteht aus 0 oder mehreren Parametern.
        // Diese Paremeter können vordefinierte Werte enthalten, oder benutzerdefinierte Werte.
        // Ein Testcase kann 0 oder mehr Tabellen zur Ausführung erfordern.
        // Dabei ist auch möglich, dass für ein und die selbe Tabelle unterschiedliche Datensätze betrachtet werden sollen.
        // Für diesen einen Testcase gibt es erwartete Ergebnisse.
        // Das Ergebnis kann entweder nur ein Rückgabewert einer Prozedur, ein Skalarwert einer Funktion, eine bestimmte Ausnahme oder ein oder mehrere Tabellen als Resultset sein.

        // TODO: Testcase Aktionen
        // Methode bereitstellen, um auf einer vorgegebenen SqlConnection den Test auszuführen.
        // Eigenschaft erstellen, über den Zustand der Ausführung mit der Enumeration: Vorbereitet, Erwartetes Ergebnis fehlt, Eingabe Parameter fehlerhaft, Fehler bei der Erstellung der Bezugsdaten, Ausführung läuft, Erfolgreich-Ausgeführt, Fehlerhaft-Ausgeführt, Command-Timeout.
        // Ereignis erstellen mit Informationen über die aktuelle Ausführung.
        private TestCaseStateEnum _State;

        /// <summary>
        /// Gets the current state of this test case.
        /// </summary>
        public TestCaseStateEnum State
        {
            get => _State;
        }

        /// <summary>
        /// Gets or sets the maximum execution timeout, in seconds, for this test case.
        /// </summary>
        /// <remarks>Setting this property to a value less than or equal to zero disables the
        /// timeout.</remarks>
        public int ExecutionTimeout { get; set; } = 30;

        /// <summary>
        /// Gets the name of the SQL test object including schema name.
        /// </summary>
        public string TestObjectName { get; internal set; } = string.Empty;

        /// <summary>
        /// Gets the collection of parameters associated with this test case.
        /// </summary>
        public TestCaseParameter[] Parameters { get; private set; }

        public void ExecuteTest(SqlConnection connection)
        {
            // TODO: ExecuteTest Prozedur ausarbeiten
            throw new NotImplementedException();
        }

        // TODO: Dokumentationsfunktionen implementieren
        // Wünschenswert wäre eine Ausgabe im md- oder html-Format.
    }
}