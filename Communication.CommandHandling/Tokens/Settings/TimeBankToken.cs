namespace Communication.CommandHandling.Tokens.Settings
{
    using System;
    using Shared;

    /// <summary>
    /// Represents how much time does player have.
    /// </summary>
    public class TimeBankToken : ICommandToken
    {
        public CommandTokenType CommandTokenType
        {
            get { return CommandTokenType.SettingsTimeBank; }
        }

        public TimeSpan? TimeBankInterval { get; }

        public TimeBankToken(TimeSpan? timeBankInterval = null)
        {
            TimeBankInterval = timeBankInterval;
        }
    }
}