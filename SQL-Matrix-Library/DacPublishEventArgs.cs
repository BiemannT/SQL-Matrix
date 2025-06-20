namespace Matrix.MsSql
{
    /// <summary>
    /// This instance covers all the information which occur during publishing the database to the SQL-Server.
    /// </summary>
    /// <param name="message">The message text of the current Publish Progress.</param>
    /// <param name="messageType">The type of the current message.</param>
    public class DacPublishEventArgs(string message, DacPublishMessageTypeEnum messageType) : EventArgs
    {

        /// <summary>
        /// Gets the message from the SQL DAC-Service.
        /// </summary>
        public string Message { get; private set; } = message;

        /// <summary>
        /// Gets the type of the current message.
        /// </summary>
        public DacPublishMessageTypeEnum MessageType { get; private set; } = messageType;

        /// <summary>
        /// Returns all the information of this instance in one line.
        /// </summary>
        /// <returns>Returns all the information of this instance in one line.</returns>
        public override string ToString()
        {
            return string.Concat(MessageType.ToString()[..2].ToUpper(), ": ", Message);
        }
    }
}