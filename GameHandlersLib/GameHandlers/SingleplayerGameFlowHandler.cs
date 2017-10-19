namespace GameHandlersLib.GameHandlers
{
    using System.Linq;
    using GameObjectsLib;
    using GameObjectsLib.Game;
    using MapHandlers;

    public sealed class SingleplayerGameFlowHandler : GameFlowHandler
    {
        public SingleplayerGameFlowHandler(Game game, MapImageProcessor processor) : base(game, processor)
        {
            PlayerOnTurn = (HumanPlayer)game.Players.First(x => x.GetType() == typeof(HumanPlayer));
        }
    }
}