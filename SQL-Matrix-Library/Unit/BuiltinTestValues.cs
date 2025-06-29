using System.Data;

namespace Matrix.MsSql.Unit
{
    /// <summary>
    /// This class provides several static methods to get built-in test values for different <see cref="SqlDbType"/> types.
    /// </summary>
    /// <remarks>
    /// All the methods are compatible to use with the delegate <see cref="BuiltinTestValueHandler"/>.
    /// </remarks>
    /// <seealso href="https://learn.microsoft.com/en-us/sql/connect/ado-net/sql-server-data-type-mappings"/>
    public static class BuiltinTestValues
    {

        /// <summary>
        /// Represents built-in test values for the type <see cref="SqlDbType.Binary"/>.
        /// </summary>
        /// <param name="size">The size of the type from 1 through 8000. Set 0 for the default value 1.</param>
        /// <param name="precision">Not required for this type.</param>
        /// <param name="scale">Not required for this type.</param>
        /// <returns>Returns an array with test values.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Will be thrown if <paramref name="size"/> is not between 1 and 8000 or 0.</exception>
        public static Array BuiltinBinary (int size, byte precision = 0, byte scale = 0)
        {
            _ = precision;
            _ = scale;
            byte[] retValues;

            if (size == 0)
            {
                // Default Value = 1
                retValues = [];
                // TODO: Werte definieren
            }
            else if (size < 1 || size > 8000)
            {
                throw new ArgumentOutOfRangeException(nameof(size), "The size must be a value from 1 through 8000.");
            }
            else
            {
                retValues = [];
                // TODO: Werte definieren
            }

            return retValues;
        }

        /// <summary>
        /// Represents built-in test values for the type <see cref="SqlDbType.VarBinary"/>.
        /// </summary>
        /// <param name="size">The size of the type from 1 through 8000. Set -1 to test VARBINARY(MAX). Set 0 for the default value 1.</param>
        /// <param name="precision">Not required for this type.</param>
        /// <param name="scale">Not required for this type.</param>
        /// <returns>Returns an array with test values.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Will be thrown if <paramref name="size"/> is not between 1 and 8000 or -1 or 0.</exception>
        public static Array BuiltinVarbinary (int size, byte precision = 0, byte scale = 0)
        {
            _ = precision;
            _ = scale;
            byte[] retValues;

            if (size == 0)
            {
                // Default Value = 1
                retValues= [];
                // TODO: Werte definieren
            }
            else if (size == -1)
            {
                // VARBINARY(MAX)
                retValues = [];
                // TODO: Werte definieren
            }
            else if (size < 1 || size > 8000)
            {
                throw new ArgumentOutOfRangeException(nameof(size), "The size must be a value from 1 through 8000.");
            }
            else
            {
                retValues = [];
                // TODO: Werte definieren
            }

            return retValues;
        }
        /// <summary>
        /// Represents built-in test values for the type <see cref="SqlDbType.VarChar"/>.
        /// </summary>
        /// <param name="size">The size of the type from 1 through 8000. Set -1 to test VARCHAR(MAX). Set 0 for the default value 1.</param>
        /// <param name="precision">Not required for this type.</param>
        /// <param name="scale">Not required for this type.</param>
        /// <returns>Returns an array with test values.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Will be thrown if <paramref name="size"/> is not between 1 and 8000 or -1 or 0.</exception>
        public static Array BuiltinVarchar (int size, byte precision = 0, byte scale = 0)
        {
            _ = precision;
            _ = scale;
            string[] retValues;

            if (size == 0)
            {
                // Default Value = 1
                retValues = [" ", "a"];
            }
            else if (size == -1)
            {
                // VARCHAR(MAX)
                retValues = [];
                // TODO: Werte definieren
            }
            else if (size < 1 || size > 8000)
            {
                throw new ArgumentOutOfRangeException(nameof(size), "The size must be a value from 1 through 8000.");
            }
            else
            {
                retValues = [];
                // TODO: Werte definieren
            }

            return retValues;
        }
    }
}