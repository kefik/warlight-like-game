using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization;
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
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    [ProtoInclude(400, typeof(AIPlayer))]
    [ProtoInclude(401, typeof(HumanPlayer))]
    public abstract class Player : IEquatable<Player>//, ISerializable
    {
        protected Player() { }
        public abstract string Name { get; }
        public System.Drawing.KnownColor Color { get; }

        /// <summary>
        /// Regions this player controls.
        /// </summary>
        public ICollection<Region> ControlledRegions { get; } = new HashSet<Region>();
        
        protected Player(System.Drawing.KnownColor color)
        {
            Color = color;
        }

        public override string ToString()
        {
            return Name;
        }

        //public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        //{
        //    info.AddValue(nameof(Color), Color, typeof(KnownColor));
            
        //    //info.AddValue(nameof(ControlledRegions), ControlledRegions, typeof(HashSet<Region>));
        //}

        //protected Player(SerializationInfo info, StreamingContext context)
        //{
        //    Color = (KnownColor)info.GetValue(nameof(Color), typeof(KnownColor));

        //    //ControlledRegions =
        //    //    (ICollection<Region>) info.GetValue(nameof(ControlledRegions), typeof(HashSet<Region>));
        //}

        public bool Equals(Player other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Color == other.Color && Equals(ControlledRegions, other.ControlledRegions);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Player) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) Color * 397) ^ (ControlledRegions != null ? ControlledRegions.GetHashCode() : 0);
            }
        }

        public static bool operator ==(Player left, Player right)
        {
            if (left == null) return false;

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
    public class AIPlayer : Player
    {
        AIPlayer() { }
        /// <summary>
        /// Represents difficulty of given artifficial player.
        /// </summary>
        public Difficulty Difficulty { get; }

        public override string Name { get; }

        public AIPlayer(Difficulty difficulty, string name, System.Drawing.KnownColor color) : base(color)
        {
            this.Difficulty = difficulty;
            Name = name;
        }

        //public override void GetObjectData(SerializationInfo info, StreamingContext context)
        //{
        //    base.GetObjectData(info, context);
        //    // serialize difficulty
        //    info.AddValue(nameof(Difficulty), Difficulty, typeof(Difficulty));
        //}

        //AIPlayer(SerializationInfo info, StreamingContext context) : base(info, context)
        //{
        //    Difficulty = (Difficulty) info.GetValue(nameof(Difficulty), typeof(Difficulty));
        //}
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

        public HumanPlayer(User user, System.Drawing.KnownColor color) : base(color)
        {
            this.User = user ?? throw new ArgumentException();
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
