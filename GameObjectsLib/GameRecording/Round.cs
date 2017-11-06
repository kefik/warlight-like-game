namespace GameObjectsLib.GameRecording
{
    using System.Collections.Generic;
    using ProtoBuf;

    /// <summary>
    ///     Abstract predecessor of GameRound and GameBeginningRound.
    /// </summary>
    [ProtoContract]
    [ProtoInclude(100, typeof(GameRound))]
    [ProtoInclude(101, typeof(GameBeginningRound))]
    public abstract class Round
    {
        public virtual IList<Turn> Turns { get; set; } = new List<Turn>();

        /// <summary>
        /// For protobuf-net.
        /// </summary>
        protected Round() { }

        public abstract ILinearizedRound Linearize();
    }
}
