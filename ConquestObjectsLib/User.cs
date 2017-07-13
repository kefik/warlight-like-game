using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquestObjectsLib
{
    /// <summary>
    /// Represents user that controls player in the game.
    /// </summary>
    class User
    {
        public string Name { get; set; }
    }
    
    /// <summary>
    /// Represents user account stored that will be stored on the server side as well.
    /// </summary>
    class GlobalUser : User
    {
        // TODO: Change properties
        public string Email { get; set; }
        public string PasswordHash { get; set; }
    }
}
