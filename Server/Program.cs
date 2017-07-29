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
            //Console.Write($"Insert port number servers going to listen on: ");
            //string s = Console.ReadLine();
            using (var server = WarlightServer.Create(5000))
            {
                server.Run(10);
            }
        }
    }
}
