namespace GameObjectsLib
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using GameMap;
    using ProtoBuf;
    using Region = GameMap.Region;

    /// <summary>
    ///     Instance of this class represents template for player in the game.
    /// </summary>
    [Serializable]
    [ProtoContract]
    [ProtoInclude(400, typeof(AiPlayer))]
    [ProtoInclude(401, typeof(HumanPlayer))]
    public abstract class Player : IEquatable<Player>, IRefreshable
    {
        protected Player()
        {
        }

        public abstract string Name { get; }

        [ProtoMember(1)]
        public KnownColor Color { get; }

        /// <summary>
        ///     Regions this player controls.
        /// </summary>
        [ProtoMember(2, AsReference = true)]
        public IList<Region> ControlledRegions { get; } = new List<Region>();

        private const int BasicIncome = 5;

        /// <summary>
        ///     Calculates players army income and returns it.
        /// </summary>
        /// <returns>Army income of the player.</returns>
        public int GetIncome()
        {
            IEnumerable<SuperRegion> superRegionsOwned = (from region in ControlledRegions
                                                          select region.SuperRegion
                                                          into superRegion
                                                          where superRegion.Owner == this
                                                          select superRegion).Distinct();
            return BasicIncome + superRegionsOwned.Sum(superRegion => superRegion.Bonus);
        }

        protected Player(KnownColor color)
        {
            Color = color;
        }

        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        ///     Refreshes controlled regions of the player.
        /// </summary>
        public void Refresh()
        {
            // if this players controlled region doesnt have as owner this player
            // remove that region from controlled regions
            for (int i = ControlledRegions.Count - 1; i >= 0; i--)
            {
                if (ControlledRegions[i].Owner != this)
                {
                    ControlledRegions.Remove(ControlledRegions[i]);
                }
            }
        }

        public bool Equals(Player other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return Color == other.Color && Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((Player) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) Color * 397) ^ (ControlledRegions != null ? ControlledRegions.GetHashCode() : 0);
            }
        }

        public static bool operator ==(Player left, Player right)
        {
            if (ReferenceEquals(left, null))
            {
                return ReferenceEquals(right, null);
            }
            return left.Equals(right);
        }

        public static bool operator !=(Player left, Player right)
        {
            return !(left == right);
        }
    }
}
