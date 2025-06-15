using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace Matrix.MsSql.Unit
{
    /// <summary>
    /// Represents the SQL object type to be tested.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter<TestObjectTypeEnum>))]
    public enum TestObjectTypeEnum
    {
        /// <summary>
        /// Not identified.
        /// </summary>
        /// <remarks>Should be used only as fallback value, not for new instances.</remarks>
        None = 0,

        /// <summary>
        /// SQL scalar function.
        /// </summary>
        ScalarFunction = 1,

        /// <summary>
        /// SQL table valued function.
        /// </summary>
        TableValuedFunction = 2,

        /// <summary>
        /// SQL stored procedure.
        /// </summary>
        StoredProcedure = 3
    }
}