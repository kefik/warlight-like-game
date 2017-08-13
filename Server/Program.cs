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
            const string ipAddress = "127.0.0.1";
            Console.WriteLine(ipAddress);
            using (var server = WarlightServer.Create(IPAddress.Parse(ipAddress), 5000))
            {
                server.Run(10);
            }
        }
    }
}
