using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Diagnostics;
using GameObjectsLib.Game;

namespace Server
{
    

    class ConnectedUser
    {
        public int Id { get; }
        readonly Stopwatch stopwatch;
        public bool IsConnected
        {
            // elapsed more than 5 minus
            get { return ((double)stopwatch.ElapsedMilliseconds / 1000) / 60 >= 5; }
        }
        
        public ConnectedUser(int id)
        {
            Id = id;
            stopwatch = Stopwatch.StartNew();
        }

        public void Refresh()
        {
            stopwatch.Restart();
        }
    }
    class WarlightServer : IDisposable
    {
        readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        readonly List<WarlightClient> directlyConnectedClients = new List<WarlightClient>();

        readonly Dictionary<string, WarlightClient> connectedUsers = new Dictionary<string, WarlightClient>();
        
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
            try
            {
                listener.Start();
                
                AcceptClientsAsync(connectionsLimit);

                Thread.Sleep(Timeout.Infinite);
            }
            finally
            {
                Dispose();
            }
        }

        async void AcceptClientsAsync(int connectionsLimit)
        {
            while (true)
            {
                while (directlyConnectedClients.Count >= connectionsLimit)
                {
                    await Task.Delay(300);
                }
                TcpClient client = await listener.AcceptTcpClientAsync().ConfigureAwait(false);
                WarlightClient wClient = new WarlightClient(client, cancellationTokenSource.Token);
                directlyConnectedClients.Add(wClient);
                // TODO: thread synchronizing problem
                wClient.RunAsync().ContinueWith(antecedent => directlyConnectedClients.Remove(wClient));
            }
        }

        public void Dispose()
        {
            Dispose(false);
        }
        void Dispose(bool calledFromFinalizer)
        {
            if (!cancellationTokenSource.IsCancellationRequested)
            {
                cancellationTokenSource.Cancel();
                if (calledFromFinalizer == false) GC.SuppressFinalize(this);
            }
        }

        ~WarlightServer()
        {
            Dispose(true);
        }

        public static IPAddress GetLocalIPAddress()
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

    class ClientNotRespondingException : Exception
    {
        
    }
}
