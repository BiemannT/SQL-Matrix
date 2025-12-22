using System.Data;

namespace BiemannT.MUT.MsSql.Def.Common
{
    /// <summary>
    /// This class provides properties and methods to represent a definition of a parameter requested by the test object.
    /// </summary>
    public class Input : IEquatable<Input>
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

        #region IEquatable<Input> Members

        /// <summary>
        /// Determines whether the current <see cref="Input"/> instance is equal to another <see cref="Input"/> instance based on the parameter
        /// name, using a case-insensitive comparison.
        /// </summary>
        /// <param name="other">The <see cref="Input"/> instance to compare with the current instance, or <see langword="null"/> to compare against no object.</param>
        /// <returns><see langword="true"/> if the parameter names are equal, ignoring case; otherwise, <see langword="false"/>.</returns>
        public bool Equals(Input? other)
        {
            if (other is null) return false;

            return this.ParameterName.Equals(other.ParameterName, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current <see cref="Input"/> instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current <see cref="Input"/> instance.</param>
        /// <returns><see langword="true"/> if the specified object is an instance of <see cref="Input"/> and is equal to the current instance; otherwise, <see langword="false"/>.</returns>
        public override bool Equals(object? obj)
        {
            if (obj is Input other)
            {
                return Equals(other);
            }
            return false;
        }

        /// <summary>
        /// Serves as the default hash function for the object.
        /// </summary>
        /// <remarks>The hash code is based on the lowercase invariant form of the parameter name,
        /// ensuring that parameter names differing only by case produce the same hash code.</remarks>
        /// <returns>A hash code for the current object, computed using a case-insensitive representation of the parameter name.</returns>
        public override int GetHashCode()
        {
            return ParameterName.ToLowerInvariant().GetHashCode();
        }

        #endregion
    }
}
