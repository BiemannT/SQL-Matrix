using System.Data;

namespace BiemannT.MUT.MsSql.Def.Common
{
    /// <summary>
    /// This class provides the base properties and methods to represent a definition of a parameter requested by the test object.
    /// </summary>
    /// <remarks>All definition formats must inherit from this class.</remarks>
    public abstract class Input
    {
        /// <summary>
        /// Gets or sets the name of the parameter.
        /// </summary>
        public abstract string ParameterName { get; set; }

        /// <summary>
        /// Gets or sets the SQL-Server parameter type definition.
        /// </summary>
        public abstract SqlTypeDefinition SqlTypeDef { get; set; }

        /// <summary>
        /// Gets or sets if a NULL-value is allowed for this parameter.
        /// </summary>
        public abstract bool IsNullable { get; set; }

        /// <summary>
        /// Gets or sets if the parameter has a default value defined in the database.
        /// </summary>
        public abstract bool HasDefaultValue { get; set; }

        /// <summary>
        /// Gets or sets the direction of the parameter.
        /// </summary>
        public abstract ParameterDirection Direction { get; set; }

        /// <summary>
        /// Gets a list of user defined values to be tested additionally.
        /// </summary>
        /// <remarks>The type of each user value should be a type compatible to the <see cref="SqlType"/>.
        /// The content of the list should be resetted, if <see cref="SqlTypeDefinition"/> will be changed.</remarks>
        public abstract List<object> UserValues { get; }
    }
}
