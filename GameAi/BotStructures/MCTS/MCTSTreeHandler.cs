namespace GameAi.BotStructures.MCTS
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Interfaces;
    using Data.EvaluationStructures;
    using Data.GameRecording;
    using Interfaces;
    using Interfaces.ActionsGenerators;
    using Interfaces.Evaluators;
    using Interfaces.Evaluators.NodeEvaluators;
    using Interfaces.Evaluators.StructureEvaluators;
    using StructuresEvaluators;

    /// <summary>
    /// Handles single-threaded MCTS evaluation.
    /// </summary>
    internal class MCTSTreeHandler : IRandomInjectable
    {
        private readonly INodeEvaluator<MCTSTreeNode> nodeEvaluator;
        private readonly IGameActionsGenerator gameActionsGenerator;
        private readonly IGameBeginningActionsGenerator beginningActionsGenerator;
        private readonly IPlayerPerspectiveEvaluator gamePlayerPerspectiveEvaluator;

        private readonly ProbabilityAwareRoundEvaluator probabilityAwareRoundEvaluator;

        private readonly byte myPlayerId;
        private readonly byte enemyPlayerId;

        private Random random;

        /// <summary>
        /// Tree representing the evaluation.
        /// Root of this tree represents player perspective
        /// from which the best move is found.
        /// </summary>
        public MCTSTree Tree { get; }

        public MCTSTreeHandler(PlayerPerspective initialBoardState,
            byte enemyPlayerId,
            IGameActionsGenerator gameActionsGenerator,
            IGameBeginningActionsGenerator beginningActionsGenerator,
            IPlayerPerspectiveEvaluator gamePlayerPerspectiveEvaluator,
            IRoundEvaluator roundEvaluator)
        {
            myPlayerId = initialBoardState.PlayerId;
            this.enemyPlayerId = enemyPlayerId;

            this.random = new Random();

            this.probabilityAwareRoundEvaluator = new ProbabilityAwareRoundEvaluator(roundEvaluator,
                gamePlayerPerspectiveEvaluator, myPlayerId, enemyPlayerId);

            var state = new NodeState()
            {
                BoardState = initialBoardState.MapMin,
                BotTurn = new BotGameBeginningTurn(myPlayerId),
                VisitCount = 0,
                WinCount = 0
            };

            Tree = new MCTSTree(state);

            this.nodeEvaluator = new UctEvaluator(Tree.Root);
            this.gameActionsGenerator = gameActionsGenerator;
            this.beginningActionsGenerator = beginningActionsGenerator;
            this.gamePlayerPerspectiveEvaluator = gamePlayerPerspectiveEvaluator;
        }

        /// <summary>
        /// Starts MCTS evaluation with possible cancellation.
        /// </summary>
        /// <param name="token"></param>
        public void StartEvaluation(CancellationToken token)
        {
#if IGNORE_CANCELLATION
            while (true)
#else
            while (!token.IsCancellationRequested)
#endif
            {
                var bestNode = SelectBestNode();

                var nodesToSimulate = Expand(bestNode);

                foreach (var nodeToSimulate in nodesToSimulate)
                {
                    // cancellation requested => finish the evaluation
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }

                    Simulate(nodeToSimulate);

                    Backpropagate(nodeToSimulate);
                }
            }
        }

        /// <summary>
        /// Selects the best node.
        /// </summary>
        /// <returns></returns>
        private MCTSTreeNode SelectBestNode()
        {
            MCTSTreeNode currentNode = Tree.Root;

            while (!currentNode.IsLeaf)
            {
                currentNode = GetBestChild(currentNode);
            }

            return currentNode;
        }

        private MCTSTreeNode GetBestChild(MCTSTreeNode node)
        {
            var children = node.Children;

            MCTSTreeNode bestChild = children[0];
            double bestChildValue = nodeEvaluator.GetValue(bestChild);

            // get the node with best rating
            for (int index = 1; index < children.Count; index++)
            {
                var child = children[index];
                double childValue = nodeEvaluator.GetValue(child);

                if (childValue > bestChildValue)
                {
                    bestChild = child;
                    bestChildValue = childValue;
                }
            }

            return bestChild;
        }

        /// <summary>
        /// Expand the node specified in parameter meaning choose next actions that will be simulated.
        /// </summary>
        /// <param name="node">Node to expand.</param>
        /// <returns>New children <see cref="node"/> was expanded to.</returns>
        private IList<MCTSTreeNode> Expand(MCTSTreeNode node)
        {
            var boardState = node.Value.BoardState;
            
            PlayerPerspective myPlayerPerspective = new PlayerPerspective(boardState, myPlayerId);
            PlayerPerspective enemyPlayerPerspective = new PlayerPerspective(boardState, enemyPlayerId);

            bool isGameBeginning = boardState.IsGameBeginning();

            IReadOnlyList<BotTurn> myActions;
            IReadOnlyList<BotTurn> enemyActions;
            if (isGameBeginning)
            {
                myActions = beginningActionsGenerator.Generate(myPlayerPerspective);
                enemyActions = beginningActionsGenerator.Generate(enemyPlayerPerspective);
            }
            else
            {
                myActions = gameActionsGenerator.Generate(myPlayerPerspective);
                enemyActions = gameActionsGenerator.Generate(enemyPlayerPerspective);
            }

            // create my nodes
            foreach (BotTurn myAction in myActions)
            {
                node.AddChild(new NodeState()
                {
                    BotTurn = myAction,
                    BoardState = boardState
                });
            }

            // create enemy nodes with result
            foreach (var child in node.Children)
            {
                var myAction = child.Action;
                var round = new BotRound()
                {
                    BotTurns = new List<BotTurn>()
                    {
                        myAction
                    }
                };
                foreach (BotTurn enemyAction in enemyActions)
                {
                    round.BotTurns.Add(enemyAction);
                    if (!isGameBeginning)
                    {
                        var (expected, worstCase) = probabilityAwareRoundEvaluator.EvaluateInExpectedAndWorstCase(boardState, round);
                        child.AddChild(new NodeState()
                        {
                            BotTurn = enemyAction,
                            BoardState = expected
                        });
                    }
                    else
                    {
                        var resultMap = probabilityAwareRoundEvaluator.EvaluateInRandomCase(boardState, round);
                        child.AddChild(new NodeState()
                        {
                            BotTurn = enemyAction,
                            BoardState = resultMap
                        });
                    }

                    round.BotTurns.RemoveAt(round.BotTurns.Count - 1);
                }
            }

            return node.Children.SelectMany(x => x.Children).ToList();
        }

        /// <summary>
        /// Play a simulation from specified node.
        /// </summary>
        /// <param name="sourceNode"></param>
        private void Simulate(MCTSTreeNode sourceNode)
        {
            // TODO: playout based on option moves
            MapMin boardState = sourceNode.GameState;

            PlayerPerspective myPlayerPerspective = new PlayerPerspective(boardState, myPlayerId);
            PlayerPerspective enemyPlayerPerspective = new PlayerPerspective(boardState, enemyPlayerId);

            for (int i = 0; i < 20; i++)
            {
                if (myPlayerPerspective.HasLost())
                {
                    sourceNode.Value.WinCount = 1;
                    sourceNode.Value.VisitCount = 1;
                    return;
                }
                if (enemyPlayerPerspective.HasLost())
                {
                    sourceNode.Value.WinCount = 0;
                    sourceNode.Value.VisitCount = 1;
                    return;
                }

                var myActions = gameActionsGenerator.Generate(myPlayerPerspective);
                var enemyActions = gameActionsGenerator.Generate(enemyPlayerPerspective);

                var randomlySelectedMyAction = myActions[random.Next(myActions.Count)];
                var randomlySelectedEnemyAction = enemyActions[random.Next(enemyActions.Count)];

                var round = new BotRound()
                {
                    BotTurns = new BotTurn[] { randomlySelectedEnemyAction, randomlySelectedMyAction }
                };
                (MapMin expected, MapMin worstCase) = probabilityAwareRoundEvaluator.EvaluateInExpectedAndWorstCase(boardState, round);

                boardState = worstCase;
                myPlayerPerspective = new PlayerPerspective(boardState, myPlayerId);
                enemyPlayerPerspective = new PlayerPerspective(boardState, enemyPlayerId);
            }

            double myValue = gamePlayerPerspectiveEvaluator.GetValue(myPlayerPerspective);
            double enemyValue = gamePlayerPerspectiveEvaluator.GetValue(enemyPlayerPerspective);

            double normalizedEnemyValue = enemyValue / (myValue + enemyValue);
            Debug.WriteLine($"Normalized enemy value: {normalizedEnemyValue:F1}");

            sourceNode.Value.WinCount = normalizedEnemyValue;
            sourceNode.Value.VisitCount = 1;
        }

        /// <summary>
        /// Backpropagates the information about results
        /// from newly expanded node back to root.
        /// </summary>
        /// <param name="newlyExpandedNode"></param>
        private void Backpropagate(MCTSTreeNode newlyExpandedNode)
        {
            byte playerId = (byte)newlyExpandedNode.Action.PlayerId;
            double valueToBackPropagate = newlyExpandedNode.WinCount;

            MCTSTreeNode currentNode = newlyExpandedNode;
            MCTSTreeNode parent = currentNode.Parent;
            // repeat until the parent node is root node
            while (!currentNode.IsRoot)
            {
                // update the node
                if (parent.Action.PlayerId != playerId)
                {
                    parent.Value.WinCount += valueToBackPropagate;
                }
                else
                {
                    parent.Value.WinCount += 1 - valueToBackPropagate;
                }

                parent.Value.VisitCount++;

                currentNode = currentNode.Parent;
                parent = currentNode.Parent;
            }
        }

        void IRandomInjectable.Inject(Random random)
        {
            this.random = random;
        }
    }
}