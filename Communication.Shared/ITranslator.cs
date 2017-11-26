namespace Communication.Shared
{
    public interface ITranslator
    {
        ICommandToken Translate(string input);
        string Translate(ICommandToken commandToken);
    }
}