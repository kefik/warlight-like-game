﻿namespace Communication.CommandHandling.Tokens.SetupMap
{
    using System.Collections.Generic;
    using Shared;

    /// <summary>
    /// Token containing information about setting up wastelands.
    /// </summary>
    public class SetupWastelandsToken : ISetupMapToken
    {
        public ICollection<int> Regions { get; }

        public SetupWastelandsToken(ICollection<int> regions)
        {
            Regions = regions;
        }
    }
}