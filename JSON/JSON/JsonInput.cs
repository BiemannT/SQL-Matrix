using BiemannT.MUT.MsSql.Def.Common;
using System.Data;
using System.Text.Json.Serialization;

namespace BiemannT.MUT.MsSql.Def.JSON
{
    /// <summary>
    /// This class provides properties and methods to represent a definition of a parameter requested by the test object in JSON-format.
    /// </summary>
    public class JsonInput : Input
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonInput"/>-class.
        /// </summary>
        public JsonInput() : base() { }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <remarks><inheritdoc/></remarks>
        [JsonPropertyName("ParameterName")]
        [JsonPropertyOrder(-10)]
        [JsonRequired]
        public override string ParameterName
        {
            get => base.ParameterName;
            set => base.ParameterName = value;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <remarks><inheritdoc/></remarks>
        [JsonPropertyName("SqlType")]
        [JsonPropertyOrder(-9)]
        [JsonRequired]
        [JsonConverter(typeof(JsonSqlTypeDefConverter))]
        public override SqlTypeDefinition SqlTypeDef
        {
            get => base.SqlTypeDef;
            set => base.SqlTypeDef = value;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <remarks><inheritdoc/></remarks>
        [JsonPropertyName("Nullable")]
        [JsonPropertyOrder(-8)]
        public override bool IsNullable
        {
            get => base.IsNullable;
            set => base.IsNullable = value;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <remarks><inheritdoc/></remarks>
        [JsonPropertyName("Default")]
        [JsonPropertyOrder(-7)]
        public override bool HasDefaultValue
        {
            get => base.HasDefaultValue;
            set => base.HasDefaultValue = value;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <remarks><inheritdoc/></remarks>
        [JsonPropertyName("Direction")]
        [JsonPropertyOrder(-6)]
        [JsonConverter(typeof(JsonInputDirectionConverter))]
        public override ParameterDirection Direction
        {
            get => base.Direction;
            set => base.Direction = value;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <remarks><inheritdoc/></remarks>
        [JsonPropertyName("UserValues")]
        [JsonPropertyOrder(-5)]
        [JsonConverter(typeof(JsonSqlValueListConverter))]
        [JsonInclude]
        public override List<object> UserValues
        {
            get => base.UserValues;
            protected set => base.UserValues = value;
        }
    }
}
