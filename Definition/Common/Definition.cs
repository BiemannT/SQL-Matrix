namespace BiemannT.MUT.MsSql.Def.Common
{
    /// <summary>
    /// This class provides the base properties and methods for different definition formats.
    /// </summary>
    /// <remarks>All definition formats must inherit from this class.</remarks>
    public class Definition
    {
        /// <summary>
        /// Initializes a new instance of the Definition class with default values.
        /// </summary>
        public Definition()
        {
            ApiVersion = new(1, 0);
            SchemaName = "dbo";
            ObjectName = string.Empty;
            MaxExecutionTime = 10;
            Inputs = [];
            ExpectedResults = [];
        }

        /// <summary>
        /// Gets or sets the API version for the current instance.
        /// </summary>
        /// <remarks>The default value is 1.0.</remarks>
        public virtual Version ApiVersion { get; protected set; }

        /// <summary>
        /// Gets or sets the schema name of the tested object.
        /// </summary>
        /// <remarks>The default value is "dbo".</remarks>
        public virtual string SchemaName { get; set; }

        /// <summary>
        /// Gets or sets the name of the tested object.
        /// </summary>
        /// <remarks>The default value is <see cref="string.Empty"/>.</remarks>
        public virtual string ObjectName { get; set; }

        /// <summary>
        /// Gets or sets the maximum execution time for each test case in seconds.
        /// </summary>
        /// <remarks>The default value is 10 seconds. The value 0 means infinite execution time.</remarks>
        public virtual int MaxExecutionTime { get; set; }

        /// <summary>
        /// Gets the collection of input parameters for the test object.
        /// </summary>
        /// <remarks>The default is an empty list.</remarks>
        public virtual List<Input> Inputs { get; }

        /// <summary>
        /// Gets the collection of expected results for the test object.
        /// </summary>
        /// <remarks>The default is an empty list.</remarks>
        public virtual List<ExpectedResult> ExpectedResults { get; }

        /// <summary>
        /// Loads the definition from the underlying source.
        /// </summary>
        /// <exception cref="NotImplementedException">The load method is not implemented in the base definition class. Use a derived class.</exception>
        public virtual void Load()
        {
            throw new NotImplementedException("Load method is not implemented in the base Definition class.");
        }

        /// <summary>
        /// Saves the current definition to the underlying source.
        /// </summary>
        /// <exception cref="NotImplementedException">The save method is not implemented in the base definition class. Use a derived class.</exception>"
        public virtual void Save()
        {
            throw new NotImplementedException("Save method is not implemented in the base Definition class.");
        }
    }
}
