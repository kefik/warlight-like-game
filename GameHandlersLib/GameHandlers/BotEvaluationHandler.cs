namespace GameHandlersLib.GameHandlers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using FormatConverters;
    using GameAi;
    using GameAi.Data;
    using GameAi.Data.Restrictions;
    using GameObjectsLib.Game;
    using GameObjectsLib.GameRecording;
    using GameObjectsLib.GameRestrictions;
    using GameObjectsLib.Players;

    /// <summary>
    /// Component that handles evaluation of bot players.
    /// </summary>
    public class BotEvaluationHandler
    {
        private object botEvaluationLock = new object();

        private readonly IList<Player> players;
        private readonly Game game;
        private readonly GameObjectsRestrictions objectsRestrictions;

        private WarlightAiBotHandler[] botHandlers;
        private Turn[] turns;

        private readonly RoundHandler roundHandler;

        private int currentlyEvaluatingIndex;

        private CancellationTokenSource cancellationTokenSource;

        public BotEvaluationHandler(Game game)
        {
            this.game = game;
            this.players = game.Players;
            this.objectsRestrictions = game.ObjectsRestrictions;
            roundHandler = new RoundHandler(game);

            cancellationTokenSource = new CancellationTokenSource();
        }

        public async Task StartOrContinueEvaluationAsync(TimeSpan timeForBotMove)
        {
            try
            {
                do
                {
                    await Task.Run(() =>
                        PlayOrContinuePlayingRoundAsync(timeForBotMove));
                } while (true);
            }
            finally
            {
                cancellationTokenSource = new CancellationTokenSource();
            }
        }

        /// <summary>
        /// Plays one round. If the round evaluation was
        /// already started, then it continues evaluating that round.
        /// </summary>
        /// <returns></returns>
        private async Task PlayOrContinuePlayingRoundAsync(TimeSpan timeForBotMove)
        {
            var token = cancellationTokenSource.Token;

            // no turn was played so far
            if (currentlyEvaluatingIndex == 0)
            {
                botHandlers = new WarlightAiBotHandler[players.Count];
                turns = new Turn[players.Count];
            }
            // create ids mapper
            var playerIdsMapper = players.CreateCompressedAiMapper();

            var restrictions = objectsRestrictions.ToRestrictions()
                .ToRemappedRestrictions(playerIdsMapper);

            var mapMin = game.Map.ToMapMin(playerIdsMapper);

            // play from index you stopped playing at
            while (true)
            {
                // if the game is finished, don't play more moves
                if (game.IsFinished())
                {
                    throw new GameFinishedException();
                }
                // fixing closure issue
                int currentIndex = currentlyEvaluatingIndex;

                // player isnt defeated => play him
                if (!players[currentIndex].IsDefeated(game.AllRounds.Count))
                {
                    if (!playerIdsMapper.TryGetNewId(players[currentIndex].Id, out int evaluationPlayerId))
                    {
                        throw new ArgumentException("Player ID must be correclty mapped in the dictionary");
                    }

                    if (players[currentIndex].GetType() == typeof(HumanPlayer))
                    {
                        botHandlers[currentIndex] = new WarlightAiBotHandler(
                            GameBotType.AggressiveBot,
                            mapMin, Difficulty.Hard,
                            (byte)evaluationPlayerId,
                            game.Players.Where(x => !x.IsDefeated(game.RoundNumber))
                                .Select(x => (byte)playerIdsMapper.GetNewId(x.Id)).ToArray(),
                            game.IsFogOfWar, restrictions);
                    }
                    else
                    {
                        var aiPlayer = (AiPlayer)players[currentIndex];
                        botHandlers[currentIndex] = new WarlightAiBotHandler(
                            aiPlayer.BotType,
                            mapMin, aiPlayer.Difficulty, (byte)evaluationPlayerId,
                            game.Players.Where(x => !x.IsDefeated(game.RoundNumber))
                                .Select(x => (byte)playerIdsMapper.GetNewId(x.Id)).ToArray(),
                            game.IsFogOfWar, restrictions);
                    }
                    var botTask = Task.Run(botHandlers[currentIndex].FindBestMoveAsync);
                    // break after specified amount of time
                    var waitTask = botHandlers[currentIndex].StopEvaluation(timeForBotMove);
                    var bestTurn = (await botTask).ToTurn(game.Map, game.Players, playerIdsMapper);
                    turns[currentlyEvaluatingIndex] = bestTurn;
                }

                // lock because currentlyEvaluatingIndex could overflow
                // + pause => index out of range exception
                lock (botEvaluationLock)
                {
                    currentlyEvaluatingIndex++;
                    // all bots have returned their turn => play it
                    // and quit the evaluation
                    if (currentlyEvaluatingIndex >= players.Count)
                    {
                        PlayRound(turns);
                        currentlyEvaluatingIndex = 0;
                        break;
                    }
                }
                if (token.IsCancellationRequested)
                {
                    throw new OperationCanceledException();
                }
            }
        }

        private void PlayRound(Turn[] turns)
        {
            Round round;
            switch (turns?.FirstOrDefault(x => x != null))
            {
                case GameBeginningTurn turn:
                    round = new GameBeginningRound()
                    {
                        Turns = turns.Where(x => x != null).ToList()
                    };
                    break;
                case GameTurn turn:
                    round = new GameRound()
                    {
                        Turns = turns.Where(x => x != null).ToList()
                    };
                    break;
                default:
                    throw new ArgumentException();
            };
            roundHandler.PlayRound(round);
        }

        public void PauseEvaluation()
        {
            lock (botEvaluationLock)
            {
                cancellationTokenSource.Cancel();
                botHandlers[currentlyEvaluatingIndex]?.StopEvaluation();
            }
        }
    }
}