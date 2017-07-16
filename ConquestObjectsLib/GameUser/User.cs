using System.Dynamic;

namespace ConquestObjectsLib.GameUser
{
    public enum UserType
    {
        None, Local, Network
    }
    /// <summary>
    /// Represents user that controls player in the game.
    /// </summary>
    public abstract class User
    {
        public string Name { get; set; }

        public abstract UserType UserType { get; }

        protected User(string name)
        {
            Name = name;
        }
        
    }
    
}
