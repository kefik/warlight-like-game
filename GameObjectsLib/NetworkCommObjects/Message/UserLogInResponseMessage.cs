using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace GameObjectsLib.NetworkCommObjects.Message
{
    /// <summary>
    /// Response whether the attempt to log in was successful.
    /// </summary>
    [ProtoContract]
    public class UserLogInResponseMessage
    {
        [ProtoMember(1)]
        public bool SuccessfullyLoggedIn { get; set; }
    }
}
