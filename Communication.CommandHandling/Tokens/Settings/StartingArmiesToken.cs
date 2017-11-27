namespace Communication.CommandHandling.Tokens.Settings
{
    using Shared;
    public class StartingArmiesToken : ICommandToken
    {
        public CommandTokenType CommandTokenType
        {
            get { return CommandTokenType.SettingsStartingArmies; }
        }

        public int StartingArmySize { get; }

        public StartingArmiesToken(int startingArmySize)
        {
            StartingArmySize = startingArmySize;
        }
    }
}