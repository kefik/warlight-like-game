using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquestObjectsLib
{
    public enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }

    abstract class Player
    {
        public virtual string Name { get; protected set; }
        public Color Color { get; set; }
    }

    class AIPlayer : Player
    {
        public Difficulty Difficulty { get; }

        public AIPlayer(Difficulty difficulty)
        {
            this.Difficulty = difficulty;
        }
    }

    class HumanPlayer : Player
    {
        public User User { get; }

        public override string Name
        {
            get { return User.Name; }
        }

        public HumanPlayer(User user)
        {
            this.User = user;
        }
    }
}
