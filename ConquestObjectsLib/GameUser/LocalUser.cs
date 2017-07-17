using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquestObjectsLib.GameUser
{
    public class LocalUser : User
    {
        public override UserType UserType
        {
            get { return UserType.LocalUser; }
        }

        public LocalUser(string name = "") : base(name)
        {
        }
    }
}
