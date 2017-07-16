using System;
using System.Collections.Generic;
using ConquestObjectsLib.GameMap;

namespace ConquestObjectsLib.Game
{
    /// <summary>
    /// Enum containing types of game any game can have.
    /// </summary>
    public enum GameType
    {
        None,
        SinglePlayer,
        MultiplayerHotseat,
        MultiplayerNetwork
    }

    /// <summary>
    /// Represents one game.
    /// </summary>
    public abstract class Game
    {
        /// <summary>
        /// Represents boolean containing information whether the game has started.
        /// </summary>
        public bool HasStarted { get; protected set; }
        
        /// <summary>
        /// Represents map being played in this game.
        /// </summary>
        public Map Map { get; }

        /// <summary>
        /// Represents list of players playing this game.
        /// </summary>
        public ICollection<Player> Players { get; }

        /// <summary>
        /// Return game type this game has.
        /// </summary>
        public abstract GameType GameType { get; }
        
        protected Game(Map map, ICollection<Player> players)
        {
            if (map.IsInitialized) throw new ArgumentException();

            this.Map = map;
            this.Players = players;
        }

        /// <summary>
        /// Starts the game if theres no error.
        /// </summary>
        public abstract void Start();
    }

}
