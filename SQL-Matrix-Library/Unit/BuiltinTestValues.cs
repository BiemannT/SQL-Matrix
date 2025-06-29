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
        /// Represents built-in test values for the type <see cref="SqlDbType.Char"/>.
        /// </summary>
        /// <param name="size">The size of the type from 1 through 8000. Set 0 for the default value 1.</param>
        /// <param name="precision">Not required for this type.</param>
        /// <param name="scale">Not required for this type.</param>
        /// <returns>Returns an array with test values.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Will be thrown if <paramref name="size"/> is not between 1 and 8000 or 0.</exception>
        public static Array BuiltinChar (int size, byte precision = 0, byte scale = 0)
        {
            _ = precision;
            _ = scale;
            string[] retValues;

            if (size == 0)
            {
                // Default Value = 1
                retValues = [" ", "a"];
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

        /// <summary>
        /// Represents built-in test values for the type <see cref="SqlDbType.NChar"/>.
        /// </summary>
        /// <param name="size">The size of the type from 1 through 4000. Set 0 for the default value 1.</param>
        /// <param name="precision">Not required for this type.</param>
        /// <param name="scale">Not required for this type.</param>
        /// <returns>Returns an array with test values.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Will be thrown if <paramref name="size"/> is not between 1 and 4000 or 0.</exception>
        public static Array BuiltinNChar (int size, byte precision = 0, byte scale = 0)
        {
            _ = precision;
            _ = scale;
            string[] retValues;

            if (size == 0)
            {
                // Default Value = 1
                retValues = [" ", "a"];
                // TODO: Werte definieren
            }
            else if (size < 1 || size > 4000)
            {
                throw new ArgumentOutOfRangeException(nameof(size), "The size must be a value from 1 through 4000.");
            }
            else
            {
                 retValues = [];
                // TODO: Werte definieren
            }

            return retValues;
        }

        /// <summary>
        /// Represents built-in test values for the type <see cref="SqlDbType.NVarChar"/>.
        /// </summary>
        /// <param name="size">The size of the type from 1 through 4000. Set -1 to test NVARCHAR(MAX). Set 0 for the default value 1.</param>
        /// <param name="precision">Not required for this type.</param>
        /// <param name="scale">Not required for this type.</param>
        /// <returns>Returns an array with test values.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Will be thrown if <paramref name="size"/> is not between 1 and 4000 or -1 or 0</exception>
        public static Array BuiltinNVarchar (int size, byte precision = 0, byte scale = 0)
        {
            _ = precision;
            _ = scale;
            string[] retValues;

            if (size == 0)
            {
                // Default Value = 1
                retValues = [" ", "a"];
                // TODO: Werte definieren
            }
            else if (size == -1)
            {
                // NVARCHAR(MAX)
                retValues = [];
                // TODO: Werte definieren
            }
            else if (size < 1 || size > 4000)
            {
                throw new ArgumentOutOfRangeException(nameof(size), "The size must be a value from 1 through 4000.");
            }
            else
            {
                retValues = [];
                // TODO: Werte definieren
            }

            return retValues;
        }

        /// <summary>
        /// Represents built-in test values for the type <see cref="SqlDbType.UniqueIdentifier"/>.
        /// </summary>
        /// <param name="size">Not required for this type.</param>
        /// <param name="precision">Not required for this type.</param>
        /// <param name="scale">Not required for this type.</param>
        /// <returns>Returns an array with test values.</returns>
        public static Array BuiltinUniqueidentifier (int size = 0, byte precision = 0, byte scale = 0)
        {
            _ = size;
            _ = precision;
            _ = scale;
            Guid[] retValues;

            retValues = [];
            // TODO: Werte definieren

            return retValues;
        }

        /// <summary>
        /// Represents built-in test values for the type <see cref="SqlDbType.Bit"/>.
        /// </summary>
        /// <param name="size">Not required for this type.</param>
        /// <param name="precision">Not required for this type.</param>
        /// <param name="scale">Not required for this type.</param>
        /// <returns>Returns an array with test values.</returns>
        public static Array BuiltinBit (int size = 0, byte precision = 0, byte scale = 0)
        {
            _ = size;
            _ = precision;
            _ = scale;
            Boolean[] retValues;

            retValues = [false, true];

            return retValues;
        }

        /// <summary>
        /// Represents built-in test values for the type <see cref="SqlDbType.TinyInt"/>.
        /// </summary>
        /// <param name="size">Not required for this type.</param>
        /// <param name="precision">Not required for this type.</param>
        /// <param name="scale">Not required for this type.</param>
        /// <returns>Returns an array with test values.</returns>
        public static Array BuiltinTinyInt (int size = 0, byte precision = 0, byte scale = 0)
        {
            _ = size;
            _ = precision;
            _ = scale;
            Byte[] retValues;

            retValues = [0, 255];

            return retValues;
        }

        /// <summary>
        /// Represents built-in test values for the type <see cref="SqlDbType.SmallInt"/>.
        /// </summary>
        /// <param name="size">Not required for this type.</param>
        /// <param name="precision">Not required for this type.</param>
        /// <param name="scale">Not required for this type.</param>
        /// <returns>Returns an array with test values.</returns>
        public static Array BuiltinSmallInt (int size = 0, byte precision = 0, byte scale = 0)
        {
            _ = size;
            _ = precision;
            _ = scale;
            Int16[] retValues;

            retValues = [-32768, 0, 32767];

            return retValues;
        }

        /// <summary>
        /// Represents built-in test values for the type <see cref="SqlDbType.Int"/>.
        /// </summary>
        /// <param name="size">Not required for this type.</param>
        /// <param name="precision">Not required for this type.</param>
        /// <param name="scale">Not required for this type.</param>
        /// <returns>Returns an array with test values.</returns>
        public static Array BuiltinInt (int size = 0, byte precision = 0, byte scale = 0)
        {
            _ = size;
            _ = precision;
            _ = scale;
            Int32[] retValues;

            retValues = [-2147483648, 0, 2147483647];

            return retValues;
        }

        /// <summary>
        /// Represents built-in test values for the type <see cref="SqlDbType.BigInt"/>.
        /// </summary>
        /// <param name="size">Not required for this type.</param>
        /// <param name="precision">Not required for this type.</param>
        /// <param name="scale">Not required for this type.</param>
        /// <returns>Returns an array with test values.</returns>
        public static Array BuiltinBigInt (int size = 0, byte precision = 0, byte scale = 0)
        {
            _ = size;
            _ = precision;
            _ = scale;
            Int64[] retValues;

            retValues = [-9223372036854775808, 0, 9223372036854775807];

            return retValues;
        }

        /// <summary>
        /// Represents built-in test values for the type <see cref="SqlDbType.SmallMoney"/>.
        /// </summary>
        /// <param name="size">Not required for this type.</param>
        /// <param name="precision">Not required for this type.</param>
        /// <param name="scale">Not required for this type.</param>
        /// <returns>Returns an array with test values.</returns>
        public static Array BuiltinSmallmoney (int size = 0, byte precision = 0, byte scale = 0)
        {
            _ = size;
            _ = precision;
            _ = scale;
            decimal[] retValues;

            retValues = [-214748.3648m, 0, 0.0001m, 214748.3647m];

            return retValues;
        }

        /// <summary>
        /// Represents built-in test values for the type <see cref="SqlDbType.Money"/>.
        /// </summary>
        /// <param name="size">Not required for this type.</param>
        /// <param name="precision">Not required for this type.</param>
        /// <param name="scale">Not required for this type.</param>
        /// <returns>Returns an array with test values.</returns>
        public static Array BuiltinMoney (int size = 0, byte precision = 0, byte scale = 0)
        {
            _ = size;
            _ = precision;
            _ = scale;
            decimal[] retValues;

            retValues = [-922337203685477.5808m, 0, 0.0001m, 922337203685477.5807m];

            return retValues;
        }

        /// <summary>
        /// Represents built-in test values for the type <see cref="SqlDbType.Decimal"/>.
        /// </summary>
        /// <param name="size">Not required for this type.</param>
        /// <param name="precision">The precision of the type from 1 through 38. Set 0 for the default value 18.</param>
        /// <param name="scale">The scale for the type from 0 until the value of <paramref name="precision"/>. Set 0 for the default value 0.</param>
        /// <returns>Returns an array with test values.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Will be thrown if <paramref name="precision"/> is not between 1 and 38 - or - <paramref name="scale"/> is not between 0 and <paramref name="precision"/>.</exception>
        public static Array BuiltinDecimal (int size = 0, byte precision = 18, byte scale = 0)
        {
            _ = size;
            decimal[] retValues;

            if (precision == 0)
            {
                // Default Value = 18

                if (scale == 0)
                {
                    // Default Value = 0
                    retValues = [0];
                    // TODO: Werte definieren
                }
                else if (scale > 18)
                {
                    throw new ArgumentOutOfRangeException(nameof(scale), "The scale must be a value between 0 and the value of precision.");
                }
                else
                {
                    retValues = [0];
                    // TODO: Werte definieren
                }
            }
            else if (precision < 1 || precision > 38)
            {
                throw new ArgumentOutOfRangeException(nameof(precision), "The precision must be a value from 1 through 38.");
            }
            else if (scale < 0 || scale > precision)
            {
                throw new ArgumentOutOfRangeException(nameof(scale), "The scale must be a value between 0 and the value of precision.");
            }
            else
            {
                retValues = [0];
                // TODO: Werte definieren
            }

            return retValues;
        }

        /// <summary>
        /// Represents built-in test values for the type <see cref="SqlDbType.Real"/>.
        /// </summary>
        /// <param name="size">Not required for this type.</param>
        /// <param name="precision">Not required for this type.</param>
        /// <param name="scale">Not required for this type.</param>
        /// <returns>Returns an array with test values.</returns>
        public static Array BuiltinReal (int size = 0, byte precision = 0, byte scale = 0)
        {
            _ = size;
            _ = precision;
            _ = scale;
            Single[] retValues;

            retValues = [0];
            // TODO: Werte definieren

            return retValues;
        }

        /// <summary>
        /// Represents built-in test values for the type <see cref="SqlDbType.Float"/>.
        /// </summary>
        /// <param name="size">The size of the type from 1 through 53. Set 0 for the default value 53.</param>
        /// <param name="precision">Not required for this type.</param>
        /// <param name="scale">Not required for this type.</param>
        /// <returns>Returns an array with test values.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Will be thrown if <paramref name="size"/> is not between 1 and 53 or 0.</exception>
        public static Array BuiltinFloat (int size = 53, byte precision = 0, byte scale = 0)
        {
            _ = precision;
            _ = scale;
            double[] retValues;

            if (size == 0)
            {
                // Default Value = 53
                retValues = [0];
                // TODO: Werte definieren
            }
            else if (size < 1 || size > 53)
            {
                throw new ArgumentOutOfRangeException(nameof(size), "The size must be a value from 1 through 53.");
            }
            else
            {
                retValues = [0];
                // TODO: Werte definieren
            }

            return retValues;
        }
    }
}