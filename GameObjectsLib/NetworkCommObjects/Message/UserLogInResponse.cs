using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace GameObjectsLib.NetworkCommObjects.Message
{
    [ProtoContract]
    public class UserLogInResponse
    {
        [ProtoMember(1)]
        public bool SuccessfullyLoggedIn { get; set; }
    }
}
