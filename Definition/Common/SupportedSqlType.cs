namespace BiemannT.MUT.MsSql.Def.Common
{
    /// <summary>
    /// This enumeration represents the supported SQL-Server data types.
    /// </summary>
    public enum SupportedSqlType
    {
        /// <summary>
        /// Indicates that the SQL type is not supported.
        /// </summary>
        NotSupported = -1,

        /// <summary>
        /// A 64-bit signed integer.
        /// </summary>
        /// <remarks>Uses the .NET-type <see cref="System.Int64"/>.</remarks>
        BigInt = 0,

        /// <summary>
        /// A fixed-length binary data ranginging from 1 to 8,000 bytes.
        /// </summary>
        /// <remarks>Uses the .NET-type <see cref="System.Byte[]"/>.</remarks>
        Binary = 1,

        /// <summary>
        /// An unsigned numeric value that can be 0, 1.
        /// </summary>
        /// <remarks>Uses the .NET-type <see cref="System.Boolean"/>.</remarks>
        Bit = 2,

        /// <summary>
        /// A fixed-length stream of non-Unicode characters ranging from 1 to 8,000 characters.
        /// </summary>
        /// <remarks>Uses the .NET-type <see cref="System.String"/>.</remarks>
        Char = 3,

        /// <summary>
        /// Date data ranging from January 1, 0001 to December 31, 9999 AD.
        /// </summary>
        /// <remarks>Uses the .NET-type <see cref="System.DateTime"/>.</remarks>
        Date = 31,

        /// <summary>
        /// Date and time data ranging in value from January 1, 1753 to December 31, 9999 to an accuracy of 3.33 milliseconds.
        /// </summary>
        /// <remarks>Uses the .NET-type <see cref="System.DateTime"/>.</remarks>
        DateTime = 4,

        /// <summary>
        /// Date and time data. Date value range is from January 1,1 AD through December 31, 9999 AD. Time value range is 00:00:00 through 23:59:59.999999 with an accuracy of 100 nanoseconds.
        /// </summary>
        DateTime2 = 33,

        /// <summary>
        /// Date and time data with time zone awareness. Date value range is from January 1,1 AD through December 31, 9999 AD. Time value range is 00:00:00 through 23:59:59.9999999 with an accuracy of 100 nanoseconds.
        /// Time zone offset range is -14:00 through +14:00.
        /// </summary>
        /// <remarks>Uses the .NET-type <see cref="System.DateTimeOffset"/>.</remarks>
        DateTimeOffset = 34,

        /// <summary>
        /// A fixed precision and scale numeric value between -10 38 -1 and 10 38 -1.
        /// </summary>
        /// <remarks>Uses the .NET-type <see cref="System.Decimal"/>.</remarks>
        Decimal = 5,

        /// <summary>
        /// A floating point number within the range of -1.79E +308 through 1.79E +308.
        /// </summary>
        /// <remarks>Uses the .NET-type <see cref="System.Double"/>.</remarks>
        Float = 6,

        /// <summary>
        /// A 32-bit signed integer.
        /// </summary>
        /// <remarks>Uses the .NET-type <see cref="System.Int32"/>.</remarks>
        Int = 8,

        /// <summary>
        /// A currency value ranging from -2 63 (or -9,223,372,036,854,775,808) to 2 63 -1 (or +9,223,372,036,854,775,807) with an accuracy to a ten-thousandth of a currency unit.
        /// </summary>
        /// <remarks>Uses the .NET-type <see cref="System.Decimal"/>.</remarks>
        Money = 9,

        /// <summary>
        /// A fixed-length stream of Unicode characters ranging from 1 to 4,000 characters.
        /// </summary>
        /// <remarks>Uses the .NET-type <see cref="System.String"/>.</remarks>
        NChar = 10,

        /// <summary>
        /// A variable-length stream of Unicode characters ranging between 1 and 4,000 characters.
        /// </summary>
        /// <remarks>Uses the .NET-type <see cref="System.String"/>.</remarks>
        NVarChar = 12,

        /// <summary>
        /// A floating point number within the range of -3.40E +38 through 3.40E +38.
        /// </summary>
        /// <remarks>Uses the .NET-type <see cref="System.Single"/>.</remarks>
        Real = 13,

        /// <summary>
        /// Date and time data ranging in value from January 1, 1900 to June 6, 2079 to an accuracy of one minute.
        /// </summary>
        /// <remarks>Uses the .NET-type <see cref="System.DateTime"/>.</remarks>
        SmallDateTime = 15,

        /// <summary>
        /// A 16-bit signed integer.
        /// </summary>
        /// <remarks>Uses the .NET-type <see cref="System.Int16"/>.</remarks>
        SmallInt = 16,

        /// <summary>
        /// A currency value ranging from -214,748.3648 to +214,748.3647 with an accuracy to a ten-thousandth of a currency unit.
        /// </summary>
        /// <remarks>Uses the .NET-type <see cref="System.Decimal"/>.</remarks>
        SmallMoney = 17,

        /// <summary>
        /// Time data based on a 24-hour clock. Time value range is 00:00:00 through 23:59:59.9999999 with an accuracy of 100 nanoseconds.
        /// </summary>
        /// <remarks>Uses the .NET-type <see cref="System.TimeSpan"/>.</remarks>
        Time = 32,

        /// <summary>
        /// An 8-bit unsigned integer.
        /// </summary>
        /// <remarks>Uses the .NET-type <see cref="System.Byte"/>.</remarks>
        TinyInt = 20,

        /// <summary>
        /// A globally unique identifier (or GUID).
        /// </summary>
        /// <remarks>Uses the .NET-type <see cref="System.Guid"/>.</remarks>
        UniqueIdentifier = 14,

        /// <summary>
        /// A variable-length stream of binary data ranging between 1 and 8,000 bytes.
        /// </summary>
        /// <remarks>Uses the .NET-type <see cref="System.Byte"/>.</remarks>
        VarBinary = 21,

        /// <summary>
        /// A variable-length stream of non-Unicode characters ranging between 1 and 8,000 characters.
        /// </summary>
        /// <remarks>Uses the .NET-type <see cref="System.String"/>.</remarks>
        VarChar = 22,
    }
}
