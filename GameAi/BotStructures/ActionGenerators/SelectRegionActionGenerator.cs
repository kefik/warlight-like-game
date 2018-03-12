namespace GameAi.BotStructures.ActionGenerators
{
    using System;
    using System.Collections.Generic;
    using Data.EvaluationStructures;
    using Data.GameRecording;
    using Interfaces;

    /// <summary>
    /// Represents action generator for selecting regions at the beginning of the game.
    /// </summary>
    internal class SelectRegionActionGenerator : IGameActionGenerator<BotGameBeginningTurn, PlayerPerspective>
    {
        private readonly int playerId;
        private readonly ICollection<int> regionsRestrictions;
        private readonly int regionsToChooseCount;
        private readonly Random random;

        public SelectRegionActionGenerator(int regionsToChooseCount, int playerId = 0, ICollection<int> regionsRestrictions = null)
        {
            // regions that player can choose > regions options count => error
            if (regionsRestrictions != null
                && regionsToChooseCount > regionsRestrictions.Count)
            {
                throw new ArgumentException("Count of regions that player can choose cannot" +
                                            "be lower than number of regions that he can choose.");
            }

            this.regionsToChooseCount = regionsToChooseCount;
            this.regionsRestrictions = regionsRestrictions;
            this.playerId = playerId;
            random = new Random();
        }

        private bool HasAnyRestrictions()
        {
            return regionsRestrictions != null && regionsRestrictions.Count != 0;
        }

        public BotGameBeginningTurn Generate(PlayerPerspective currentGameState)
        {
            if (currentGameState.PlayerId == playerId
                && HasAnyRestrictions())
            {
                return GenerateWhenRestrictions(currentGameState);
            }

            return GenerateWhenNoRestrictions(currentGameState);
        }

        private BotGameBeginningTurn GenerateWhenRestrictions(PlayerPerspective playerPerspective)
        {
            BotGameBeginningTurn gameBeginningTurn = new BotGameBeginningTurn(playerPerspective.PlayerId);

            RegionMin[] regionsMin = new RegionMin[regionsRestrictions.Count];

            // get regions that player can choose for beginning of the game
            int index = 0;
            {
                foreach (int regionsRestriction in regionsRestrictions)
                {
                    regionsMin[index++] = playerPerspective.MapMin.GetRegion(regionsRestriction);
                }
            }
            // indices of regions that were chosen by the algorithm
            var chosenIndices = new HashSet<int>();

            // TODO: generate properly starting regions
            index = 0;
            do
            {
                int regionToChooseIndex = random.Next(0, regionsRestrictions.Count);

                if (!chosenIndices.Contains(regionsMin[regionToChooseIndex].Id))
                {
                    chosenIndices.Add(regionsMin[regionToChooseIndex].Id);
                    index++;
                }
            } while (index < regionsToChooseCount);

            gameBeginningTurn.SeizedRegionsIds = chosenIndices;

            return gameBeginningTurn;
        }

        private BotGameBeginningTurn GenerateWhenNoRestrictions(PlayerPerspective playerPerspective)
        {
            throw new NotImplementedException();
        }
    }
}