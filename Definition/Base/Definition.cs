namespace BiemannT.MUT.MsSql.Def.Base
{
    /// <summary>
    /// This class provides the base properties and methods for different definition formats.
    /// </summary>
    /// <remarks>All definition formats must inherit from this class.</remarks>
    public abstract class Definition
    {
        /// <summary>
        /// Gets or sets the API version for the current instance.
        /// </summary>
        public abstract Version ApiVersion { get; protected set; }

        /// <summary>
        /// Gets or sets the schema name of the tested object.
        /// </summary>
        public abstract string SchemaName { get; set; }

        /// <summary>
        /// Gets or sets the name of the tested object.
        /// </summary>
        public abstract string ObjectName { get; set; }

        /// <summary>
        /// Gets or sets the maximum execution time for each test case in seconds.
        /// </summary>
        public abstract int MaxExecutionTime { get; set; }

        /// <summary>
        /// Loads the definition from the underlying source.
        /// </summary>
        public abstract void Load();

        /// <summary>
        /// Saves the current definition to the underlying source.
        /// </summary>
        public abstract void Save();
    }
}
