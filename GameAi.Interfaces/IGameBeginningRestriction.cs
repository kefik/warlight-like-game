namespace GameAi.Interfaces
{
    using System.Collections.Generic;
    
    public interface IGameBeginningRestriction : IRestriction
    {
        int RegionsPlayerCanChooseCount { get; set; }
        ICollection<int> RestrictedRegions { get; set; }
    }
}