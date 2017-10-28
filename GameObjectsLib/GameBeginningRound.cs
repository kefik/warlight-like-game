namespace GameObjectsLib
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GameMap;
    using ProtoBuf;

    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    public sealed class GameBeginningRound : Round
    {
        public IList<Seize> SelectedRegions { get; } = new List<Seize>();

        public GameBeginningRound(IList<Seize> list)
        {
            SelectedRegions = list ?? throw new ArgumentException();
        }
        
        public GameBeginningRound()
        {
        }

        /// <summary>
        /// Adds seizingPlayer and region to Seized regions list.
        /// </summary>
        /// <param name="seizingPlayer"></param>
        /// <param name="region"></param>
        public void SeizeRegion(Player seizingPlayer, Region region)
        {
            if (SelectedRegions.Any(x => x.Region == region && x.SeizingPlayer == seizingPlayer))
            {
                throw new ArgumentOutOfRangeException(nameof(SelectedRegions), $"The region {region.Name} has already been seized.");
            }
            if (region == null)
            {
                throw new ArgumentException("Region cannot be null.");
            }
            if (SelectedRegions.Count(x => x.SeizingPlayer == seizingPlayer) >= 2)
            {
                throw new ArgumentOutOfRangeException(nameof(SelectedRegions), "Too many regions were selected..");
            }

            SelectedRegions.Add(new Seize(seizingPlayer, region));
        }

        /// <summary>
        /// Resets everything that has been seized.
        /// </summary>
        public override void Reset()
        {
            SelectedRegions.Clear();
        }

        /// <summary>
        ///     Verifies correctness of game beginning rounds.
        /// </summary>
        /// <param name="rounds">Rounds from different players.</param>
        /// <returns>Linearized round.</returns>
        public static GameBeginningRound Process(IList<GameBeginningRound> rounds)
        {
            // verify if two players chose one region, if so, return null
            {
                bool doesCollide = (from round in rounds
                                    select round.SelectedRegions
                                    into regions
                                    from region in regions
                                    group region by region.Region).Any(g => g.Count() > 1);

                if (doesCollide)
                {
                    return null;
                }
            }

            var linearizedRegions = from round in rounds
                                    select round.SelectedRegions
                                    into regions
                                    from region in regions
                                    select region;

            GameBeginningRound newRound = new GameBeginningRound(linearizedRegions.ToList());

            return newRound;
        }

        public override void Validate()
        {
            throw new NotImplementedException();
        }
    }
}