using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquestObjectsLib
{
    class User
    {
        public string Name { get; set; }
    }
    

    class GlobalUser : User
    {
        public string Email { get; set; }
        public string PasswordHash { get; set; }
    }
}
