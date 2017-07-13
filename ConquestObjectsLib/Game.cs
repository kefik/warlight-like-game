using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConquestObjectsLib.GameMap;

namespace ConquestObjectsLib
{
    /// <summary>
    /// Represents one game.
    /// </summary>
    class Game
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

        public Game(Map map, ICollection<Player> players)
        {
            this.Map = map;
            this.Players = players;
        }

        /// <summary>
        /// Starts the game if theres no error.
        /// </summary>
        public void Start()
        {
            // check everything
            if (map == null || Players?.DefaultIfEmpty() == null || Started) throw new ArgumentException();

            // start the game
            Started = true;
        }
        

    }
}
