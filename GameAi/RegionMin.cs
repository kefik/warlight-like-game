namespace GameAi
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using GameObjectsLib;
    using GameObjectsLib.GameMap;

    /// <summary>
    /// Minimized version of <see cref="Region"/>.
    /// </summary>
    internal class RegionMin : IEquatable<RegionMin>
    {
        private class RegionMinStatic
        {
            public int Id { get; }
            public RegionMin[] Neighbours { get; set; }
            public SuperRegionMin SuperRegion { get; }

            public RegionMinStatic(Region region, Player myPlayer)
            {
                Id = region.Id;
                SuperRegion = new SuperRegionMin(region.SuperRegion, myPlayer);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool IsNeighbourOf(RegionMin region)
            {
                foreach (var neighbour in Neighbours)
                {
                    if (neighbour.Static.Id == region.Static.Id)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        /// <summary>
        /// First 2 bits = owner, 14 bits = army
        /// </summary>
        private ushort ownerAndArmyEncoded;

        private RegionMinStatic Static { get; }

        /// <summary>
        /// Represents the owner from players perspective specified in constructor.
        /// </summary>
        public OwnershipState Owner
        {
            get { return (OwnershipState)(ownerAndArmyEncoded & 0b11); }
            set { ownerAndArmyEncoded = (ushort)(((ushort.MaxValue >> 2) << 2) | (ushort)value); }
        }

        public int Id
        {
            get { return Static.Id; }
        }

        /// <summary>
        /// Represents size of army of the region.
        /// </summary>
        public int Army
        {
            get { return ownerAndArmyEncoded >> 3; }
            set
            {
                ownerAndArmyEncoded = (ushort)(((ushort.MaxValue >> 13) << 13) | ((ushort)value >> 3));
            }
        }

        /// <summary>
        /// True, if the region is hidden by Fog of war.
        /// </summary>
        public bool IsVisible
        {
            get { return (ownerAndArmyEncoded & 0b100) != 0; }
            set
            {
                ushort mask;
                if (value)
                {
                    mask = ushort.MaxValue & 0b100;
                }
                else
                {
                    mask = 0;
                }
                ownerAndArmyEncoded = (ushort)(mask | ownerAndArmyEncoded);
            }
        }

        /// <summary>
        /// SuperRegion in which this region is contained.
        /// </summary>
        internal SuperRegionMin SuperRegion
        {
            get { return Static.SuperRegion; }
        }

        /// <summary>
        /// Array of neighbour regions of this given region.
        /// </summary>
        internal RegionMin[] NeighbourRegions
        {
            get { return Static.Neighbours; }
            set { Static.Neighbours = value; }
        } 
        
        internal RegionMin(Region region, Player playerPerspective)
        {
            Army = region.Army;
            if (region.Owner == null)
            {
                Owner = OwnershipState.Unoccupied;
            }
            else if (region.Owner == playerPerspective)
            {
                Owner = OwnershipState.Mine;
            }
            else
            {
                Owner = OwnershipState.Enemy;
            }
            if (region.IsNeighbourOf(playerPerspective))
            {
                IsVisible = true;
            }

            Static = new RegionMin.RegionMinStatic(region, playerPerspective);
        }

        /// <summary>
        /// Shallow-copies of this instance.
        /// </summary>
        /// <returns>Shallow copy of this instance.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RegionMin Copy()
        {
            return (RegionMin)MemberwiseClone();
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
        
        public bool Equals(RegionMin other)
        {
            return !ReferenceEquals(null, other) && Static.Id == other.Static.Id;
        }
        
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (obj.GetType() != this.GetType())
            {
                return false;
            }
            return Equals((RegionMin)obj);
        }
        
        public override int GetHashCode()
        {
            return (Static != null ? Static.GetHashCode() : 0);
        }
        
        public static bool operator ==(RegionMin left, RegionMin right)
        {
            return left != null && !ReferenceEquals(null, right) && left.Static.Id == right.Static.Id;
        }
        
        public static bool operator !=(RegionMin left, RegionMin right)
        {
            return !(left == right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ReferenceEquals(RegionMin other)
        {
            return ReferenceEquals(this, other);
        }
    }
}