namespace GameAi
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
    public class SuperRegionMin : IRefreshable
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
        public OwnerState Owner { get; internal set; }

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
            internal set { Static.Regions = value; }
        }

        internal SuperRegionMin(SuperRegion superRegion, Player playerPerspective)
        {
            if (superRegion.Owner == null)
            {
                Owner = OwnerState.Unoccupied;
            }
            else if (superRegion.Owner == playerPerspective)
            {
                Owner = OwnerState.Mine;
            }
            else
            {
                Owner = OwnerState.Enemy;
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
                    case OwnerState.Unoccupied:
                        unoccupiedCount++;
                        break;
                    case OwnerState.Enemy:
                        enemyCount++;
                        break;
                    case OwnerState.Mine:
                        mineCount++;
                        break;
                }
            }
            if (unoccupiedCount != 0 || (mineCount != 0 && enemyCount != 0))
            {
                Owner = OwnerState.Unoccupied;
            }
            else if (mineCount != 0 && unoccupiedCount == 0 && enemyCount == 0)
            {
                Owner = OwnerState.Mine;
            }
            else if (mineCount == 0 && unoccupiedCount == 0 && enemyCount != 0)
            {
                Owner = OwnerState.Enemy;
            }
        }
        
        /// <summary>
        /// Shallow-copies this instance.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal SuperRegionMin ShallowCopy()
        {
            return (SuperRegionMin)MemberwiseClone();
        }
    }
}