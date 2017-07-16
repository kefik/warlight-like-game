using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquestObjectsLib.GameUser
{
    public class NetworkUser : User
    {
        public override UserType UserType
        {
            get { return UserType.Network; }
        }

        public NetworkUser(string name) : base(name)
        {
        }
    }
}
