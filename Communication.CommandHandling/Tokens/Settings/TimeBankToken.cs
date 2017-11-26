namespace Communication.CommandHandling.Tokens.Settings
{
    using Shared;

    public class TimeBankToken : ICommandToken
    {
        public CommandTokenType CommandTokenType
        {
            get { return CommandTokenType.SettingsTimeBank; }
        }

        public int TimeBankInterval { get; }

        public TimeBankToken(int timeBankInterval)
        {
            TimeBankInterval = timeBankInterval;
        }
    }
}