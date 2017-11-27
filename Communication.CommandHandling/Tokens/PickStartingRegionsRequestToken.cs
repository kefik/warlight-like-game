namespace Communication.CommandHandling.Tokens
{
    using System;
    using System.Collections.Generic;
    using Shared;

    public class PickStartingRegionsRequestToken : ICommandToken
    {
        public CommandTokenType CommandTokenType
        {
            get
            {
                return CommandTokenType.PickStartingRegionsRequest;
            }
        }

        public TimeSpan? Timeout { get; }

        public ICollection<int> RegionIds { get; }

        public PickStartingRegionsRequestToken(ICollection<int> regionIds, TimeSpan? timeOut = null)
        {
            RegionIds = regionIds;
            Timeout = timeOut;
        }
    }
}