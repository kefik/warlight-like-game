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

    /// <summary>
    /// Control that handles simulation.
    /// Its purpose is to provide easy API for simulation.
    /// </summary>
    public class SimulationFlowHandler
    {
        /// <summary>
        /// Its purpose is to be used to properly lock this handler
        /// at certain time.
        /// </summary>
        private readonly object simulationLock = new object();
        /// <summary>
        /// Handles bot evaluation.
        /// </summary>
        private readonly BotEvaluationHandler botEvaluationHandler;

        /// <summary>
        /// Handles playing the record.
        /// </summary>
        private readonly GameRecordHandler gameRecordHandler;

        /// <summary>
        /// Player perspective from which the simulation is viewed.
        /// </summary>
        private Player playerPerspective;

        public Game Game { get; }

        public IMapImageProcessor ImageProcessor { get; }
        
        /// <summary>
        /// Reports whether the simulation is running
        /// at the moment.
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Is invoked when image is changed.
        /// </summary>
        public event Action OnImageChanged
        {
            add { ImageProcessor.OnImageChanged += value; }
            remove { ImageProcessor.OnImageChanged -= value; }
        }

        public SimulationFlowHandler(Game game,
            IMapImageProcessor processor,
            Player playerPerspective)
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

            this.playerPerspective = playerPerspective;
            gameRecordHandler = new GameRecordHandler(processor, game, playerPerspective);
        }

        /// <summary>
        /// Changes player perspective from which the simulation
        /// is viewed (noticable only in fog of war mode).
        /// </summary>
        /// <param name="playerPerspective">
        /// Player perspective from which the simulation will be viewed.
        /// Null means GOD mode (can see everything).
        /// </param>
        public void ChangePlayerPerspective(Player playerPerspective)
        {
            this.playerPerspective = playerPerspective;
            gameRecordHandler.Load(Game, playerPerspective);
        }

        /// <summary>
        /// Asynchronously starts or continue bots evaluation with
        /// specified maximum time for each bot move.
        /// </summary>
        /// <param name="timeForBotMove"></param>
        /// <returns></returns>
        public async Task StartOrContinueEvaluationAsync(TimeSpan timeForBotMove)
        {
            // lock bcuz more threads could come before IsRunning = true line
            lock (simulationLock)
            {
                if (IsRunning)
                {
                    throw new ArgumentException("Cannot start evaluation if it's already been started.");
                }

                IsRunning = true;
            }
            // continue playing the bot
            try
            {
                await botEvaluationHandler.StartOrContinueEvaluationAsync(timeForBotMove);
            }
            catch (OperationCanceledException)
            {
                // ignore
            }
            catch (GameFinishedException)
            {
                // ignore for now
            }
            finally
            {
                gameRecordHandler.Load(Game, playerPerspective);

                IsRunning = false;
            }
        }

        /// <summary>
        /// Pauses the bot evaluation.
        /// </summary>
        public void PauseEvaluation()
        {
            // stop evaluation of currently playing bot immediately
            botEvaluationHandler.PauseEvaluation();
        }

        /// <summary>
        /// Moves the game record player to the next action
        /// the given bot can see.
        /// </summary>
        public void MoveToNextAction()
        {
            gameRecordHandler.MoveToNextAction();
        }

        /// <summary>
        /// Moves the game record player to the previous action
        /// the given bot can see.
        /// </summary>
        public void MoveToPreviousAction()
        {
            gameRecordHandler.MoveToPreviousAction();
        }

        /// <summary>
        /// Moves the game record player to the next round.
        /// </summary>
        public void MoveToNextRound()
        {
            gameRecordHandler.MoveToNextRound();
        }

        /// <summary>
        /// Moves the game record player to the previous round.
        /// </summary>
        public void MoveToPreviousRound()
        {
            gameRecordHandler.MoveToPreviousRound();
        }

        /// <summary>
        /// Moves the game record player to the beginning of the game.
        /// </summary>
        public void MoveToBeginning()
        {
            gameRecordHandler.MoveToBeginning();
        }

        /// <summary>
        /// Moves the game record player to the end of the game.
        /// </summary>
        public void MoveToEnd()
        {
            gameRecordHandler.MoveToEnd();
        }
    }
}