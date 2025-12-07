using System.Data;

namespace BiemannT.MUT.MsSql.Def.Common
{
    /// <summary>
    /// This class provides properties and methods to represent a definition of a parameter requested by the test object.
    /// </summary>
    public class Input
    {
        /// <summary>
        /// Initializes a new instance of the Input class with default values for all properties.
        /// </summary>
        public Input()
        {
            ParameterName = string.Empty;
            SqlTypeDef = new SqlTypeDefinition();
            IsNullable = true;
            HasDefaultValue = false;
            Direction = ParameterDirection.Input;
            UserValues = [];
        }

        /// <summary>
        /// Gets or sets the name of the parameter.
        /// </summary>
        /// <remarks>Default is <see cref="string.Empty"/>.</remarks>
        public virtual string ParameterName { get; set; }

        /// <summary>
        /// Gets or sets the SQL-Server parameter type definition.
        /// </summary>
        /// <remarks>Default is the type <see cref="SupportedSqlType.NotSupported"/>.</remarks>
        public virtual SqlTypeDefinition SqlTypeDef { get; set; }

        /// <summary>
        /// Gets or sets if a NULL-value is allowed for this parameter.
        /// </summary>
        /// <remarks>Default is <see langword="true"/>.</remarks>
        public virtual bool IsNullable { get; set; }

        /// <summary>
        /// Gets or sets if the parameter has a default value defined in the database.
        /// </summary>
        /// <remarks>Default is <see langword="false"/>.</remarks>
        public virtual bool HasDefaultValue { get; set; }

        /// <summary>
        /// Gets or sets the direction of the parameter.
        /// </summary>
        /// <remarks>
        /// Only <see cref="ParameterDirection.Input"/> and <see cref="ParameterDirection.Output"/> are supported.
        /// Default is <see cref="ParameterDirection.Input"/>.
        /// </remarks>
        public virtual ParameterDirection Direction { get; set; }

        /// <summary>
        /// Gets a list of user defined values to be tested additionally
        /// to the pre-defined test values according to the <see cref="SqlTypeDef"/>.
        /// </summary>
        /// <remarks>
        /// The type of each user value should be a type compatible to the <see cref="SqlTypeDef"/>.
        /// User values will be ignored, if the <see cref="Direction"/> is not <see cref="ParameterDirection.Input"/>.
        /// </remarks>
        public virtual List<object> UserValues { get; protected set; }
    }
}
