namespace Communication.CommandHandling.Tokens
{
    using System.Collections.Generic;
    using Shared;

    public class AttackResponseToken : ICommandToken
    {
        public CommandTokenType CommandTokenType
        {
            get
            {
                return CommandTokenType.AttackResponse;
            }
        }

        public int PlayerId { get; }

        public ICollection<(int AttackingRegionId, int DefendingRegionId, int Army)> Attacks { get; }

        public AttackResponseToken(int playerId,
            ICollection<(int AttackingRegionId, int DefendingRegionId, int Army)> attacks = null)
        {
            PlayerId = playerId;
            Attacks = attacks;
        }
    }
}