using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameObjectsLib.GameMap;
using ProtoBuf;

namespace GameObjectsLib.GameMap
{
    /// <summary>
    /// Represents region group of giving size giving bonus to player who owns it all.
    /// </summary>
    [Serializable]
    [ProtoContract]
    public class SuperRegion : IEquatable<SuperRegion>, IRefreshable
    {
        [ProtoMember(1)]
        public int Id { get; }
        /// <summary>
        /// Name given of the SuperRegion.
        /// </summary>
        [ProtoMember(2)]
        public string Name { get; }
        /// <summary>
        /// Bonus given by the SuperRegion taken by one player.
        /// </summary>
        [ProtoMember(3)]
        public int Bonus { get; }

        [ProtoMember(4, AsReference = true)]
        public Player Owner { get; private set; } // TODO: finish refreshing of situation, use GetOwner() method

        [ProtoMember(5, AsReference = true)]
        public IList<Region> Regions { get; } = new List<Region>();
        /// <summary>
        /// Constructs SuperRegion instance.
        /// </summary>
        /// <param name="name">Name of the new SuperRegion.</param>
        /// <param name="bonus">Bonus given by SuperRegion.</param>
        /// <param name="id">Id identifying the SuperRegion.</param>
        public SuperRegion(int id, string name, int bonus)
        {
            Id = id;
            Name = name;
            Bonus = bonus;
        }
        /// <summary>
        /// Calculates owner of this SuperRegion.
        /// </summary>
        /// <returns>Owner of this SuperRegion, or null.</returns>
        private Player GetOwner()
        {
            var firstOwner = Regions.FirstOrDefault()?.Owner;
            return (from region in Regions where region.Owner != firstOwner select region).Any() // if there exists any such that hes not same as the first owner
                ? null // return null
                : firstOwner;
        }
        /// <summary>
        /// Refreshes given instance of super region.
        /// </summary>
        public void Refresh()
        {
            Owner = GetOwner();
        }


        SuperRegion() { }

        public override string ToString()
        {
            string name = string.Format($"{nameof(Name)}: {Name}");
            string bonus = string.Format($"{nameof(Bonus)}: {Bonus}");
            string regions;
            {
                var sb = new StringBuilder();
                foreach (var region in Regions)
                {
                    sb.Append(region.Name + ", ");
                }
                regions = string.Format($"{nameof(Regions)}: {sb}");
            }
            return name + ", " + bonus + ", " + regions;
        }

        public bool Equals(SuperRegion other)
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
            return Equals((SuperRegion) obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public static bool operator ==(SuperRegion left, SuperRegion right)
        {
            if (object.ReferenceEquals(left, null)) return object.ReferenceEquals(right, null);
            return left.Equals(right);
        }

        public static bool operator !=(SuperRegion left, SuperRegion right)
        {
            return !(left == right);
        }
    }
}
