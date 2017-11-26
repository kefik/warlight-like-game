namespace Communication.Shared
{
    /// <summary>
    /// Handles command and executes it.
    /// </summary>
    public interface ICommandHandler
    {
        ICommandToken Execute(ICommandToken commandTokens);
    }
}