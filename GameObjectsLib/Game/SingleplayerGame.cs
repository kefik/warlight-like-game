namespace GameObjectsLib.Game
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GameAi.Data.Restrictions;
    using GameMap;
    using GameRestrictions;
    using Players;
    using ProtoBuf;

    /// <summary>
    ///     Instance represents one singleplayer game.
    /// </summary>
    [Serializable]
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    internal class SingleplayerGame : Game
    {
        private SingleplayerGame()
        {
        }

        /// <summary>
        ///     Instance of this class serves to validate correctness of the game before it starts.
        /// </summary>
        private class SingleplayerGameValidator
        {
            private readonly SingleplayerGame game;

            public SingleplayerGameValidator(SingleplayerGame game)
            {
                this.game = game;
            }

            /// <summary>
            ///     Decides whether the game has too many players.
            /// </summary>
            /// <returns>True if it has more players than the limit of the map.</returns>
            public bool HasTooManyPlayers()
            {
                return game.Players.Count > game.Map.PlayersLimit;
            }

            /// <summary>
            ///     Decides whether the game at least 2 players.
            /// </summary>
            /// <returns>True if it has at least 2 players.</returns>
            public bool HasEnoughPlayers()
            {
                return game.Players.Count >= 2;
            }

            /// <summary>
            ///     Decides whether the game has exactly one human player.
            /// </summary>
            /// <returns>True if it has at least 1 human player.</returns>
            public bool HasOneHumanPlayer()
            {
                return (from player in game.Players
                        where player.GetType() == typeof(HumanPlayer)
                        select player).Count() == 1;
            }
        }

        public SingleplayerGame(int id, Map map, IList<Player> players, bool isFogOfWar, GameObjectsRestrictions objectsRestrictions)
            : base(id, map, players, isFogOfWar, objectsRestrictions)
        {
        }


        public override GameType GameType
        {
            get { return GameType.SinglePlayer; }
        }


        public override void Validate()
        {
            SingleplayerGameValidator validator = new SingleplayerGameValidator(this);

            // validates the game before it starts
            if (!validator.HasEnoughPlayers())
            {
                throw new ArgumentException();
            }
            if (validator.HasTooManyPlayers())
            {
                throw new ArgumentException();
            }
            if (!validator.HasOneHumanPlayer())
            {
                throw new ArgumentException();
            }
        }
    }
}
