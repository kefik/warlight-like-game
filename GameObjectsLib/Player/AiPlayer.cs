namespace GameObjectsLib.Player
{
    using System;
    using System.Drawing;
    using ProtoBuf;

    /// <summary>
    ///     Instance of this class represents Ai player in the game.
    /// </summary>
    [Serializable]
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    public sealed class AiPlayer : Player
    {
        private AiPlayer()
        {
        }

        /// <summary>
        ///     Represents difficulty of given artifficial player.
        /// </summary>
        public Difficulty Difficulty { get; }

        public override string Name { get; }

        public AiPlayer(Difficulty difficulty, string name, KnownColor color) : base(color)
        {
            Difficulty = difficulty;
            Name = name;
        }
    }
}