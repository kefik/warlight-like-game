namespace Communication.CommandHandling.Tokens.Settings
{
    using Shared;
    public class StartingArmiesToken : ICommandToken
    {
        public int StartingArmySize { get; }

        public StartingArmiesToken(int startingArmySize)
        {
            StartingArmySize = startingArmySize;
        }
    }
}