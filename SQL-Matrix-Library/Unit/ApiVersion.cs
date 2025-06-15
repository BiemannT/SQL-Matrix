using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace Matrix.MsSql.Unit
{
    /// <summary>
    /// Represents the necessary API version of this test file.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter<ApiVersion>))]
    public enum ApiVersion
    {
        /// <summary>
        /// Not identified.
        /// </summary>
        None = 0,

        /// <summary>
        /// Version v1.0.0
        /// </summary>
        v1 = 1
    }
}