namespace GameAi.Data.Restrictions
{
    using System.Collections.Generic;
    using Interfaces;

    public class GameBeginningRestriction : IGameBeginningRestriction
    {
        public int PlayerId { get; set; }
        public int RegionsPlayerCanChooseCount { get; set; }
        public ICollection<int> RestrictedRegions { get; set; }
    }
}