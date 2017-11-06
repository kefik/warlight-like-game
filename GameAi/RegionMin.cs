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
    /// Minimized version of <see cref="Region"/>.
    /// </summary>
    public class RegionMin
    {
        private class RegionMinStatic
        {
            public int Id { get; }
            public RegionMin[] Neighbours { get; set; }
            public SuperRegionMin SuperRegion { get; }

            public RegionMinStatic(Region region, Player ownerEncoded)
            {
                Id = region.Id;
                SuperRegion = new SuperRegionMin(region.SuperRegion, ownerEncoded);
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
        /// First 5 bits = owner, 1 visibility, 12 bits = army
        /// </summary>
        private ushort ownerAndArmyEncoded;

        private RegionMinStatic Static { get; }
        
        /// <summary>
        /// Represents the owner from players perspective specified in constructor.
        /// </summary>
        internal byte OwnerEncoded
        {
            get { return (byte) (ownerAndArmyEncoded & 0b11111); }
            set
            {
                ownerAndArmyEncoded &= 0b1111111111100000; // 5 zeros, 11 ones
                value &= 0b00011111;
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
        /// Represents size of army of the region.
        /// </summary>
        public int Army
        {
            get { return ownerAndArmyEncoded >> 6; }
            internal set
            {
                ownerAndArmyEncoded = (ushort)(((ushort.MaxValue >> 10) << 10) | ((ushort)value >> 6));
            }
        }
        
        /// <summary>
        /// True, if the region is hidden by Fog of war.
        /// </summary>
        public bool IsVisible
        {
            get { return (ownerAndArmyEncoded & 0b100000) != 0; }
            internal set
            {
                ushort mask;
                if (value)
                {
                    mask = ushort.MaxValue & 0b100000;
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
        public SuperRegionMin SuperRegion
        {
            get { return Static.SuperRegion; }
        }

        /// <summary>
        /// Array of neighbour regions of this given region.
        /// </summary>
        public RegionMin[] NeighbourRegions
        {
            get { return Static.Neighbours; }
            internal set { Static.Neighbours = value; }
        } 
        
        internal RegionMin(Region region, byte ownerEncoded, Player playerPerspective, bool isFogOfWarGame = true)
        {
            Army = region.Army;
            OwnerEncoded = region.Owner == null ? (byte)0 : ownerEncoded;
            
            if (!isFogOfWarGame || region.IsNeighbourOf(playerPerspective))
            {
                IsVisible = true;
            }

            Static = new RegionMinStatic(region, playerPerspective);
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
        /// Shallow-copies of this instance.
        /// </summary>
        /// <returns>Shallow copy of this instance.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal RegionMin ShallowCopy()
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
    }
}