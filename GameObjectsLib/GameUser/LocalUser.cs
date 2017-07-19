using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace GameObjectsLib.GameUser
{
    [Serializable]
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    public class LocalUser : User
    {
        public override UserType UserType
        {
            get { return UserType.LocalUser; }
        }
        LocalUser(){ }

        public LocalUser(string name = "") : base(name)
        {
        }
    }
}
