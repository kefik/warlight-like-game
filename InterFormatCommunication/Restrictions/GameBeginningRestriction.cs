namespace InterFormatCommunication.Restrictions
{
    using System.Collections.Generic;
    using GameAi.Interfaces;

    public class GameBeginningRestriction : IGameBeginningRestriction
    {
        public int PlayerId { get; set; }
        public int RegionsPlayerCanChooseCount { get; set; }
        public ICollection<int> RestrictedRegions { get; set; }
    }
}