namespace GameObjectsLib.Game
{
    using System;
    using System.Collections.Generic;
    using GameMap;
    using ProtoBuf;

    [ProtoContract]
    internal class HotseatGame : Game
    {
        public override GameType GameType
        {
            get { return GameType.MultiplayerHotseat; }
        }

        public HotseatGame(int id, Map map, IList<Player> players) : base(id, map, players)
        {
        }

        // ReSharper disable once UnusedMember.Local
        private HotseatGame()
        {
        }

        public override void Validate()
        {
            throw new NotImplementedException();
        }
    }
}
