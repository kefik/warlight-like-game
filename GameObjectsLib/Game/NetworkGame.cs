﻿namespace GameObjectsLib.Game
{
    using System;
    using System.Collections.Generic;
    using GameMap;
    using ProtoBuf;

    [ProtoContract]
    internal class NetworkGame : Game
    {
        public override GameType GameType
        {
            get { return GameType.MultiplayerNetwork; }
        }

        public NetworkGame(int id, Map map, IList<Player> players) : base(id, map, players)
        {
        }

        public override void Validate()
        {
            throw new NotImplementedException();
        }
    }
}
