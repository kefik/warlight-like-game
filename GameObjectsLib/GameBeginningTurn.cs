namespace GameObjectsLib
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GameMap;
    using Players;
    using ProtoBuf;

    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    public class GameBeginningTurn : Turn
    {
        public IList<Seize> SelectedRegions { get; } = new List<Seize>();

        public GameBeginningTurn(IList<Seize> list, Player playerOnTurn) : base(playerOnTurn)
        {
            SelectedRegions = list ?? throw new ArgumentException();
        }

        public GameBeginningTurn(Player playerOnTurn) : base(playerOnTurn)
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
                throw new ArgumentOutOfRangeException(nameof(SelectedRegions),
                    $"The region {region.Name} has already been seized.");
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

        public override void Reset()
        {
            throw new System.NotImplementedException();
        }
    }
}