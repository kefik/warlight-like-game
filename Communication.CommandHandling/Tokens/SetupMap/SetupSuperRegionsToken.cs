namespace Communication.CommandHandling.Tokens.SetupMap
{
    using System.Collections.Generic;
    using Shared;

    /// <summary>
    /// Token which contains neighbour inicialization data.
    /// </summary>
    public class SetupSuperRegionsToken : ISetupMapToken
    {
        public ICollection<(int SuperRegionId, int BonusArmy)> InitialChanges { get; }

        public SetupSuperRegionsToken(ICollection<(int SuperRegionId, int BonusArmy)> initialChanges)
        {
            InitialChanges = initialChanges;
        }
    }
}