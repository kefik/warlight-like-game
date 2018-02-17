namespace Communication.Shared
{
    /// <summary>
    /// Handles processing of commands, namely translating, interpreting
    /// and translating back.
    /// </summary>
    public interface ICommandProcessor
    {
        /// <summary>
        /// Processes input passed in parameter and returns answer or null.
        /// </summary>
        /// <param name="input">Input</param>
        /// <returns>Output</returns>
        string Process(string input);
    }
}