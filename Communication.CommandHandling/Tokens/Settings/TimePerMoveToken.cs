namespace Communication.CommandHandling.Tokens.Settings
{
    using System;
    using Shared;
    public class TimePerMoveToken : ICommandToken
    {
        public CommandTokenType CommandTokenType
        {
            get { return CommandTokenType.SettingsTimePerMove; }
        }

        public TimeSpan Time { get; }

        public TimePerMoveToken(TimeSpan time)
        {
            Time = time;
        }
    }
}