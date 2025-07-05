using Microsoft.Data.SqlClient;
using System.Collections;

namespace Matrix.MsSql.Unit
{
    /// <summary>
    /// This class provides properties and methods to represent one input parameter for a test case.
    /// </summary>
    public class TestCaseParameter
    {
        /// <summary>
        /// Initialize an instance of <see cref="TestCaseParameter"/>.
        /// </summary>
        /// <param name="parameterBase">General base information used by this parameter instance.</param>
        /// <remarks>It is not foreseen to create this instance by public calls.</remarks>
        protected TestCaseParameter (TestCaseParameterBase parameterBase)
        {
            this.BaseInfo = parameterBase;
        }

        /// <summary>
        /// Gets the base information for this specific test case parameter.
        /// </summary>
        /// <value>An instance of type <see cref="TestCaseParameterBase"/>.</value>
        public TestCaseParameterBase BaseInfo { get; private set; }

        /// <summary>
        /// Gets the value associated with the parameter.
        /// </summary>
        /// <remarks>
        /// The value is <see langword="null"/>, if the properties <see cref="IsDefaultValue"/> is <see langword="true"/>.
        /// If <see cref="IsNull"/> is <see langword="true"/>, then the value is <see cref="DBNull.Value"/>.
        /// </remarks>
        /// <value>
        /// The value of this parameter as <see langword="object"/>-type.
        /// The type of the value is defined in the property <see cref="Type"/>.
        /// </value>
        public object? Value { get; private set; }

        /// <summary>
        /// Determines whether the <see cref="Value"/> is generated or user specific.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if the <see cref="Value"/> is generated based on the parameter definition.
        /// Otherwise <see langword="false"/> if the <see cref="Value"/> is user specific.
        /// </value>
        public bool IsBuiltinValue { get; private set; }

        /// <summary>
        /// Determines whether the <see cref="Value"/> represents SQL DEFAULT input.
        /// </summary>
        /// <value><see langword="true"/> if this parameter represents a DEFAULT value, otherwise <see langword="false"/>.</value>
        /// <remarks>If <see langword="true"/> the property <see cref="Value"/> is <see langword="null"/>.</remarks>
        public bool IsDefaultValue { get; private set; }

        /// <summary>
        /// Determines whether the <see cref="Value"/> represents SQL NULL input.
        /// </summary>
        /// <value><see langword="true"/> if this parameter represents a NULL value, otherwise <see langword="false"/>.</value>
        /// <remarks>If <see langword="true"/> the property <see cref="Value"/> is <see langword="null"/>.</remarks>
        public bool IsNull { get; private set; }

        /// <summary>
        /// Prepare the <see cref="SqlParameter"/> to perform the test case.
        /// </summary>
        /// <returns>Returns an instance of <see cref="SqlParameter"/> based on the properties from this instance.</returns>
        public SqlParameter GetSqlParameter()
        {
            // Parameter initialisieren
            SqlParameter parameter = new()
            {
                SqlDbType = this.BaseInfo.Type,
                ParameterName = this.BaseInfo.ParameterName,
                Direction = System.Data.ParameterDirection.Input,
                IsNullable = this.BaseInfo.IsNullable,
                Size = this.BaseInfo.Size,
                Precision = this.BaseInfo.Precision,
                Scale = this.BaseInfo.Scale,
                SqlValue = this.Value
            };

            return parameter;
        }

        /// <summary>
        /// Generates a set of <see cref="TestCaseParameter"/> for one test case input parameter as defined in the <see cref="TestInput"/>-instance.
        /// </summary>
        /// <param name="input">An instance of <see cref="TestInput"/> as template for the generation of test parameters.</param>
        /// <returns>Returns an array of <see cref="TestCaseParameter"/> with built-in test values and user specific test values.</returns>
        /// <exception cref="InvalidOperationException">The content of the <paramref name="input"/> is not valid.</exception>
        public static TestCaseParameter[] GenerateParameters(TestInput input)
        {
            TestCaseParameterBase parameterBase;
            Array? builtinValues;
            ArrayList parameters = [];

            try
            {
                // Input Definition analysieren und Parameter Basis-Klasse erzeugen
                parameterBase = new(input);

                // Built-in Testwerte einlesen
                builtinValues = parameterBase.BuiltinTestValue?.Invoke(parameterBase.Size, parameterBase.Precision, parameterBase.Scale);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("The Test Definition Input Parameter is not valid.", ex);
            }

            // Parameter mit Standard-Werten erstellen
            
            // NULL-Wert
            if (input.Nullable)
            {
                TestCaseParameter test = new(parameterBase)
                {
                    IsBuiltinValue = true,
                    IsNull = true,
                    IsDefaultValue = false,
                    Value = DBNull.Value
                };

                parameters.Add(test);
            }

            // DEFAULT-Wert
            if (input.DefaultValue)
            {
                TestCaseParameter test = new(parameterBase)
                {
                    IsBuiltinValue = true,
                    IsNull = false,
                    IsDefaultValue = true,
                    Value = null
                };

                parameters.Add(test);
            }

            // Built-in test Werte
            if (builtinValues != null)
            {
                foreach (object wert in builtinValues)
                {
                    TestCaseParameter test = new(parameterBase)
                    {
                        IsBuiltinValue = true,
                        IsNull = false,
                        IsDefaultValue = false,
                        Value = wert
                    };

                    parameters.Add(test);
                }
            }

            // Benutzerdefinierte Werte
            // TODO: Benutzerdefinierte Werte verarbeiten und der Parameterliste ergänzen

            return (TestCaseParameter[])parameters.ToArray(typeof(TestCaseParameter));
        }
    }
}