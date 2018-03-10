namespace GameHandlersLib.GameHandlers
{
    using System.Linq;
    using GameObjectsLib;
    using GameObjectsLib.Game;
    using GameObjectsLib.Players;
    using MapHandlers;
    using IMapImageProcessor = MapHandlers.IMapImageProcessor;

    public sealed class SingleplayerGameFlowHandler : GameFlowHandler
    {
        public SingleplayerGameFlowHandler(Game game, IMapImageProcessor processor) : base(game, processor)
        {
            PlayerOnTurn = (HumanPlayer)game.Players.First(x => x.GetType() == typeof(HumanPlayer));
        }
        
        public override bool NextPlayer()
        {
            return false;
        }
    }
}