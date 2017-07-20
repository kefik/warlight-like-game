using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace GameObjectsLib.GameUser
{
    [Serializable]
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    public class NetworkUser : User, IEquatable<NetworkUser>
    {
        public int Id { get; }

        public override UserType UserType
        {
            get { return UserType.NetworkUser; }
        }

        public NetworkUser(int id, string name) : base(name)
        {
            Id = id;
        }

        public bool Equals(NetworkUser other)
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
            return Equals((NetworkUser) obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }

        //public static bool operator ==(NetworkUser left, NetworkUser right)
        //{
        //    if (left == null) return false;
        //    return left.Equals(right);
        //}

        //public static bool operator !=(NetworkUser left, NetworkUser right)
        //{
        //    return !(left == right);
        //}
    }
}