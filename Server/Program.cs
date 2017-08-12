using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using ProtoBuf;
using Server.WarlightLikeDatabase;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(WarlightServer.GetLocalIPAddress());
            using (var server = WarlightServer.Create(5000))
            {
                server.Run(10);
            }

        }
    }
}
