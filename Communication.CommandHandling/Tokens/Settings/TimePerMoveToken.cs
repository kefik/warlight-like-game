namespace Communication.CommandHandling.Tokens.Settings
{
    using Shared;
    public class TimePerMoveToken : ICommandToken
    {
        public CommandTokenType CommandTokenType
        {
            get { return CommandTokenType.SettingsTimePerMove; }
        }

        public int Time { get; }

        public TimePerMoveToken(int time)
        {
            Time = time;
        }
    }
}