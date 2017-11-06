namespace GameObjectsLib.Players
{
    using System;
    using System.Drawing;
    using GameUser;
    using ProtoBuf;

    /// <summary>
    ///     Instance of this class represents human player in the game.
    /// </summary>
    [Serializable]
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    public sealed class HumanPlayer : Player, IEquatable<HumanPlayer>
    {
        private HumanPlayer()
        {
        }

        /// <summary>
        ///     Represents user this human player is linked to.
        /// </summary>
        public User User { get; }

        public override string Name
        {
            get { return User.Name; }
        }

        public HumanPlayer(User user, KnownColor color) : base(color)
        {
            User = user ?? throw new ArgumentException();
        }

        public bool Equals(HumanPlayer other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return base.Equals(other) && Equals(User, other.User);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            HumanPlayer player = obj as HumanPlayer;
            return player != null && Equals(player);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode() * 397) ^ (User != null ? User.GetHashCode() : 0);
            }
        }
    }
}