namespace GameAi.BotStructures.ActionGenerators
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Common.Collections;
    using Data.EvaluationStructures;
    using Data.GameRecording;
    using Data.Restrictions;
    using Interfaces.ActionsGenerators;
    using Interfaces.Evaluators.StructureEvaluators;

    /// <summary>
    /// Its value <see cref="SelectRegionEvaluationNode.Value"/>
    /// represents <seealso cref="RegionMin.Id"/>.
    /// </summary>
    internal class SelectRegionEvaluationNode : TreeNode<SelectRegionEvaluationNode, int>
    {
    }

    /// <summary>
    /// Tree of <see cref="SelectRegionEvaluationNode"/>s.
    /// </summary>
    internal class SelectRegionEvaluationTree : Tree<SelectRegionEvaluationNode, int>
    {
        /// <summary>
        /// Translates <see cref="SelectRegionEvaluationTree"/> into <seealso cref="IReadOnlyList{t}"/>
        /// of <seealso cref="BotGameBeginningTurn"/>s.
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Obtains leaves returning them in left-to-right order.
        /// </summary>
        /// <returns></returns>
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
        private readonly IRegionMinEvaluator regionMinEvaluator;
        private IDictionary<byte, GameBeginningRestriction> restrictions;

        public SelectRegionActionsGenerator(IRegionMinEvaluator regionMinEvaluator,
            ICollection<GameBeginningRestriction> gameBeginningRestrictions)
        {
            restrictions = gameBeginningRestrictions.ToDictionary(x => (byte)x.PlayerId, x => x);

            this.regionMinEvaluator = regionMinEvaluator;
        }

        public IReadOnlyList<BotGameBeginningTurn> Generate(PlayerPerspective currentGameState)
        {
            var copiedState = currentGameState.ShallowCopy();
            return GenerateActions(copiedState);
        }

        /// <summary>
        /// Temp variable for <see cref="GenerateActions"/>
        /// method to be faster.
        /// </summary>
        private byte playerId;

        /// <summary>
        /// Temp variable for <see cref="GenerateActions"/>
        /// method to be faster.
        /// </summary>
        private int regionsToChooseCount;

        /// <summary>
        /// Temp variable for <see cref="GenerateActions"/>
        /// method to be faster.
        /// </summary>
        private ICollection<int> regionsRestrictions;

        private IReadOnlyList<BotGameBeginningTurn> GenerateActions(PlayerPerspective playerPerspective)
        {
            // obtain best regions based on restrictions
            // choose regionsToChooseCount regions recursively, take n best combinations
            var dictionaryEntry = restrictions[playerPerspective.PlayerId];
            playerId = (byte)dictionaryEntry.PlayerId;
            regionsToChooseCount = dictionaryEntry.RegionsPlayerCanChooseCount;
            regionsRestrictions = dictionaryEntry.RestrictedRegions;

            SelectRegionEvaluationTree tree = new SelectRegionEvaluationTree()
            {
                Root = new SelectRegionEvaluationNode()
            };

            int combinationsPicked = 0;

            ChooseBestRegions(3, ref combinationsPicked, 0, playerPerspective, tree.Root, new HashSet<double>(), 0);

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

            // sort regions by value (greater value => better)
            var sortedRegions =
                regionsRestrictions
                    .Select(x => new
                    {
                        RegionId = x,
                        Value = regionMinEvaluator.GetValue(playerPerspective, playerPerspective.GetRegion(x))
                    })
                    .OrderByDescending(x => x.Value)
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
                // the region is not mine and I wasn't at this situation on the branch => pick it
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