using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.WarlightLikeDatabase
{
    public abstract class NamedEntity : Entity
    {
        public virtual string Name { get; set; }
    }
}
