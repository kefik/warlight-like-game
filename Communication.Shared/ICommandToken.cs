namespace Communication.Shared
{
    /// <summary>
    /// Represents a command.
    /// </summary>
    public interface ICommandToken
    {
        /// <summary>
        /// Type of the command.
        /// </summary>
        CommandTokenType CommandTokenType { get; }
    }
}