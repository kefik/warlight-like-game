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

        public GameBeginningRound(List<Seize> list)
        {
            SelectedRegions = list;
        }

        public GameBeginningRound()
        {
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

    [ProtoContract]
    public struct Seize
    {
        [ProtoMember(1, AsReference = true)]
        public Player SeizingPlayer { get; }

        [ProtoMember(2, AsReference = true)]
        public Region Region { get; }

        public Seize(Player player, Region region)
        {
            SeizingPlayer = player;
            Region = region;
        }
    }
}