using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GameObjectsLib.Game;

namespace Server
{
    class WarlightServer : IDisposable
    {
        readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        readonly Queue<WarlightClient> clients = new Queue<WarlightClient>();
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
                while (clients.Count >= connectionsLimit)
                {
                    await Task.Delay(300);
                }
                TcpClient client = await listener.AcceptTcpClientAsync().ConfigureAwait(false);
                
                WarlightClient wClient = new WarlightClient(client, cancellationTokenSource.Token);
                clients.Enqueue(wClient);
                wClient.RunAsync();
            }
        }

        public void Dispose()
        {
            Dispose(false);
        }
        void Dispose(bool calledFromFinalizer = false)
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

    class ClientNotRespondingException : Exception
    {
        
    }
}
