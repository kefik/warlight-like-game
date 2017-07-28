using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using GameObjectsLib.Game;
using ProtoBuf;

namespace Server
{
    class WarlightServer
    {
        readonly Queue<Task> clients = new Queue<Task>();
        readonly TcpListener listener;
        private WarlightServer(IPEndPoint endPoint)
        {
            listener = new TcpListener(endPoint);
        }
        /// <summary>
        /// Creates an instance of Warlight server on given port and given pcs inter network address.
        /// </summary>
        /// <param name="port">Port number.</param>
        /// <returns>Instance of the server prepared to run.</returns>
        public static WarlightServer Create(int port)
        {
            WarlightServer server;
            {
                IPEndPoint endPoint = new IPEndPoint(GetLocalIPAddress(), port);
                server = new WarlightServer(endPoint);
            }

            return server;
        }

        /// <summary>
        /// Creates instance of Warlight server based on address and port.
        /// </summary>
        /// <param name="address">Address of the server.</param>
        /// <param name="port">Port where it will be listening on.</param>
        /// <returns>Instance of the server prepared to run.</returns>
        public static WarlightServer Create(IPAddress address, int port)
        {
            WarlightServer server;
            {
                IPEndPoint endPoint = new IPEndPoint(address, port);
                server = new WarlightServer(endPoint);
            }


            return server;
        }
        

        public void Run(int connectionsLimit)
        {
            listener.Start();

            Accept(connectionsLimit);
            
            while (true)
            {
                while (clients.Any())
                {
                    var clientTask = clients.Dequeue();

                    if (clientTask.IsFaulted || clientTask.IsCompleted) continue;

                    clients.Enqueue(clientTask);
                    
                    
                }
                Task.Delay(100);
            }
        }

        async void Accept(int connectionsLimit)
        {
            while (true)
            {
                while (clients.Count >= connectionsLimit)
                {
                    await Task.Delay(300);
                }
                TcpClient client = await listener.AcceptTcpClientAsync();
                clients.Enqueue(Task.Run(() => new WarlightClient(client).Run()));
            }
        }

        

        static IPAddress GetLocalIPAddress()
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                return null;
            }

            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

            return host
                .AddressList
                .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
        }
    }

    class WarlightClient
    {
        readonly TcpClient client;
        readonly Stopwatch stopwatch;

        bool finished;
        public bool IsFinished
        {
            get { return stopwatch.ElapsedMilliseconds > 1000 || finished; }
        }

        public WarlightClient(TcpClient client)
        {
            this.client = client;
            stopwatch = Stopwatch.StartNew();
        }

        public void Run()
        {
            // TODO: horrible, get it better
            var stream = client.GetStream();

            while (!stream.DataAvailable) ;

            if (IsFinished) return;
            stopwatch.Restart();

            var gameObject = Serializer.Deserialize<object>(stream);
            // TODO: game logic


            finished = true;
        }


    }
}
