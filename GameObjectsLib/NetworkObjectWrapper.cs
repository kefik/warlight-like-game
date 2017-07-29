using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GameObjectsLib.Game;
using GameObjectsLib.GameUser;
using ProtoBuf;

namespace GameObjectsLib
{
    [ProtoContract]
    [ProtoInclude(100, typeof(NetworkObjectWrapper<Game.Game>))]
    [ProtoInclude(101, typeof(NetworkObjectWrapper<Round>))]
    [ProtoInclude(102, typeof(NetworkObjectWrapper<GameBeginningRound>))]
    [ProtoInclude(103, typeof(NetworkObjectWrapper<bool>))]
    [ProtoInclude(104, typeof(NetworkObjectWrapper<HotseatGame>))]
    [ProtoInclude(105, typeof(NetworkObjectWrapper<NetworkGame>))]
    [ProtoInclude(106, typeof(NetworkObjectWrapper<MyNetworkUser>))]
    public abstract class NetworkObjectWrapper
    {
        public abstract object Value { get; }

        /// <summary>
        /// Asynchronously serializes this instance of the object into <see cref="stream"/>.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public async Task SerializeAsync(Stream stream)
        {
            await Task.Yield();
            Serializer.SerializeWithLengthPrefix(stream, this, PrefixStyle.Base128);
        }

        public void Serialize(Stream stream)
        {
            Serializer.SerializeWithLengthPrefix(stream, this, PrefixStyle.Base128);
        }

        /// <summary>
        /// Asynchronously deserializes object from the stream and returns it.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static async Task<NetworkObjectWrapper> DeserializeAsync(Stream stream)
        {
            await Task.Yield();

            int length;
            if (Serializer.TryReadLengthPrefix(stream, PrefixStyle.Base128, out length))
            {
                byte[] buffer = new byte[length];
                await stream.ReadAsync(buffer, 0, buffer.Length);

                using (var ms = new MemoryStream())
                {
                    await ms.WriteAsync(buffer, 0, buffer.Length);

                    ms.Position = 0;

                    var wrapper = Serializer.Deserialize<NetworkObjectWrapper>(ms);

                    return wrapper;
                }
            }

            throw new ArgumentException();
        }

        public static NetworkObjectWrapper Deserialize(Stream stream)
        {
            int length;
            if (Serializer.TryReadLengthPrefix(stream, PrefixStyle.Base128, out length))
            {
                byte[] buffer = new byte[length];
                stream.Read(buffer, 0, buffer.Length);

                using (var ms = new MemoryStream())
                {
                     ms.Write(buffer, 0, buffer.Length);

                    ms.Position = 0;

                    var wrapper = Serializer.Deserialize<NetworkObjectWrapper>(ms);

                    return wrapper;
                }
            }

            throw new ArgumentException();
        }

    }
    [ProtoContract]
    public class NetworkObjectWrapper<T> : NetworkObjectWrapper
    {
        public override object Value
        {
            get { return TypedValue; }
        }
        [ProtoMember(1)]
        public T TypedValue { get; set; }
    }
}
