namespace Communication.CommandHandling.Tokens.Settings
{
    using GameAi;
    using Shared;

    public class SetupBotToken : ICommandToken
    {
        public CommandTokenType CommandTokenType
        {
            get { return CommandTokenType.SettingsBot; }
        }

        public int PlayerId { get; }
        public OwnerPerspective OwnerPerspective { get; }

        public SetupBotToken(int playerId, OwnerPerspective ownerPerspective)
        {
            PlayerId = playerId;
            OwnerPerspective = ownerPerspective;
        }
    }
}