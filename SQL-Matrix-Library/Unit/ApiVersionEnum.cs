using System.Text.Json.Serialization;

namespace Matrix.MsSql.Unit
{
    /// <summary>
    /// Represents the necessary API version of this test file.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter<ApiVersionEnum>))]
    public enum ApiVersionEnum
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