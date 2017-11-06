namespace GameObjectsLib
{
    using Players;
    using ProtoBuf;

    [ProtoContract]
    [ProtoInclude(100, typeof(GameTurn))]
    [ProtoInclude(101, typeof(GameBeginningTurn))]
    public abstract class Turn
    {
        [ProtoMember(1, AsReference = true)]
        public Player PlayerOnTurn { get; }

        protected Turn(Player playerOnTurn)
        {
            PlayerOnTurn = playerOnTurn;
        }

        public abstract void Reset();
    }
}