namespace GameHandlersLib.GameHandlers
{
    using System;
    using System.Collections.Generic;
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

        public async Task StartOrContinueEvaluationAsync()
        {
            var token = cancellationTokenSource.Token;
            do
            {
                await PlayOrContinuePlayingRoundAsync();
                if (token.IsCancellationRequested)
                {
                    throw new OperationCanceledException();
                }
            } while (true);
        }

        /// <summary>
        /// Plays one round. If the round evaluation was
        /// already started, then it continues evaluating that round.
        /// </summary>
        /// <returns></returns>
        public async Task PlayOrContinuePlayingRoundAsync()
        {
            var token = cancellationTokenSource.Token;
            
            // no turn was played so far
            if (currentlyEvaluatingIndex == 0)
            {
                botHandlers = new WarlightAiBotHandler[players.Count];
                turns = new Turn[players.Count];
            }

            // play from index you stopped playing at
            for (int i = currentlyEvaluatingIndex; i < botHandlers.Length; i++)
            {
                var restrictions = objectsRestrictions.ToRestrictions();
                if (players[i].GetType() == typeof(HumanPlayer))
                {
                    botHandlers[i] = new WarlightAiBotHandler(game, (HumanPlayer)players[i],
                        GameBotType.MonteCarloTreeSearchBot, restrictions);
                }
                else
                {
                    botHandlers[i] = new WarlightAiBotHandler(game, (AiPlayer)players[i],
                        restrictions);
                }
                var bestTurn = (await botHandlers[i].FindBestMoveAsync()).ToTurn(game.Map, game.Players);
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