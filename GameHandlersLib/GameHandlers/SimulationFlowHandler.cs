namespace GameHandlersLib.GameHandlers
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using GameAi;
    using GameObjectsLib.Game;
    using GameObjectsLib.Players;
    using MapHandlers;

    public class SimulationFlowHandler
    {
        public Game Game { get; }

        public MapImageProcessor ImageProcessor { get; }

        private WarlightAiBotHandler[] botHandlers;

        public event Action OnImageChanged
        {
            add { ImageProcessor.OnImageChanged += value; }
            remove { ImageProcessor.OnImageChanged -= value; }
        }

        public SimulationFlowHandler(Game game, MapImageProcessor processor)
        {
            if (game.RoundNumber != 0)
            {
                throw new ArgumentException("Cannot simulate game that has already begun.");
            }

            if (game.Players.Any(x => x.GetType() != typeof(AiPlayer)))
            {
                throw new ArgumentException("Only AI players can be used in the simulation.");
            }

            Game = game;
            ImageProcessor = processor;

            botHandlers = new WarlightAiBotHandler[game.Players.Count];
        }

        public async Task PlayAsync()
        {
            var bots = Game.Players;

            for (int index = 0; index < bots.Count; index++)
            {
                var botHandler = new WarlightAiBotHandler(Game, bots[index], GameBotType.MonteCarloTreeSearchBot,
                    null);

                var bestMoveTask = Task.Run(botHandler.FindBestMoveAsync);
                await botHandler.StopEvaluation(new TimeSpan(0, 0, 0, 2));

                var bestMove = await bestMoveTask;
            }
        }

        public void PauseEvaluation()
        {
            // TODO: stop evaluation of currently playing bot immediately
        }

        public async Task ContinueEvaluationAsync()
        {
            // TODO: continue playing the bot
        }

        public void SkipAction()
        {
            
        }

        public void ReturnAction()
        {
            
        }

        public void SkipTurn()
        {
            
        }

        public void ReturnTurn()
        {
            
        }

        public void SkipRound()
        {
            
        }

        public void ReturnRound()
        {
            
        }
    }
}