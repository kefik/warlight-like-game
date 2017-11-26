namespace Communication.CommandHandling
{
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
            var inputCommandToken = translator.Translate(input);

            var outputCommandToken = commandHandler.Execute(inputCommandToken);

            string output = translator.Translate(outputCommandToken);

            return output;
        }
    }
}