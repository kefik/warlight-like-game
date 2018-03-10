namespace GameHandlersLib.GameHandlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GameObjectsLib;
    using GameObjectsLib.Game;
    using GameObjectsLib.GameUser;
    using GameObjectsLib.Players;
    using MapHandlers;
    using IMapImageProcessor = MapHandlers.IMapImageProcessor;

    public sealed class HotseatGameFlowHandler : GameFlowHandler
    {
        private IEnumerator<HumanPlayer> playersEnumerator;
        private readonly IEnumerable<HumanPlayer> localPlayers;

        public override HumanPlayer PlayerOnTurn
        {
            get { return playersEnumerator?.Current; }
        }

        public HotseatGameFlowHandler(Game game, IMapImageProcessor processor) : base(game, processor)
        {
            localPlayers = from player in game.Players
                           where player.GetType() == typeof(HumanPlayer)
                           let humanPlayer = (HumanPlayer)player
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
        private bool NextLocalPlayer()
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

        /// <summary>
        /// Resets players, iterating through them once more.
        /// </summary>
        private void Reset()
        {
            playersEnumerator = localPlayers.GetEnumerator();
            playersEnumerator.MoveNext();
        }

        public override bool NextPlayer()
        {
            return NextLocalPlayer();
        }

        public override void PlayRound()
        {
            // cannot play round if theres any other player to play
            if (NextLocalPlayer())
            {
                throw new ArgumentOutOfRangeException("Cannot play round if there is any other player on turn this round.");
            }

            base.PlayRound();
            Reset();
        }
    }
}