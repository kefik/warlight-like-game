namespace GameObjectsLib.GameRecording
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
                throw new ArgumentException($"The region {region.Name} has already been seized.");
            }
            if (region == null)
            {
                throw new ArgumentNullException(nameof(region));
            }
            if (SelectedRegions.Count(x => x.SeizingPlayer == seizingPlayer) >= 2)
            {
                throw new ArgumentException("Too many regions were selected.");
            }

            SelectedRegions.Add(new Seize(seizingPlayer, region));
        }

        public override void Reset()
        {
            SelectedRegions.Clear();
        }
    }
}