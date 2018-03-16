namespace GameAi.BotStructures.ActionGenerators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Common.Collections;
    using Data.EvaluationStructures;
    using Data.GameRecording;
    using Interfaces.ActionsGenerators;
    using Interfaces.Evaluators.StructureEvaluators;

    internal class SelectRegionEvaluationNode : TreeNode<SelectRegionEvaluationNode, int>
    {
    }

    internal class SelectRegionEvaluationTree : Tree<SelectRegionEvaluationNode, int>
    {
        public IReadOnlyList<BotGameBeginningTurn> ToBotGameBeginningTurns(int playerId)
        {
            var leaves = GetLeaves();

            var turns = new List<BotGameBeginningTurn>();
            foreach (SelectRegionEvaluationNode selectRegionEvaluationNode in leaves)
            {
                var list = new List<int>();
                // get nodes
                SelectRegionEvaluationNode temp = selectRegionEvaluationNode;
                while (!temp.IsRoot)
                {
                    list.Add(temp.Value);

                    temp = temp.Parent;
                }

                turns.Add(new BotGameBeginningTurn(playerId)
                {
                    SeizedRegionsIds = list
                });
            }

            return turns;
        }

        public IList<SelectRegionEvaluationNode> GetLeaves()
        {
            List<SelectRegionEvaluationNode> leaves = new List<SelectRegionEvaluationNode>();
            ForEachPreOrder(x =>
            {
                if (x.IsLeaf)
                {
                    leaves.Add(x);
                }
            });

            return leaves;
        }
    }

    public class SelectRegionActionsGenerator : IGameBeginningActionsGenerator
    {
        private readonly byte playerId;
        private readonly int[] regionsRestrictions;
        private readonly int regionsToChooseCount;
        private readonly Random random;

        private readonly IRegionMinEvaluator regionMinEvaluator;

        public SelectRegionActionsGenerator(IRegionMinEvaluator regionMinEvaluator,
            int regionsToChooseCount, byte playerId = 0, ICollection<int> regionsRestrictions = null)
        {
            // regions that player can choose > regions options count => error
            if (regionsRestrictions == null
                || regionsToChooseCount > regionsRestrictions.Count)
            {
                throw new ArgumentException("Count of regions that player can choose cannot" +
                                            "be lower than number of regions that he can choose.");
            }

            this.regionsToChooseCount = regionsToChooseCount;
            this.regionMinEvaluator = regionMinEvaluator;
            this.regionsRestrictions = regionsRestrictions.OrderBy(x => x).ToArray();
            this.playerId = playerId;
            random = new Random();
        }

        public IReadOnlyList<BotGameBeginningTurn> Generate(PlayerPerspective currentGameState)
        {
            var copiedState = currentGameState.ShallowCopy();
            if (currentGameState.PlayerId != playerId)
            {
                return null;
            }
            return GenerateActions(copiedState);
        }

        private IReadOnlyList<BotGameBeginningTurn> GenerateActions(PlayerPerspective playerPerspective)
        {
            // obtain best regions based on restrictions
            // choose regionsToChooseCount regions recursively, take n best combinations

            SelectRegionEvaluationTree tree = new SelectRegionEvaluationTree()
            {
                Root = new SelectRegionEvaluationNode()
            };

            int combinationsPicked = 0;

            ChooseBestRegions(80, ref combinationsPicked, 0, playerPerspective, tree.Root, new HashSet<double>(), 0);

            var botTurns = tree.ToBotGameBeginningTurns(playerId);

            var correctBotTurns = botTurns.Where(x => x.SeizedRegionsIds.Count == regionsToChooseCount).ToList();

            return correctBotTurns;
        }

        private void ChooseBestRegions(int combinationsToPickCount,
            ref int combinationsPicked,
            int pickedRegionsCount, PlayerPerspective playerPerspective,
            SelectRegionEvaluationNode currentNode,
            HashSet<double> pathsValues,
            double currentPathSum)
        {
            if (pickedRegionsCount >= regionsToChooseCount)
            {
                combinationsPicked++;
                return;
            }

            // sort regions by cost (lesser cost => better pick)
            var sortedRegions =
                regionsRestrictions
                    .Select(x => new
                    {
                        RegionId = x,
                        Value = regionMinEvaluator.GetCost(playerPerspective, playerPerspective.GetRegion(x))
                    })
                    .OrderBy(x => x.Value)
                    .ToList();

            // choose best first option such that wasn't chosen before
            for (int i = 0; i < sortedRegions.Count; i++)
            {
                if (combinationsPicked >= combinationsToPickCount)
                {
                    return;
                }
                double value = sortedRegions[i].Value;

                // sum of the path + new
                double pathSum = value + currentPathSum;

                ref var region = ref playerPerspective.GetRegion(sortedRegions[i].RegionId);
                // the region is not mine => pick it
                if (region.OwnerId != playerId && !pathsValues.Contains(pathSum))
                {
                    var playerPerspectiveCopy = playerPerspective.ShallowCopy();
                    ref var copiedRegion = ref playerPerspectiveCopy.GetRegion(region.Id);
                    copiedRegion.OwnerId = playerId;
                    // add node to the tree
                    currentNode.AddChild(copiedRegion.Id);
                    // cache the current path sum value => mark as visited
                    pathsValues.Add(pathSum);
                    ChooseBestRegions(combinationsToPickCount, ref combinationsPicked,
                        pickedRegionsCount + 1, playerPerspectiveCopy,
                        currentNode.Children.Last(),
                        pathsValues, pathSum);
                }
            }
        }
    }
}