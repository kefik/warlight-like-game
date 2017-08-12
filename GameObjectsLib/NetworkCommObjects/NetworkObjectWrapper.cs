namespace GameObjectsLib.NetworkCommObjects
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Message;
    using ProtoBuf;

    /// <summary>
    /// Network object wrapper whose purpose is to enable network communication via objects.
    /// </summary>
    [ProtoContract, ProtoInclude(100, typeof(NetworkObjectWrapper<CreateGameRequestMessage>)),
     ProtoInclude(101, typeof(NetworkObjectWrapper<CreateGameResponseMessage>)),
     ProtoInclude(102, typeof(NetworkObjectWrapper<JoinGameRequestMessage>)),
     ProtoInclude(103, typeof(NetworkObjectWrapper<JoinGameResponseMessage>)),
     ProtoInclude(104, typeof(NetworkObjectWrapper<LoadGameRequestMessage>)),
     ProtoInclude(105, typeof(NetworkObjectWrapper<LoadGameResponseMessage<bool>>)),
     ProtoInclude(106, typeof(NetworkObjectWrapper<LoadMyGamesListRequestMessage>)),
     ProtoInclude(107, typeof(NetworkObjectWrapper<LoadMyGamesResponseMessage>)),
     ProtoInclude(108, typeof(NetworkObjectWrapper<UserLogInRequestMessage>)),
     ProtoInclude(109, typeof(NetworkObjectWrapper<UserLogInResponseMessage>))]
    public abstract class NetworkObjectWrapper
    {
        public abstract object Value { get; }

        /// <summary>
        ///     Asynchronously serializes this instance of the object into <see cref="stream" />.
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
        ///     Asynchronously deserializes object from the stream and returns it.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static async Task<NetworkObjectWrapper> DeserializeAsync(Stream stream)
        {
            await Task.Yield();

            int length;
            if (Serializer.TryReadLengthPrefix(stream, PrefixStyle.Base128, out length))
            {
                var buffer = new byte[length];
                await stream.ReadAsync(buffer, 0, buffer.Length);

                using (MemoryStream ms = new MemoryStream())
                {
                    await ms.WriteAsync(buffer, 0, buffer.Length);

                    ms.Position = 0;

                    NetworkObjectWrapper wrapper = Serializer.Deserialize<NetworkObjectWrapper>(ms);

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
                var buffer = new byte[length];
                stream.Read(buffer, 0, buffer.Length);

                using (MemoryStream ms = new MemoryStream())
                {
                    ms.Write(buffer, 0, buffer.Length);

                    ms.Position = 0;

                    NetworkObjectWrapper wrapper = Serializer.Deserialize<NetworkObjectWrapper>(ms);

                    return wrapper;
                }
            }

            throw new ArgumentException();
        }
    }
    /// <summary>
    /// Typed NetworkObjectWrapper.
    /// </summary>
    /// <typeparam name="T">Type.</typeparam>
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