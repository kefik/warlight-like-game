namespace GameHandlersLib.GameHandlers
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using GameAi;
    using GameAi.Data;
    using GameObjectsLib.Game;
    using GameObjectsLib.Players;
    using MapHandlers;

    public class SimulationFlowHandler
    {
        public Game Game { get; }

        public MapImageProcessor ImageProcessor { get; }

        public bool IsRunning { get; private set; }

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

        public async Task StartOrContinueEvaluationAsync()
        {
            if (IsRunning)
            {
                throw new ArgumentException("Cannot start evaluation if it's already been started.");
            }

            IsRunning = true;
            // TODO: continue playing the bot
        }

        public async Task PauseEvaluationAsync()
        {
            IsRunning = false;
            // TODO: stop evaluation of currently playing bot immediately
        }

        public async Task MoveToNextActionAsync()
        {
            
        }

        public async Task MoveToPreviousActionAsync()
        {
            
        }

        public async Task MoveToNextRoundAsync()
        {
            
        }

        public async Task MoveToPreviousRoundAsync()
        {
            
        }
    }
}