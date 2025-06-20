namespace Matrix.MsSql
{
    /// <summary>
    /// Specifies the type (or severity) of message associated with a given event.
    /// </summary>
    public enum DacPublishMessageTypeEnum
    {
        /// <summary>
        /// Informational message.
        /// </summary>
        Message = 0,

        /// <summary>
        /// Noncritical problem.
        /// </summary>
        Warning = 1,

        /// <summary>
        /// Serious, possibly fatal issue.
        /// </summary>
        Error = 2
    }
}