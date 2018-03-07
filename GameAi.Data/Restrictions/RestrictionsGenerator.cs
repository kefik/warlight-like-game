namespace GameAi.Data.Restrictions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Common.Extensions;

    /// <summary>
    /// Generates restrictions for the game.
    /// </summary>
    public class RestrictionsGenerator
    {
        private readonly IList<int> regionsIds;
        private readonly IList<int> playersIds;

        public RestrictionsGenerator(
            IEnumerable<int> regionsIds,
            IEnumerable<int> playersIds)
        {
            this.regionsIds = regionsIds.ToList();
            this.playersIds = playersIds.ToList();
        }

        public Restrictions Generate()
        {
            // generate game beginning restrictions
            var gameBeginningRestrictions = GenerateGameBeginningRestrictions();

            var restrictions = new Restrictions()
            {
                GameBeginningRestrictions = gameBeginningRestrictions
            };
            return restrictions;
        }

        private IList<GameBeginningRestriction> GenerateGameBeginningRestrictions()
        {
            var gameBeginningRestrictions = new List<GameBeginningRestriction>();
            int regionsOfferCount = this.regionsIds.Count / playersIds.Count;
            regionsIds.Shuffle();

            // TODO: not constant, varaible regions to choose based on user preferences
            int regionsToChooseCount = regionsOfferCount > 2 ? 2 : regionsOfferCount;

            var regionsEnumerator = regionsIds.GetEnumerator();

            foreach (int playerId in playersIds)
            {
                var restriction = new GameBeginningRestriction
                {
                    PlayerId = playerId,
                    RegionsPlayerCanChooseCount = regionsToChooseCount
                };

                for (int i = 0; i < regionsOfferCount; i++)
                {
                    if (!regionsEnumerator.MoveNext())
                    {
                        throw new ArgumentException();
                    }

                    restriction.RestrictedRegions
                        .Add(regionsEnumerator.Current);
                }

                gameBeginningRestrictions.Add(restriction);
            }
            return gameBeginningRestrictions;
        }
    }
}