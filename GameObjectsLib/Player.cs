using System;
using System.Collections.Generic;
using GameObjectsLib.GameMap;
using GameObjectsLib.GameUser;

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
    public abstract class Player
    {
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
    }

    /// <summary>
    /// Instance of this class represents AI player in the game.
    /// </summary>
    public class AIPlayer : Player
    {
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
    }

    /// <summary>
    /// Instance of this class represents human player in the game.
    /// </summary>
    public class HumanPlayer : Player
    {
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
    }
}
