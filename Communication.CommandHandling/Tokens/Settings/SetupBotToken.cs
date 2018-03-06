namespace Communication.CommandHandling.Tokens.Settings
{
    using GameAi;
    using GameAi.Data;
    using Shared;

    public class SetupBotToken : ICommandToken
    {
        public int PlayerId { get; }
        public OwnerPerspective OwnerPerspective { get; }

        public SetupBotToken(int playerId, OwnerPerspective ownerPerspective)
        {
            PlayerId = playerId;
            OwnerPerspective = ownerPerspective;
        }
    }
}