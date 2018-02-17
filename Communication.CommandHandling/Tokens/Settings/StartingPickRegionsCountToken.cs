namespace Communication.CommandHandling.Tokens.Settings
{
    using Shared;

    public class StartingPickRegionsCountToken : ICommandToken
    {
        public int RegionsToPickCount { get; }

        public StartingPickRegionsCountToken(int regionsToPickCount)
        {
            RegionsToPickCount = regionsToPickCount;
        }
    }
}