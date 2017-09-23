namespace Server
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;

    internal class ConnectedUser
    {
        public int Id { get; }
        private readonly Stopwatch stopwatch;

        public bool IsConnected
        {
            // elapsed more than 5 minus
            get { return (double) stopwatch.ElapsedMilliseconds / 1000 / 60 >= 5; }
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

    internal class WarlightServer : IDisposable
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly List<WarlightClient> directlyConnectedClients = new List<WarlightClient>();

        private readonly Dictionary<string, WarlightClient> connectedUsers = new Dictionary<string, WarlightClient>();

        private readonly TcpListener listener;

        private WarlightServer(IPEndPoint endPoint)
        {
            listener = new TcpListener(endPoint);
        }

        /// <summary>
        ///     Creates an instance of Warlight server on given port and given pcs inter network address.
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
        ///     Creates instance of Warlight server based on address and port.
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

        private async void AcceptClientsAsync(int connectionsLimit)
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

        private void Dispose(bool calledFromFinalizer)
        {
            if (!cancellationTokenSource.IsCancellationRequested)
            {
                cancellationTokenSource.Cancel();
                if (calledFromFinalizer == false)
                {
                    GC.SuppressFinalize(this);
                }
            }
        }

        ~WarlightServer()
        {
            Dispose(true);
        }

        public static IPAddress GetLocalIPAddress()
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                return null;
            }

            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

            return host
                .AddressList
                .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
        }
    }

    internal class ClientNotRespondingException : Exception
    {
    }
}
