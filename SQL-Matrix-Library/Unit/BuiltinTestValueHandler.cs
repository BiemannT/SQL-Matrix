namespace Matrix.MsSql.Unit
{
    /// <summary>
    /// Encapsulates a method to retrieve built-in test values.
    /// </summary>
    /// <param name="size">The size required for the SQL-type.</param>
    /// <param name="precision">The precision required for the SQL-type.</param>
    /// <param name="scale">The scale required for the SQL-type.</param>
    /// <returns>Returns an array with test values.</returns>
    public delegate Array BuiltinTestValueHandler(int size, byte precision, byte scale);
}