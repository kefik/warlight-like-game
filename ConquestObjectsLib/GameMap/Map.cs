using System.Collections.Generic;

namespace ConquestObjectsLib.GameMap
{
    enum MapType
    {
        World
    }
    abstract class Map
    {
        public string Name { get; }
        public ICollection<Region> Regions { get; protected set; }

        protected Map(string name)
        {
            this.Name = name;
        }
    }
}
