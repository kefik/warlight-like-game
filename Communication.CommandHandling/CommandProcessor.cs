namespace Communication.CommandHandling
{
    using System;
    using System.Diagnostics;
    using Shared;

    public class CommandProcessor : ICommandProcessor
    {
        private readonly ITranslator translator;
        private readonly ICommandHandler commandHandler;

        public CommandProcessor(ITranslator translator)
        {
            this.translator = translator;
            this.commandHandler = new CommandHandler();
        }

        public string Process(string input)
        {
            try
            {
                var inputCommandToken = translator.Translate(input);

                var outputCommandToken = commandHandler.Execute(inputCommandToken);

                string output = translator.Translate(outputCommandToken);

                return output;
            }
            // NotImplementedException => I can ignore this command
            catch (NotImplementedException exc)
            {
                Debug.WriteLine(exc);
#if DEBUG
                throw;
#endif
                return null;
            }
        }
    }
}