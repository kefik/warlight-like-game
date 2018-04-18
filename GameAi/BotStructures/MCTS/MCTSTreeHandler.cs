#if DEBUG
//#define LOG_TREEHANDLER
//#define IGNORE_CANCELLATION
#endif

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
            IRoundEvaluator roundEvaluator,
            Random random)
        {
            myPlayerId = initialBoardState.PlayerId;
            this.enemyPlayerId = enemyPlayerId;

            this.random = random;

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
#if LOG_TREEHANDLER
            Debug.WriteLine($"EVALUATION STARTED (player {myPlayerId})");
            Debug.WriteLine("-------------------");

            try
            {
#endif
#if IGNORE_CANCELLATION
            while (true)
#else
            do
#endif
            {
                var bestNode = SelectBestNode();

                MCTSTreeNode nodeToSimulate;

                // node selected hasn't been visit yet and it is not root => 
                // it must be node that hasn't been simulated yet
                if (bestNode.VisitCount == 0 && !bestNode.IsRoot)
                {
                    nodeToSimulate = bestNode;
                }
                else
                {
                    var nodesToSimulate = Expand(bestNode);

                    // i lost
                    if (nodesToSimulate == null)
                    {
                        Backpropagate(bestNode, 1);

                        continue;
                    }
                    // enemy lost
                    if (nodesToSimulate.Count == 0)
                    {
                        Backpropagate(bestNode, 0);

                        continue;
                    }

                    nodeToSimulate =
                        nodesToSimulate[
                            random.Next(nodesToSimulate.Count)];
                }

                double resultValue = Simulate(nodeToSimulate);

                Backpropagate(nodeToSimulate, resultValue);
            } while (!token.IsCancellationRequested);

#if LOG_TREEHANDLER
            }
            finally
            {
                Debug.WriteLine("EVALUATION ENDED");
                Debug.WriteLine("BEST PATH");
                foreach (var mctsTreeNode in Tree.Root.Children
                    .OrderByDescending(x => x.VisitCount)
                    .ThenByDescending(x => x.WinCount))
                {
                    var gameState = mctsTreeNode.GameState;
                    Debug.WriteLine($"{mctsTreeNode.WinCount:F}/{mctsTreeNode.VisitCount}");
                    Debug.WriteLine($"ACTIONS:");

                    switch (mctsTreeNode.Action)
                    {
                        case BotGameBeginningTurn gameBeginningTurn:
                            Debug.WriteLine($"Seizes:");
                            foreach (int seizedRegionsId in gameBeginningTurn.SeizedRegionsIds)
                            {
                                Debug.WriteLine($"{gameState.GetRegion(seizedRegionsId).Name}");
                            }
                            break;
                        case BotGameTurn gameTurn:
                            var deployments = gameTurn.Deployments;
                            Debug.WriteLine("Deployments:");
                            foreach (BotDeployment botDeployment in deployments)
                            {
                                var region =
gameState.GetRegion(botDeployment.RegionId);
                                Debug.WriteLine($"Region: {region.Name}, New army: {botDeployment.Army}");
                            }

                            var attacks = gameTurn.Attacks;
                            Debug.WriteLine("Attacks:");
                            foreach (BotAttack botAttack in attacks)
                            {
                                var attackingRegion =
gameState.GetRegion(botAttack.AttackingRegionId);
                                var defendingRegion =
gameState.GetRegion(botAttack.DefendingRegionId);
                                Debug.WriteLine(
                                    $"{attackingRegion.Name} -> {defendingRegion.Name}, Army: {botAttack.AttackingArmy}");
                            }
                            break;
                    }
                    Debug.WriteLine("");
                }
                Debug.WriteLine("");
                Debug.WriteLine("");
            }
#endif
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

            // get the child node with best rating
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
            else if (myPlayerPerspective.HasLost())
            {
                return null;
            }
            else if (enemyPlayerPerspective.HasLost())
            {
                return new List<MCTSTreeNode>();
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

                        MapMin newBoardState =
                            DidMySuperRegionChangeOwner(expected.SuperRegionsMin,
                                worstCase.SuperRegionsMin)
                                ? worstCase
                                : expected;
                        child.AddChild(new NodeState()
                        {
                            BotTurn = enemyAction,
                            BoardState = newBoardState
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

            // return enemy all enemy nodes
            return node.Children.SelectMany(x => x.Children).ToList();
        }

        /// <summary>
        /// Play a simulation from specified node.
        /// </summary>
        /// <param name="newlyExpandedNode"></param>
        private double Simulate(MCTSTreeNode newlyExpandedNode)
        {
            // TODO: playout based on option moves
            MapMin boardState = newlyExpandedNode.GameState;

            PlayerPerspective myPlayerPerspective = new PlayerPerspective(boardState, myPlayerId);
            PlayerPerspective enemyPlayerPerspective = new PlayerPerspective(boardState, enemyPlayerId);

            for (int i = 0; i < 10; i++)
            {
                if (myPlayerPerspective.HasLost())
                {
                    return 1;
                }
                if (enemyPlayerPerspective.HasLost())
                {
                    return 0;
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

                boardState = expected;

                var expectedCaseSuperRegions = expected.SuperRegionsMin;
                var worstCaseSuperRegions = worstCase.SuperRegionsMin;
                if (DidMySuperRegionChangeOwner(expectedCaseSuperRegions,
                    worstCaseSuperRegions))
                {
                    boardState = worstCase;
                }
                myPlayerPerspective = new PlayerPerspective(boardState, myPlayerId);
                enemyPlayerPerspective = new PlayerPerspective(boardState, enemyPlayerId);
            }

            double myValue = gamePlayerPerspectiveEvaluator.GetValue(myPlayerPerspective);
            double enemyValue = gamePlayerPerspectiveEvaluator.GetValue(enemyPlayerPerspective);

            double normalizedEnemyValue = enemyValue / (myValue + enemyValue);

            return normalizedEnemyValue;
        }

        /// <summary>
        /// Backpropagate the <see cref="enemyPerspectiveValue"/> from <see cref="sourceNode"/>
        /// back to root.
        /// </summary>
        /// <param name="sourceNode"></param>
        /// <param name="enemyPerspectiveValue"></param>
        private void Backpropagate(MCTSTreeNode sourceNode,
            double enemyPerspectiveValue)
        {
            MCTSTreeNode currentNode = sourceNode;
            // repeat until the parent node is root node
            while (currentNode != null)
            {
                // update the node
                if (currentNode.Action.PlayerId != myPlayerId)
                {
                    currentNode.Value.WinCount += enemyPerspectiveValue;
                }
                else
                {
                    currentNode.Value.WinCount += 1 - enemyPerspectiveValue;
                }

                currentNode.Value.VisitCount++;

                currentNode = currentNode.Parent;
            }
        }

        private bool DidMySuperRegionChangeOwner(
            SuperRegionMin[] expected, SuperRegionMin[] worstCase)
        {
            for (int j = 0; j < expected.Length; j++)
            {
                if (expected[j].OwnerId == myPlayerId &&
                    worstCase[j].OwnerId != myPlayerId)
                {
                    return true;
                }
            }

            return false;
        }

        void IRandomInjectable.Inject(Random random)
        {
            this.random = random;
        }
    }
}