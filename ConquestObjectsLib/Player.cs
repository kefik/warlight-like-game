using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquestObjectsLib
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

    // TODO: rethink locking after game started (currently player is immutable, maybe I should remake it like Game class or the other way
    /// <summary>
    /// Instance of this class represents template for player in the game.
    /// </summary>
    abstract class Player
    {
        public abstract string Name { get; }
        public Color Color { get; }

        protected Player(Color color)
        {
            Color = color;
        }
    }

    /// <summary>
    /// Instance of this class represents AI player in the game.
    /// </summary>
    class AIPlayer : Player
    {
        /// <summary>
        /// Represents difficulty of given artifficial player.
        /// </summary>
        public Difficulty Difficulty { get; }

        public override string Name { get; }

        public AIPlayer(Difficulty difficulty, string name, Color color) : base(color)
        {
            this.Difficulty = difficulty;
            Name = name;
        }
    }

    /// <summary>
    /// Instance of this class represents human player in the game.
    /// </summary>
    class HumanPlayer : Player
    {
        /// <summary>
        /// Represents user this human player is linked to.
        /// </summary>
        public User User { get; }

        public override string Name
        {
            get { return User.Name; }
        }

        public HumanPlayer(User user, Color color) : base(color)
        {
            this.User = user ?? throw new ArgumentException();
        }
    }
}
