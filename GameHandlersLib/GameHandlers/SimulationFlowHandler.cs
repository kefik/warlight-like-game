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
        private readonly BotEvaluationHandler botEvaluationHandler;
        private readonly GameRecordHandler gameRecordHandler;

        public Game Game { get; }

        public MapImageProcessor ImageProcessor { get; }

        public bool IsRunning { get; private set; }

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
            
            botEvaluationHandler = new BotEvaluationHandler(game);

            gameRecordHandler = new GameRecordHandler(processor);
            gameRecordHandler.Load(game);
        }

        public async Task StartOrContinueEvaluationAsync()
        {
            if (IsRunning)
            {
                throw new ArgumentException("Cannot start evaluation if it's already been started.");
            }

            IsRunning = true;
            // continue playing the bot
            try
            {
                await botEvaluationHandler.StartOrContinueEvaluationAsync();
            }
            catch (OperationCanceledException)
            {
                // ignore
            }

            gameRecordHandler.Load(Game);

            IsRunning = false;
        }

        public void PauseEvaluation()
        {
            // stop evaluation of currently playing bot immediately
            botEvaluationHandler.PauseEvaluation();
        }

        public void MoveToNextAction()
        {
            gameRecordHandler.MoveToNextAction();
        }

        public void MoveToPreviousAction()
        {
            gameRecordHandler.MoveToPreviousAction();
        }

        public void MoveToNextRoundAsync()
        {
            gameRecordHandler.MoveToNextRound();
        }

        public async Task MoveToPreviousRoundAsync()
        {
            
        }
    }
}