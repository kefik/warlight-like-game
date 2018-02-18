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
    /// Minimized version of <see cref="Region"/>. It's purpose is
    /// to be smaller and faster than <seealso cref="Region"/> class for
    /// evaluation.
    /// </summary>
    /// <remarks>
    /// Must be class and not struct, because of neighbours.
    /// </remarks>
    public struct RegionMin
    {
        private class RegionMinStatic
        {
            public int Id { get; }
            public int[] NeighboursIds { get; set; }
            public SuperRegionMin SuperRegion { get; set; }
            public bool IsWasteland { get; }

            public RegionMinStatic(Region region, Player ownerEncoded)
            {
                Id = region.Id;
                SuperRegion = new SuperRegionMin(region.SuperRegion, ownerEncoded);
            }

            public RegionMinStatic(int regionId, SuperRegionMin superRegion, bool isWasteland = false)
            {
                Id = regionId;
                SuperRegion = superRegion;
                IsWasteland = isWasteland;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool IsNeighbourOf(RegionMin region)
            {
                foreach (var neighbourId in NeighboursIds)
                {
                    if (neighbourId == region.Static.Id)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        /// <summary>
        /// First 5 bits = owner, 1 visibility, 10 bits = army
        /// </summary>
        private ushort ownerAndArmyEncoded;

        private RegionMinStatic Static { get; }
        
        /// <summary>
        /// Represents the owner from players perspective specified in constructor.
        /// </summary>
        /// <remarks>
        /// First 5 bits of <see cref="ownerAndArmyEncoded"/>
        /// are used for owner identification.
        /// </remarks>
        public byte OwnerEncoded
        {
            get { return (byte) (ownerAndArmyEncoded & 0b11111); }
            set
            {
                ownerAndArmyEncoded &= 0b1111111111100000; // 5 zeros, 11 ones
                value &= 0b11111;
                ownerAndArmyEncoded |= value;
            }
        }

        /// <summary>
        /// Id of the instance.
        /// </summary>
        public int Id
        {
            get { return Static.Id; }
        }

        /// <summary>
        /// Reports whether the region is wasteland (more armies at the game beginning in it).
        /// </summary>
        public bool IsWasteland
        {
            get { return Static.IsWasteland; }
        }

        /// <summary>
        /// Represents size of army of the region.
        /// </summary>
        /// <remarks>10 upper bits from <see cref="ownerAndArmyEncoded"/>
        /// are used for army.</remarks>
        public int Army
        {
            get { return ownerAndArmyEncoded >> 6; }
            set
            {
                ushort armyValue = (ushort) value;

                // army mask = 10x 0, 6x 1
                ushort armyMask = 0b0000000000111111;

                // reset army bits
                ownerAndArmyEncoded &= armyMask;

                ownerAndArmyEncoded |= (ushort)(armyValue << 6);
            }
        }
        
        /// <summary>
        /// True, if the region is hidden by Fog of war.
        /// </summary>
        /// <remarks>
        /// 6-th bit of <see cref="ownerAndArmyEncoded"/> is used for
        /// IsVisible boolean.
        /// </remarks>
        public unsafe bool IsVisible
        {
            get { return (ownerAndArmyEncoded & 0b100000) != 0; }
            set
            {
                // get byte value (faster than if)
                byte byteValue = *((byte*) (&value));

                ushort shiftedValue = (ushort)(byteValue << 5);
                ownerAndArmyEncoded = (ushort)(shiftedValue | ownerAndArmyEncoded);
            }
        }

        /// <summary>
        /// SuperRegion in which this region is contained.
        /// </summary>
        public SuperRegionMin SuperRegion
        {
            get { return Static.SuperRegion; }
            internal set { Static.SuperRegion = value; }
        }

        /// <summary>
        /// Array of neighbour regions of this given region.
        /// </summary>
        public int[] NeighbourRegionsIds
        {
            get { return Static.NeighboursIds; }
            set { Static.NeighboursIds = value; }
        } 
        
        internal RegionMin(Region region, byte ownerEncoded, Player playerPerspective)
        {
            ownerAndArmyEncoded = 0;
            Static = new RegionMinStatic(region, playerPerspective);

            Army = region.Army;
            OwnerEncoded = region.Owner == null ? (byte)0 : ownerEncoded;
        }

        public RegionMin(int regionId, SuperRegionMin superRegion, int army, bool isWasteland = false)
        {
            ownerAndArmyEncoded = 0;
            Static = new RegionMinStatic(regionId, superRegion, isWasteland);

            Army = army;
        }

        /// <summary>
        /// Gets OwnerPerspective of this region based on playerPerspective passed in parameter.
        /// </summary>
        /// <param name="playerPerspective"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public OwnerPerspective GetOwnerPerspective(byte playerPerspective)
        {
            if (OwnerEncoded == 0)
            {
                return (byte)OwnerPerspective.Unoccupied;
            }

            return playerPerspective == OwnerEncoded ? OwnerPerspective.Mine : OwnerPerspective.Enemy;
        }
        
        /// <summary>
        /// True, if the region is neighbour to this instance of the region.
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsNeighbourOf(RegionMin region)
        {
            return Static.IsNeighbourOf(region);
        }
    }
}