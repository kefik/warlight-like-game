namespace GameAi.Data.Restrictions
{
    using System.Collections.Generic;
    using Interfaces;
    using ProtoBuf;

    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    public class GameBeginningRestriction
    {
        public int PlayerId { get; set; }
        public int RegionsPlayerCanChooseCount { get; set; }
        public ICollection<int> RestrictedRegions { get; set; }
    }
}