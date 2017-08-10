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
using GameObjectsLib.Game;
using GameObjectsLib.GameMap;
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
            try
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
                    if (deserializedObject.GetType() == typeof(GameSeed))
                    {
                        GameSeed seed = (GameSeed)deserializedObject;

                        // seed to create the game
                    }
                    else if (deserializedObject.GetType() == typeof(Round))
                    {
                        Round round = (Round)deserializedObject;
                    }
                    else if (deserializedObject.GetType() == typeof(GameBeginningRound))
                    {
                        GameBeginningRound round = (GameBeginningRound)deserializedObject;
                    }
                    else if (deserializedObject.GetType() == typeof(MyNetworkUser))
                    {
                        MyNetworkUser user = (MyNetworkUser)deserializedObject;
                        string passwordHash;
                        {
                            // calculate hash
                            byte[] data = System.Text.Encoding.ASCII.GetBytes(user.Password);
                            data = new System.Security.Cryptography.SHA256Managed().ComputeHash(data);
                            passwordHash = System.Text.Encoding.ASCII.GetString(data);
                        }
                        using (var db = new WarlightDbContext())
                        {
                            var matchedUser = (from dbUser in db.Users
                                               where (dbUser.Login == user.Name) &&
                                                     dbUser.PasswordHash == passwordHash
                                               select dbUser).FirstOrDefault();
                            bool existsMatchingUser = matchedUser != null;
                            {
                                NetworkObjectWrapper confirmation =
                                    new NetworkObjectWrapper<bool>() { TypedValue = existsMatchingUser };
                                await confirmation.SerializeAsync(stream);
                            }
                            if (existsMatchingUser)
                            {
                                user.Email = matchedUser.Email;
                                NetworkObjectWrapper userWrapper =
                                    new NetworkObjectWrapper<MyNetworkUser>() { TypedValue = user };
                                await userWrapper.SerializeAsync(stream);
                            }
                        }
                    }
                }
            }
            finally
            {
                Dispose();
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
