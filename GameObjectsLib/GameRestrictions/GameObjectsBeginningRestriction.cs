namespace GameObjectsLib.GameRestrictions
{
    using System.Collections.Generic;
    using GameMap;
    using Players;
    using ProtoBuf;

    [ProtoContract]
    public class GameObjectsBeginningRestriction
    {
        [ProtoMember(1, AsReference = true)]
        public Player Player { get; set; }
        [ProtoMember(2)]
        public int RegionsToChooseCount { get; set; }

        [ProtoMember(3, AsReference = true)]
        public ICollection<Region> RegionsPlayersCanChoose { get; set; }
    }
}