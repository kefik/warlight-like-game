namespace GameHandlersLib.GameHandlers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using GameAi;
    using GameAi.Data;
    using GameAi.Data.Restrictions;
    using GameObjectsLib.Game;
    using GameObjectsLib.Players;

    public class BotEvaluationHandler
    {
        private IList<Player> players;
        private Game game;
        private Restrictions restrictions;

        private WarlightAiBotHandler[] botHandlers;
        private int currentlyEvaluatingIndex;

        private CancellationTokenSource cancellationTokenSource;

        public BotEvaluationHandler(Game game, IList<Player> players, Restrictions restrictions)
        {
            this.game = game;
            this.players = players;
            this.restrictions = restrictions;

            cancellationTokenSource = new CancellationTokenSource();
        }

        public async Task StartOrContinueEvaluationAsync()
        {
            var token = cancellationTokenSource.Token;
            do
            {
                if (currentlyEvaluatingIndex == 0)
                {
                    botHandlers = new WarlightAiBotHandler[players.Count];
                }

                for (int i = currentlyEvaluatingIndex; i < botHandlers.Length; i++)
                {
                    if (players[i].GetType() == typeof(HumanPlayer))
                    {
                        botHandlers[i] = new WarlightAiBotHandler(game, (HumanPlayer) players[i],
                            GameBotType.MonteCarloTreeSearchBot, null);
                    }
                    else
                    {
                        botHandlers[i] = new WarlightAiBotHandler(game, (AiPlayer)players[i],
                            null);
                    }
                    var bestTurn = await botHandlers[i].FindBestMoveAsync();
                    // TODO: store best turn
                    currentlyEvaluatingIndex++;

                    if (token.IsCancellationRequested)
                    {
                        throw new OperationCanceledException();
                    }
                }

                currentlyEvaluatingIndex = 0;
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

            // play from index you stopped playing at
            for (int i = currentlyEvaluatingIndex; i < botHandlers.Length; i++)
            {
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
                var bestTurn = await botHandlers[i].FindBestMoveAsync();
                // TODO: store best turn

                // current index must not be >= number of bots
                currentlyEvaluatingIndex = ++currentlyEvaluatingIndex % botHandlers.Length;

                if (token.IsCancellationRequested)
                {
                    throw new OperationCanceledException();
                }
            }
        }

        public void PauseEvaluation()
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource = new CancellationTokenSource();
            botHandlers[currentlyEvaluatingIndex].StopEvaluation();
        }
    }
}