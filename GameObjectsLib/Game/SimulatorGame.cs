namespace GameObjectsLib.Game
{
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

        public SimulatorGame(int id, Map map, IList<Player> players, bool isFogOfWar, GameObjectsRestrictions objectsRestrictions)
            : base(id, map, players, isFogOfWar, objectsRestrictions)
        {
            
        }

        public override void Validate()
        {
            throw new System.NotImplementedException();
        }
    }
}