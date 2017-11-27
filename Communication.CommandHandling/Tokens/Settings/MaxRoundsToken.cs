namespace Communication.CommandHandling.Tokens.Settings
{
    using Shared;
    public class MaxRoundsToken : ICommandToken
    {
        public CommandTokenType CommandTokenType
        {
            get { return CommandTokenType.SettingsMaxRounds; }
        }

        public int MaxRoundsCount { get; }

        public MaxRoundsToken(int maxRoundsCount)
        {
            MaxRoundsCount = maxRoundsCount;
        }
    }
}