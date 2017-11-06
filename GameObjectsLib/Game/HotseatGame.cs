namespace GameObjectsLib.Game
{
    using System;
    using System.Collections.Generic;
    using GameMap;
    using Players;
    using ProtoBuf;

    [ProtoContract]
    internal class HotseatGame : Game
    {
        public override GameType GameType
        {
            get { return GameType.MultiplayerHotseat; }
        }

        public HotseatGame(int id, Map map, IList<Player> players, bool isFogOfWar) : base(id, map, players, isFogOfWar)
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
