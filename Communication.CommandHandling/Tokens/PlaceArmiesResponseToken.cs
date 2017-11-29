namespace Communication.CommandHandling.Tokens
{
    using System.Collections.Generic;
    using Shared;

    public class PlaceArmiesResponseToken : ICommandToken
    {
        public CommandTokenType CommandTokenType
        {
            get { return CommandTokenType.PlaceArmiesResponse; }
        }

        public int PlayerId { get; }

        public ICollection<(int RegionId, int Army)> Changes { get; }

        public PlaceArmiesResponseToken(int playerId, ICollection<(int, int)> changes)
        {
            PlayerId = playerId;
            Changes = changes;
        }
    }
}