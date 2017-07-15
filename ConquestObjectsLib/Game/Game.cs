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
        public abstract bool HasStarted { get; protected set; }

        /// <summary>
        /// Represents boolean containing information whether the game has been created.
        /// When it is true, some things are locked for editing.
        /// </summary>
        public abstract bool IsCreated { get; protected set; }

        Map map;

        /// <summary>
        /// Represents map being played in this game.
        /// </summary>
        public Map Map
        {
            get { return map; }
            set
            {
                if (IsCreated) throw new Exception(); // TODO: game has already started exception
                map = value;
            }
        }

        ICollection<Player> players;

        /// <summary>
        /// Represents list of players playing this game.
        /// </summary>
        public ICollection<Player> Players
        {
            get { return players; }
            set
            {
                if (HasStarted) throw new Exception(); // TODO: game has already started exception
                players = value;
            }
        }

        /// <summary>
        /// Return game type this game has.
        /// </summary>
        public abstract GameType GameType { get; }

        protected Game(Map map, ICollection<Player> players)
        {
            this.Map = map;
            this.Players = players;
        }

        /// <summary>
        /// Creates the game.
        /// </summary>
        public abstract void Create();

        /// <summary>
        /// Starts the game if theres no error.
        /// </summary>
        public abstract void Start();
    }

}
