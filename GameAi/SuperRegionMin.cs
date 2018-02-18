namespace GameAi
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using GameObjectsLib;
    using GameObjectsLib.GameMap;
    using GameObjectsLib.Players;

    /// <summary>
    /// Represents minimized version of SuperRegion class for the purpose of calculation
    /// </summary>
    public struct SuperRegionMin
    {
        private class SuperRegionMinStatic
        {
            public int Id { get; }
            public int[] RegionsIds { get; set; }
            public int Bonus { get; }

            public SuperRegionMinStatic(SuperRegion superRegion)
            {
                Id = superRegion.Id;
                Bonus = superRegion.Bonus;
            }

            public SuperRegionMinStatic(int id, int bonus)
            {
                Id = id;
                Bonus = bonus;
            }
        }

        private SuperRegionMinStatic Static { get; }

        /// <summary>
        /// Id of this super region.
        /// </summary>
        public int Id
        {
            get { return Static.Id; }
        }

        /// <summary>
        /// Represents owner of this region.
        /// </summary>
        public OwnerPerspective OwnerPerspective { get; set; }

        /// <summary>
        /// Represents bonus given to the owner of this region per round.
        /// </summary>
        public int Bonus
        {
            get { return Static.Bonus; }
        }

        /// <summary>
        /// Represents regions that belong to the given SuperRegion.
        /// </summary>
        public int[] RegionsIds
        {
            get { return Static.RegionsIds; }
            set { Static.RegionsIds = value; }
        }

        internal SuperRegionMin(SuperRegion superRegion, Player playerPerspective)
        {
            if (superRegion.Owner == null)
            {
                OwnerPerspective = OwnerPerspective.Unoccupied;
            }
            else if (superRegion.Owner == playerPerspective)
            {
                OwnerPerspective = OwnerPerspective.Mine;
            }
            else
            {
                OwnerPerspective = OwnerPerspective.Enemy;
            }

            Static = new SuperRegionMinStatic(superRegion);
        }

        public SuperRegionMin(int superRegionId, int bonusArmy)
        {
            Static = new SuperRegionMinStatic(superRegionId, bonusArmy);

            OwnerPerspective = OwnerPerspective.Unoccupied;
        }
    }
}