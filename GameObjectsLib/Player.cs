using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using GameObjectsLib.GameMap;
using GameObjectsLib.GameUser;
using ProtoBuf;
using Region = GameObjectsLib.GameMap.Region;

namespace GameObjectsLib
{
    /// <summary>
    /// Represents difficulty of given artifficial player.
    /// </summary>
    public enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }

    /// <summary>
    /// Instance of this class represents template for player in the game.
    /// </summary>
    [Serializable]
    [ProtoContract]
    [ProtoInclude(400, typeof(AIPlayer))]
    [ProtoInclude(401, typeof(HumanPlayer))]
    public abstract class Player : IEquatable<Player>, IRefreshable
    {
        protected Player() { }
        public abstract string Name { get; }
        [ProtoMember(1)]
        public KnownColor Color { get; }

        /// <summary>
        /// Regions this player controls.
        /// </summary>
        [ProtoMember(2, AsReference = true)]
        public IList<Region> ControlledRegions { get; } = new List<Region>();

        const int BasicIncome = 5;
        /// <summary>
        /// Calculates players army income and returns it.
        /// </summary>
        /// <returns>Army income of the player.</returns>
        public int GetIncome()
        {
            var superRegionsOwned = (from region in ControlledRegions
                                select region.SuperRegion into superRegion
                                where superRegion.Owner == this
                                select superRegion).Distinct();
            return BasicIncome + superRegionsOwned.Sum(superRegion => superRegion.Bonus);
        }

        protected Player(KnownColor color)
        {
            Color = color;
        }

        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Refreshes controlled regions of the player.
        /// </summary>
        public void Refresh()
        {
            // if this players controlled region doesnt have as owner this player
            // remove that region from controlled regions
            for (int i = ControlledRegions.Count - 1; i >= 0; i--)
            {
                if (ControlledRegions[i].Owner != this)
                    ControlledRegions.Remove(ControlledRegions[i]);
            }
        }

        public bool Equals(Player other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Color == other.Color && Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Player)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int)Color * 397) ^ (ControlledRegions != null ? ControlledRegions.GetHashCode() : 0);
            }
        }

        public static bool operator ==(Player left, Player right)
        {
            if (ReferenceEquals(left, null)) return ReferenceEquals(right, null);
            return left.Equals(right);
        }

        public static bool operator !=(Player left, Player right)
        {
            return !(left == right);
        }
    }

    /// <summary>
    /// Instance of this class represents AI player in the game.
    /// </summary>
    [Serializable, ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    public sealed class AIPlayer : Player
    {
        AIPlayer() { }
        /// <summary>
        /// Represents difficulty of given artifficial player.
        /// </summary>
        public Difficulty Difficulty { get; }

        public override string Name { get; }

        public AIPlayer(Difficulty difficulty, string name, KnownColor color) : base(color)
        {
            Difficulty = difficulty;
            Name = name;
        }
    }

    /// <summary>
    /// Instance of this class represents human player in the game.
    /// </summary>
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    public sealed class HumanPlayer : Player, IEquatable<HumanPlayer>
    {
        HumanPlayer() { }
        /// <summary>
        /// Represents user this human player is linked to.
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
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && Equals(User, other.User);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
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
