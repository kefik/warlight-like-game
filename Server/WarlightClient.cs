using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using GameObjectsLib;
using GameObjectsLib.GameUser;
using ProtoBuf;
using Server.WarlightLikeDatabase;
using Game = GameObjectsLib.Game.Game;

namespace Server
{
    class WarlightClient : IDisposable
    {
        readonly TcpClient client;
        CancellationToken token;

        public WarlightClient(TcpClient client, CancellationToken token)
        {
            this.client = client ?? throw new ArgumentNullException();
            this.token = token;
        }

        public async Task RunAsync()
        {
            while (!token.IsCancellationRequested)
            {
                var stream = client.GetStream();

                var delayTask = Task.Delay(TimeSpan.FromSeconds(50000));
                var deserializationTask = NetworkObjectWrapper.DeserializeAsync(stream);
                var completedTask = await Task.WhenAny(delayTask, deserializationTask);

                // client didnt send anything
                if (completedTask == delayTask)
                {
                    Dispose();
                    throw new ClientNotRespondingException();
                }
                // its non blocking now
                var deserializedObject = deserializationTask.Result.Value;
                // find out the type
                // TODO
                if (deserializedObject.GetType() == typeof(Game))
                {
                    Game game = (Game) deserializedObject;
                }
                else if (deserializedObject.GetType() == typeof(Round))
                {
                    Round round = (Round) deserializedObject;
                }
                else if (deserializedObject.GetType() == typeof(GameBeginningRound))
                {
                    GameBeginningRound round = (GameBeginningRound) deserializedObject;
                }
                else if (deserializedObject.GetType() == typeof(MyNetworkUser))
                {
                    MyNetworkUser user = (MyNetworkUser) deserializedObject;
                    using (var db = new WarlightDbContext())
                    {
                        bool existsMatchingUser = (from dbUser in db.Users
                                                   where dbUser.Login == user.Name &&
                                                         dbUser.PasswordHash == user.PasswordHash
                                                   select dbUser).Any();

                        NetworkObjectWrapper confirmation =
                            new NetworkObjectWrapper<bool>() {TypedValue = existsMatchingUser};
                        await confirmation.SerializeAsync(stream);
                    }
                }
            }

        }

        bool isDisposed;
        public void Dispose()
        {
            Dispose(false);
        }
        void Dispose(bool calledFromFinalizer)
        {
            if (calledFromFinalizer == false) GC.SuppressFinalize(this);
            if (isDisposed == false)
            {
                client.Close();
                isDisposed = true;
            }
        }

        ~WarlightClient()
        {
            Dispose(true);
        }
    }
}
