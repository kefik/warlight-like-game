namespace Communication.CommandHandling.Tokens
{
    using System.Collections.Generic;
    using Shared;

    public class PickStartingRegionsResponseToken : ICommandToken
    {
        public ICollection<int> RegionIds { get; }

        public PickStartingRegionsResponseToken(ICollection<int> regionIds)
        {
            RegionIds = regionIds;
        }
    }
}