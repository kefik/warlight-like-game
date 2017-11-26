namespace Communication.Shared
{
    public interface ICommandHandler
    {
        ICommandToken Execute(ICommandToken commandTokens);
    }
}