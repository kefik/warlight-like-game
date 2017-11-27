namespace Communication.CommandHandling.Tokens.Settings
{
    using Shared;

    public class StartingPickRegionsCountToken : ICommandToken
    {
        public CommandTokenType CommandTokenType
        {
            get { return CommandTokenType.SettingsStartingPickAmount; }
        }

        public int RegionsToPickCount { get; }

        public StartingPickRegionsCountToken(int regionsToPickCount)
        {
            RegionsToPickCount = regionsToPickCount;
        }
    }
}