using System;
using System.Collections.Generic;
using ConquestObjectsLib.Game;
using ConquestObjectsLib.GameMap;

namespace ConquestObjectsLib.Game
{
    public class HotseatGame : Game
    {
        public override bool HasStarted { get; protected set; }
        public override bool IsCreated { get; protected set; }

        int aiPlayersLimit;
        // Max number of AI players that can play this game.
        public int AIPlayersLimit
        {
            get { return aiPlayersLimit; }
            set
            {
                if (IsCreated) throw new Exception(); // TODO: better exception
                if (value + humanPlayersLimit > Map.PlayersMax) throw new Exception();

                aiPlayersLimit = value;
            }
        }

        int humanPlayersLimit;
        // Max number of human players that can play this game.
        public int HumanPlayersLimit
        {
            get { return humanPlayersLimit; }
            set
            {
                if (IsCreated) throw new Exception(); // TODO: better exception
                if (value + aiPlayersLimit > Map.PlayersMax) throw new Exception();

                humanPlayersLimit = value;
            }
        }
        
        public override GameType GameType
        {
            get { return GameType.MultiplayerNetwork; }
        }

        public HotseatGame(Map map, ICollection<Player> players) : base(map, players)
        {
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
