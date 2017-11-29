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

        /// <summary>
        /// Number of regions player can pick.
        /// </summary>
        public int PickLimit { get; set; }

        public PickStartingRegionsRequestToken(ICollection<int> regionIds, int pickLimit, TimeSpan? timeOut = null)
        {
            RegionIds = regionIds;
            Timeout = timeOut;
            PickLimit = pickLimit;
        }
    }
}