namespace GameObjectsLib
{
    using ProtoBuf;

    [ProtoContract]
    [ProtoInclude(100, typeof(LinearizedGameRound))]
    [ProtoInclude(101, typeof(LinearizedGameBeginningRound))]
    public interface ILinearizedRound
    {
        
    }
}