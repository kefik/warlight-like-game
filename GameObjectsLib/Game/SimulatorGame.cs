namespace GameObjectsLib.Game
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using GameAi.Data.Restrictions;
    using GameMap;
    using GameRestrictions;
    using Players;
    using ProtoBuf;

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    [ProtoContract]
    internal class SimulatorGame : Game
    {
        public override GameType GameType
        {
            get
            {
                return GameType.Simulator;
            }
        }

        private SimulatorGame()
        {
            
        }

        public SimulatorGame(Map map, IList<Player> players, bool isFogOfWar, GameObjectsRestrictions objectsRestrictions)
            : base(map, players, isFogOfWar, objectsRestrictions)
        {
            if (objectsRestrictions == null)
            {
                throw new ArgumentException("Restrictions cannot be null.");
            }
        }

        public override void Validate()
        {
            throw new System.NotImplementedException();
        }
    }
}