namespace Communication.CommandHandling.Tokens.SetupMap
{
    using System.Collections.Generic;
    using Shared;

    public class SetupSuperRegionsToken : ICommandToken
    {
        public CommandTokenType CommandTokenType
        {
            get { return CommandTokenType.SetupSuperRegions; }
        }

        public ICollection<(int SuperRegionId, int BonusArmy)> InitialChanges { get; }

        public SetupSuperRegionsToken(ICollection<(int SuperRegionId, int BonusArmy)> initialChanges)
        {
            InitialChanges = initialChanges;
        }
    }
}