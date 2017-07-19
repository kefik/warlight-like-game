using System;
using System.Collections.Generic;
using System.Linq;
using GameObjectsLib.Game;
using GameObjectsLib.GameMap;

namespace GameObjectsLib.Game
{
    /// <summary>
    /// Instance represents one singleplayer game.
    /// </summary>
    public class SingleplayerGame : GameObjectsLib.Game.Game
    {
        /// <summary>
        /// Instance of this class serves to validate correctness of the game before it starts.
        /// </summary>
        class SingleplayerGameValidator
        {
            readonly SingleplayerGame game;
            public SingleplayerGameValidator(SingleplayerGame game)
            {
                this.game = game;
            }
            /// <summary>
            /// Decides whether the game has too many players.
            /// </summary>
            /// <returns>True if it has more players than the limit of the map.</returns>
            public bool HasTooManyPlayers()
            {
                return game.Players.Count > game.Map.PlayersLimit;
            }
            /// <summary>
            /// Decides whether the game at least 2 players.
            /// </summary>
            /// <returns>True if it has at least 2 players.</returns>
            public bool HasEnoughPlayers()
            {
                return game.Players.Count >= 2;
            }
            /// <summary>
            /// Decides whether the game has exactly one human player.
            /// </summary>
            /// <returns>True if it has at least 1 human player.</returns>
            public bool HasOneHumanPlayer()
            {
                return (from player in game.Players
                        where player.GetType() == typeof(HumanPlayer)
                        select player).Count() == 1;
            }
        }
        public SingleplayerGame(Map map, ICollection<Player> players) : base(map, players)
        {
        }
        

        public override GameType GameType
        {
            get { return GameType.SinglePlayer; }
        }
        

        public override void Start()
        {
            SingleplayerGameValidator validator = new SingleplayerGameValidator(this);

            // validates the game before it starts
            if (!validator.HasEnoughPlayers()) throw new ArgumentException();
            if (validator.HasTooManyPlayers()) throw new ArgumentException();
            if (!validator.HasOneHumanPlayer()) throw new ArgumentException();
            

            // TODO: save it to the database

            HasStarted = true;
        }
    }
}
