namespace Communication.CommandHandling.Tokens
{
    using System.Collections.Generic;
    using Shared;

    public class PickStartingRegionsResponseToken : ICommandToken
    {
        public CommandTokenType CommandTokenType
        {
            get
            {
                return CommandTokenType.PickStartingRegionsResponse;
            }
        }

        public ICollection<int> RegionIds { get; }

        public PickStartingRegionsResponseToken(ICollection<int> regionIds)
        {
            RegionIds = regionIds;
        }
    }
}