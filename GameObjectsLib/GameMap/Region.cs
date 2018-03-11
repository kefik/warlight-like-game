﻿namespace GameObjectsLib.GameMap
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Players;
    using ProtoBuf;

    /// <summary>
    ///     Instace of this class represents region for given map in the game.
    /// </summary>
    [Serializable]
    [ProtoContract]
    public class Region : IEquatable<Region>
    {
        public const int MinimumArmy = 1;

        [ProtoMember(1)]
        public int Id { get; }

        [ProtoMember(2)]
        public string Name { get; }

        /// <summary>
        ///     Player owning given region. Null means no owner.
        /// </summary>
        [ProtoMember(3, AsReference = true)]
        public Player Owner { get; internal set; }

        /// <summary>
        ///     Number of units occuppying this region.
        /// </summary>
        [ProtoMember(4)]
        public int Army { get; set; }

        /// <summary>
        ///     Represents region group it belongs to.
        /// </summary>
        [ProtoMember(5, AsReference = true)]
        public SuperRegion SuperRegion { get; internal set; }

        /// <summary>
        ///     Represents list of regions that are neighbours to this given region.
        /// </summary>
        [ProtoMember(6, AsReference = true)]
        public IList<Region> NeighbourRegions { get; } = new List<Region>();


        public Region(int id, string name, SuperRegion superRegion)
        {
            Id = id;
            Name = name;
            SuperRegion = superRegion;
        }

        /// <summary>
        /// Changes owner of the region.
        /// </summary>
        /// <param name="newOwner"></param>
        public void ChangeOwner(Player newOwner)
        {
            if (newOwner == Owner)
            {
                return;
            }

            Owner?.ControlledRegionsInternal.Remove(this);
            if (newOwner != null)
            {
                newOwner.ControlledRegionsInternal.Add(this);
            }

            Owner = newOwner;
        }

        /// <summary>
        ///     Finds out whether region in parameter is neighbour of this instance.
        /// </summary>
        /// <param name="region">Region.</param>
        /// <returns>True, if it is neighbour to this instance.</returns>
        public bool IsNeighbourOf(Region region)
        {
            if (region == null)
            {
                return false;
            }
            return NeighbourRegions.Any(x => x == region);
        }

        /// <summary>
        ///     Finds out whether this region is neighbour of any of players controlled regions.
        /// </summary>
        /// <param name="player">Given player.</param>
        /// <returns>True, if it is neighbour of player.</returns>
        public bool IsNeighbourOf(Player player)
        {
            return player.ControlledRegions.Any(controlledRegion => controlledRegion.IsNeighbourOf(this));
        }

        private Region()
        {
        }

        public override string ToString()
        {
            return Name;
        }

        public bool Equals(Region other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return Id == other.Id;
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
            return Equals((Region) obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public static bool operator ==(Region left, Region right)
        {
            if (ReferenceEquals(left, null))
            {
                return ReferenceEquals(right, null);
            }
            return left.Equals(right);
        }

        public static bool operator !=(Region left, Region right)
        {
            return !(left == right);
        }
    }
}
