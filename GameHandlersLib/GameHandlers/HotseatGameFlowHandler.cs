namespace GameHandlersLib.GameHandlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GameObjectsLib;
    using GameObjectsLib.Game;
    using GameObjectsLib.GameUser;
    using MapHandlers;

    public sealed class HotseatGameFlowHandler : GameFlowHandler
    {
        private readonly IEnumerator<HumanPlayer> playersEnumerator;

        public override HumanPlayer PlayerOnTurn
        {
            get { return playersEnumerator?.Current; }
        }

        public HotseatGameFlowHandler(Game game, MapImageProcessor processor) : base(game, processor)
        {
            IEnumerable<HumanPlayer> localPlayers = from player in game.Players
                                                    where player.GetType() == typeof(HumanPlayer)
                                                    let humanPlayer = (HumanPlayer) player
                                                    where humanPlayer.User.UserType == UserType.LocalUser
                                                          || humanPlayer.User.UserType == UserType.MyNetworkUser
                                                    select humanPlayer;
            playersEnumerator = localPlayers.GetEnumerator();
            NextLocalPlayer();
        }

        /// <summary>
        ///     Moves to next player.
        /// </summary>
        /// <returns>False, if player is theres no next player, true otherwise.</returns>
        public bool NextLocalPlayer()
        {
            bool isThereNextPlayer = playersEnumerator.MoveNext();

            // theres no next local player
            if (!isThereNextPlayer)
            {
                return false;
            }

            // the player was defeated
            if (PlayerOnTurn.IsDefeated(GameState))
            {
                return NextLocalPlayer();
            }

            return true;
        }

        public override void PlayRound()
        {
            // cannot play round if theres any other player to play
            if (NextLocalPlayer())
            {
                throw new ArgumentOutOfRangeException();
            }

            base.PlayRound();
            playersEnumerator.Reset();
        }
    }
}