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
    internal class HotseatGame : Game
    {
        public override GameType GameType
        {
            get { return GameType.MultiplayerHotseat; }
        }

        public HotseatGame(int id, Map map, IList<Player> players, bool isFogOfWar, GameObjectsRestrictions objectsRestrictions)
            : base(id, map, players, isFogOfWar, objectsRestrictions)
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
