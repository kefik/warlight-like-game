namespace GameObjectsLib.Game
{
    using System;
    using System.Collections.Generic;
    using GameAi.Data.Restrictions;
    using GameMap;
    using GameRestrictions;
    using Players;
    using ProtoBuf;

    [ProtoContract]
    internal class NetworkGame : Game
    {
        public override GameType GameType
        {
            get { return GameType.MultiplayerNetwork; }
        }

        public NetworkGame(int id, Map map, IList<Player> players, bool isFogOfWar,
            GameRestrictions restrictions) : base(id, map, players, isFogOfWar, restrictions)
        {
        }

        public override void Validate()
        {
            throw new NotImplementedException();
        }
    }
}
