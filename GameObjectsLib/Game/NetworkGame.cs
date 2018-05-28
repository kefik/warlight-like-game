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

        public NetworkGame(Map map, IList<Player> players, bool isFogOfWar,
            GameObjectsRestrictions objectsRestrictions) : base(map, players, isFogOfWar, objectsRestrictions)
        {
        }

        public override void Validate()
        {
            throw new NotImplementedException();
        }
    }
}
