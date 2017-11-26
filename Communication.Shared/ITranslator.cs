namespace Communication.Shared
{
    using System.Collections.Generic;

    /// <summary>
    /// Can translate into string input into command and other way.
    /// </summary>
    public interface ITranslator
    {
        /// <summary>
        /// Translates string into command.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        ICommandToken Translate(string input);

        /// <summary>
        /// Translates command into string.
        /// </summary>
        /// <param name="commandToken"></param>
        /// <returns></returns>
        string Translate(ICommandToken commandToken);
    }
}