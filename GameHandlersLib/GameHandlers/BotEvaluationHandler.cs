﻿namespace GameHandlersLib.GameHandlers
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

    public class BotEvaluationHandler
    {
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
                var token = cancellationTokenSource.Token;
                do
                {
                    await PlayOrContinuePlayingRoundAsync(timeForBotMove);
                    if (token.IsCancellationRequested)
                    {
                        throw new OperationCanceledException();
                    }
                } while (true);
            }
            catch (GameFinishedException)
            {
                Debug.WriteLine("Game was finished.");
            }
        }

        /// <summary>
        /// Plays one round. If the round evaluation was
        /// already started, then it continues evaluating that round.
        /// </summary>
        /// <returns></returns>
        public async Task PlayOrContinuePlayingRoundAsync(TimeSpan timeForBotMove)
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

            var restrictions = objectsRestrictions.ToRestrictions().ToRemappedRestrictions(playerIdsMapper);

            var mapMin = game.Map.ToMapMin(playerIdsMapper);

            // play from index you stopped playing at
            for (int i = currentlyEvaluatingIndex; i < botHandlers.Length; i++)
            {
                // if the game is finished, don't play more moves
                if (game.IsFinished())
                {
                    throw new GameFinishedException();
                }

                // fixing closure issue
                int currentIndex = i;

                if (!playerIdsMapper.TryGetNewId(players[currentIndex].Id, out int evaluationPlayerId))
                {
                    throw new ArgumentException("Player ID must be correclty mapped in the dictionary");
                }

                if (players[currentIndex].GetType() == typeof(HumanPlayer))
                {
                    botHandlers[currentIndex] = new WarlightAiBotHandler(
                        GameBotType.AggressiveBot,
                        mapMin, Difficulty.Hard, (byte)evaluationPlayerId,
                        game.IsFogOfWar, restrictions);
                }
                else
                {
                    var aiPlayer = (AiPlayer) players[currentIndex];
                    botHandlers[currentIndex] = new WarlightAiBotHandler(
                        aiPlayer.BotType,
                        mapMin, aiPlayer.Difficulty, (byte)evaluationPlayerId,
                        game.IsFogOfWar, restrictions);
                }
                var botTask = Task.Run(botHandlers[currentIndex].FindBestMoveAsync);
                // break after specified amount of time
                //botHandlers[i].StopEvaluation(timeForBotMove);
                var bestTurn = (await botTask).ToTurn(game.Map, game.Players, playerIdsMapper);
                turns[i] = bestTurn;
                
                currentlyEvaluatingIndex++;

                // all bots have returned their turn => play it
                // and quit the evaluation
                if (currentlyEvaluatingIndex >= players.Count)
                {
                    PlayRound(turns);
                    currentlyEvaluatingIndex = 0;
                    break;
                }
                if (token.IsCancellationRequested)
                {
                    throw new OperationCanceledException();
                }
            }
        }

        private void PlayRound(Turn[] turns)
        {
            ILinearizedRound linearizedRound;
            switch (turns?.FirstOrDefault())
            {
                case GameBeginningTurn turn:
                    linearizedRound = new GameBeginningRound()
                    {
                        Turns = turns
                    }.Linearize();
                    break;
                case GameTurn turn:
                    linearizedRound = new GameRound()
                    {
                        Turns = turns
                    }.Linearize();
                    break;
                default:
                    throw new ArgumentException();
            }
            game.AllRounds.Add(linearizedRound);
            roundHandler.PlayRound(linearizedRound);
        }

        public void PauseEvaluation()
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource = new CancellationTokenSource();
            botHandlers[currentlyEvaluatingIndex].StopEvaluation();
        }
    }
}