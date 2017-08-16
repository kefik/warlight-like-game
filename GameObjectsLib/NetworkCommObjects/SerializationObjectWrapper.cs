namespace GameObjectsLib.NetworkCommObjects
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Game;
    using Message;
    using ProtoBuf;

    //public interface ISerializable<T>
    //{
    //    void Serialize(Stream stream);
    //}

    //public interface IAsynchronouslySerializable<T>
    //{
    //    Task SerializeAsync(Stream stream);
    //}

    //public static class SerializableExtensions
    //{
    //    public static void Serialize<T>(this ISerializable<T> serializable, Stream stream)
    //    {
    //        serializable.Serialize(stream);
    //    }

    //    public static async Task SerializeAsync<T>(this IAsynchronouslySerializable<T> serializable, Stream stream)
    //    {
    //        await serializable.SerializeAsync(stream);
    //    }
    //}
    /// <summary>
    ///     Network object wrapper whose purpose is to enable network communication via objects.
    /// </summary>
    [ProtoContract]
    [ProtoInclude(100, typeof(SerializationObjectWrapper<CreateGameRequestMessage>))]
    [ProtoInclude(101, typeof(SerializationObjectWrapper<CreateGameResponseMessage>))]
    [ProtoInclude(102, typeof(SerializationObjectWrapper<JoinGameRequestMessage>))]
    [ProtoInclude(103, typeof(SerializationObjectWrapper<JoinGameResponseMessage>))]
    [ProtoInclude(104, typeof(SerializationObjectWrapper<LoadGameRequestMessage>))]
    [ProtoInclude(105, typeof(SerializationObjectWrapper<LoadGameResponseMessage<bool>>))]
    [ProtoInclude(106, typeof(SerializationObjectWrapper<LoadMyGamesListRequestMessage>))]
    [ProtoInclude(107, typeof(SerializationObjectWrapper<LoadMyGamesListResponseMessage>))]
    [ProtoInclude(108, typeof(SerializationObjectWrapper<UserLogInRequestMessage>))]
    [ProtoInclude(109, typeof(SerializationObjectWrapper<UserLogInResponseMessage>))]
    [ProtoInclude(110, typeof(SerializationObjectWrapper<LoadOpenedGamesListRequestMessage>))]
    [ProtoInclude(111, typeof(SerializationObjectWrapper<LoadOpenedGamesListResponseMessage>))]
    [ProtoInclude(112, typeof(SerializationObjectWrapper<LoadOpenedGameRequestMessage>))]
    [ProtoInclude(113, typeof(SerializationObjectWrapper<LoadOpenedGameResponseMessage>))]
    [ProtoInclude(120, typeof(SerializationObjectWrapper<Game>))]
    public abstract class SerializationObjectWrapper
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
        public static async Task<SerializationObjectWrapper> DeserializeAsync(Stream stream)
        {
            int length = 0;
            if (!await Task.Factory.StartNew(
                () => Serializer.TryReadLengthPrefix(stream, PrefixStyle.Base128, out length),
                TaskCreationOptions.DenyChildAttach)) throw new ArgumentException();

            var buffer = new byte[length];
            await stream.ReadAsync(buffer, 0, buffer.Length);

            using (MemoryStream ms = new MemoryStream())
            {
                await ms.WriteAsync(buffer, 0, buffer.Length);

                ms.Position = 0;

                SerializationObjectWrapper wrapper = Serializer.Deserialize<SerializationObjectWrapper>(ms);

                return wrapper;
            }
        }

        public static SerializationObjectWrapper Deserialize(Stream stream)
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

                    SerializationObjectWrapper wrapper = Serializer.Deserialize<SerializationObjectWrapper>(ms);

                    return wrapper;
                }
            }

            throw new ArgumentException();
        }
    }

    /// <summary>
    ///     Typed NetworkObjectWrapper.
    /// </summary>
    /// <typeparam name="T">Type.</typeparam>
    [ProtoContract]
    public class SerializationObjectWrapper<T> : SerializationObjectWrapper
    {
        public override object Value
        {
            get { return TypedValue; }
        }

        [ProtoMember(1)]
        public T TypedValue { get; set; }
    }
}