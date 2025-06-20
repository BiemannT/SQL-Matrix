namespace Matrix.MsSql
{
    /// <summary>
    /// Represents the method that handles the <see cref="SetupDacService.ProgressMessage"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An <see cref="DacPublishEventArgs"/> instance with detailed information containing the event data.</param>
    public delegate void DacPublishEventHandler(object sender, DacPublishEventArgs e);
}