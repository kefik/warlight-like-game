﻿namespace Communication.CommandHandling.Tokens
{
    using System.Collections.Generic;
    using Shared;

    public class UpdateMapToken : ICommandToken
    {
        public CommandTokenType CommandTokenType
        {
            get
            {
                return CommandTokenType.UpdateMap;
            }
        }

        public ICollection<(int RegionId, int Owner, int Army)> Changes { get; }

        public UpdateMapToken(ICollection<(int RegionId, int Owner, int Army)> changes)
        {
            Changes = changes;
        }
    }
}