﻿namespace GameAi
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using GameObjectsLib;
    using GameObjectsLib.GameMap;

    /// <summary>
    /// Represents minimized version of SuperRegion class for the purpose of calculation
    /// </summary>
    internal class SuperRegionMin : IRefreshable
    {
        private class SuperRegionMinStatic
        {
            public int Id { get; }
            public RegionMin[] Regions { get; set; }
            public int Bonus { get; }

            public SuperRegionMinStatic(SuperRegion superRegion)
            {
                Id = superRegion.Id;
                Bonus = superRegion.Bonus;
            }
        }

        private SuperRegionMinStatic Static { get; }

        public int Id
        {
            get { return Static.Id; }
        }
        /// <summary>
        /// Represents owner of this region.
        /// </summary>
        public OwnershipState Owner { get; set; }

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
        public RegionMin[] Regions
        {
            get { return Static.Regions; }
            set { Static.Regions = value; }
        }

        public SuperRegionMin(SuperRegion superRegion, Player playerPerspective)
        {
            if (superRegion.Owner == null)
            {
                Owner = OwnershipState.Unoccupied;
            }
            else if (superRegion.Owner == playerPerspective)
            {
                Owner = OwnershipState.Mine;
            }
            else
            {
                Owner = OwnershipState.Enemy;
            }

            Static = new SuperRegionMinStatic(superRegion);
        }

        /// <summary>
        /// Refreshes SuperRegion, deciding whos the new owner.
        /// </summary>
        public void Refresh()
        {
            int unoccupiedCount = 0,
                mineCount = 0,
                enemyCount = 0;
            foreach (var region in Static.Regions)
            {
                switch (region.Owner)
                {
                    case OwnershipState.Unoccupied:
                        unoccupiedCount++;
                        break;
                    case OwnershipState.Enemy:
                        enemyCount++;
                        break;
                    case OwnershipState.Mine:
                        mineCount++;
                        break;
                }
            }
            if (unoccupiedCount != 0 || (mineCount != 0 && enemyCount != 0))
            {
                Owner = OwnershipState.Unoccupied;
            }
            else if (mineCount != 0 && unoccupiedCount == 0 && enemyCount == 0)
            {
                Owner = OwnershipState.Mine;
            }
            else if (mineCount == 0 && unoccupiedCount == 0 && enemyCount != 0)
            {
                Owner = OwnershipState.Enemy;
            }
        }
        
        /// <summary>
        /// Shallow-copies this instance.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SuperRegionMin ShallowCopy()
        {
            return (SuperRegionMin)MemberwiseClone();
        }
    }
}