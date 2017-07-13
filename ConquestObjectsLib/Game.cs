using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConquestObjectsLib.GameMap;

namespace ConquestObjectsLib
{
    class Game
    {
        public Map Map { get; }
        public ICollection<Player> Players { get; }

        public Game(Map map, ICollection<Player> players)
        {
            this.Map = map;
            this.Players = players;
        }
    }
}
