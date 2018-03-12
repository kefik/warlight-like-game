namespace GameAi.Data.EvaluationStructures
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Minimized version of <see cref="GameObjectsLib.GameMap.Region"/>. It's purpose is
    /// to be smaller and faster than <seealso cref="GameObjectsLib.GameMap.Region"/> class for
    /// evaluation.
    /// </summary>
    /// <remarks>
    /// Must be class and not struct, because of neighbours.
    /// </remarks>
    public struct RegionMin
    {
        private class RegionMinStatic
        {
            public int Id { get; internal set; }
            public int[] NeighboursIds { get; set; }
            public int SuperRegionId { get; set; }
            public bool IsWasteland { get; }

            public RegionMinStatic(int regionId, int superRegionId, bool isWasteland = false)
            {
                Id = regionId;
                SuperRegionId = superRegionId;
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
        public byte OwnerId
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
            set { Static.Id = value; }
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
            get { return (ownerAndArmyEncoded & 0b10_0000) != 0; }
            set
            {
                // get byte value (faster than if)
                byte byteValue = *((byte*) (&value));
                
                // reset is visible bit
                ushort isVisibleMask = 0b1111_1111_1101_1111;
                ownerAndArmyEncoded &= isVisibleMask;

                ushort shiftedValue = (ushort)(byteValue << 5);
                ownerAndArmyEncoded = (ushort)(shiftedValue | ownerAndArmyEncoded);
            }
        }

        /// <summary>
        /// SuperRegion in which this region is contained.
        /// </summary>
        public int SuperRegionId
        {
            get { return Static.SuperRegionId; }
            set { Static.SuperRegionId = value; }
        }

        /// <summary>
        /// Array of neighbour regions of this given region.
        /// </summary>
        public int[] NeighbourRegionsIds
        {
            get { return Static.NeighboursIds; }
            set { Static.NeighboursIds = value; }
        } 

        public RegionMin(int regionId, int superRegionId, int army, byte ownerIdPlayer = 0, bool isWasteland = false)
        {
            ownerAndArmyEncoded = 0;
            Static = new RegionMinStatic(regionId, superRegionId, isWasteland);

            Army = army;
            OwnerId = ownerIdPlayer;
        }

        /// <summary>
        /// Gets OwnerPerspective of this region based on playerPerspective passed in parameter.
        /// </summary>
        /// <param name="playerPerspective"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public OwnerPerspective GetOwnerPerspective(byte playerPerspective)
        {
            if (OwnerId == 0)
            {
                return (byte)OwnerPerspective.Unoccupied;
            }

            return playerPerspective == OwnerId ? OwnerPerspective.Mine : OwnerPerspective.Enemy;
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

        public override string ToString()
        {
            return Id.ToString();
        }
    }
}