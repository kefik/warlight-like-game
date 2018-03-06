namespace TheAiGames.EngineCommHandler
{
    using System;
    using System.IO;
    using Communication.CommandHandling;
    using Communication.Shared;
    using TranslationUnits;

    internal class Program
    {
        private static TextReader reader;
        private static TextWriter writer;

        private static void Main(string[] args)
        {
            reader = Console.In;
            reader = new StreamReader("file.txt");
            writer = Console.Out;

            ITranslator translator = new Translator();
            ICommandProcessor commandProcessor = new CommandProcessor(translator);
            
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.Length == 0)
                {
                    continue;
                }

                string answer = commandProcessor.Process(line);

                if (answer != null)
                {
                    writer.WriteLine(answer);
                }
            }
        }
    }
}
