using System;
using System.Collections.Generic;
using ConquestObjectsLib.Game;
using ConquestObjectsLib.GameMap;

namespace ConquestObjectsLib.Game
{
    public class SingleplayerGame : Game
    {
        public SingleplayerGame(Map map, ICollection<Player> players) : base(map, players)
        {
        }

        public override bool HasStarted { get; protected set; }
        public override bool IsCreated { get; protected set; }

        int playersLimit;

        public int PlayersLimit
        {
            get { return playersLimit; }
            set
            {
                if (IsCreated) throw new Exception();
                if (playersLimit > Map.PlayersMax) throw new Exception();

                playersLimit = value;
            }
        }

        public override GameType GameType
        {
            get { return GameType.SinglePlayer; }
        }

        public override void Create()
        {
            // TODO: validation
            IsCreated = true;
        }

        public override void Start()
        {
            if (!IsCreated) Create();

            // TODO: validation
            HasStarted = true;
        }
    }
}
