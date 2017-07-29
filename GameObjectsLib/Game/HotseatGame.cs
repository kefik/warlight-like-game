using System;
using System.Collections.Generic;
using GameObjectsLib.Game;
using GameObjectsLib;
using GameObjectsLib.GameMap;
using ProtoBuf;

namespace GameObjectsLib.Game
{
   [ProtoContract]
   class HotseatGame : Game
    {
        public override GameType GameType
        {
            get { return GameType.MultiplayerHotseat; }
        }

        public HotseatGame(int id, Map map, IList<Player> players) : base(id, map, players)
        {
        }

        private HotseatGame() : base() { }

        public override void Validate()
        {
            throw new NotImplementedException();
        }
    }

}
