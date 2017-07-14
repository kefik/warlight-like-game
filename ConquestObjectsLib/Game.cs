using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConquestObjectsLib.GameMap;

namespace ConquestObjectsLib
{
    /// <summary>
    /// Enum containing 3 types of game any game can have.
    /// </summary>
    public enum GameType
    {
        None, SinglePlayer, MultiplayerHotseat, MultiplayerNetwork
    }
    /// <summary>
    /// Represents one game.
    /// </summary>
    public class Game
    {
        /// <summary>
        /// Represents boolean containing information whether the game has started.
        /// </summary>
        public bool Started { get; private set; }

        Map map;
        /// <summary>
        /// Represents map being played in this game.
        /// </summary>
        public Map Map
        {
            get { return map; }
            set
            {
                if (Started) throw new Exception(); // TODO: game has already started exception
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
                if (Started) throw new Exception(); // TODO: game has already started exception
                players = value;
            }
        }

        GameType gameType;
        /// <summary>
        /// Represents game type this game has.
        /// </summary>
        public GameType GameType
        {
            get { return gameType; }
            set
            {
                if (Started) throw new Exception();
                gameType = value;
            }
        }

        public Game(GameType gameType, Map map, ICollection<Player> players)
        {
            this.gameType = gameType;
            this.Map = map;
            this.Players = players;
        }

        /// <summary>
        /// Starts the game if theres no error.
        /// </summary>
        public void Start()
        {
            // TODO: validate everything
            if (map == null || Players?.DefaultIfEmpty() == null || Started) throw new ArgumentException();
            
            // start the game
            Started = true;
        }
    }
}
