namespace Communication.Shared
{
    /// <summary>
    /// Can translate string input into command and other way.
    /// </summary>
    public interface ITranslator
    {
        /// <summary>
        /// Translates string into command.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        ICommandToken Translate(string command);

        /// <summary>
        /// Translates command into string.
        /// </summary>
        /// <param name="commandToken"></param>
        /// <returns></returns>
        string Translate(ICommandToken commandToken);
    }
}