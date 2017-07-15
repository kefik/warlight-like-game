using System;
using System.Collections.Generic;
using ConquestObjectsLib.Game;
using ConquestObjectsLib.GameMap;

namespace ConquestObjectsLib.Game
{
    public class NetworkGame : Game
    {
        public bool IsCreated { get; protected set; }

        
        public override GameType GameType
        {
            get { return GameType.MultiplayerNetwork; }
        }

        public NetworkGame(Map map, ICollection<Player> players) : base(map, players)
        {
        }


        
        /// <summary>
        /// Creates a game and sends it to the server.
        /// </summary>
        public void Create()
        {
            // TODO: validation
            IsCreated = true;
        }

        /// <summary>
        /// Starts the game.
        /// </summary>
        public override void Start()
        {
            if (!IsCreated) Create();

            // initialization
            Map.Initialize();

            // TODO: validation
            HasStarted = true;
        }
    }
}
