using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using GameObjectsLib;
using GameObjectsLib.GameMap;
using ProtoBuf;

namespace GameObjectsLib.GameMap
{
    /// <summary>
    /// Instace of this class represents region for given map in the game.
    /// </summary>
    [Serializable]
    [ProtoContract(AsReferenceDefault = true, ImplicitFields = ImplicitFields.AllFields)]
    public class Region : IEquatable<Region>
    {
        public int Id { get; }
        public string Name { get; }
        /// <summary>
        /// Player owning given region. Null means no owner.
        /// </summary>
        public Player Owner { get; set; }

        /// <summary>
        /// Number of units occuppying this region.
        /// </summary>
        public int Army { get; set; }
        /// <summary>
        /// Represents region group it belongs to.
        /// </summary>
        public SuperRegion SuperRegion { get; }

        /// <summary>
        /// Represents list of regions that are neighbours to this given region.
        /// </summary>
        public IList<Region> NeighbourRegions { get; } = new List<Region>();
        

        public Region(int id, string name, SuperRegion superRegion)
        {
            Id = id;
            Name = name;
            SuperRegion = superRegion;
        }
        Region() { }

        public override string ToString()
        {
            return Name;
        }

        public bool Equals(Region other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Region) obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public static bool operator ==(Region left, Region right)
        {
            if (object.ReferenceEquals(left, null)) return object.ReferenceEquals(right, null);
            return left.Equals(right);
        }

        public static bool operator !=(Region left, Region right)
        {
            return !(left == right);
        }
    }
}
