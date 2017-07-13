namespace ConquestObjectsLib.GameMap
{
    /// <summary>
    /// Instace of this class represents region for given map in the game.
    /// </summary>
    class Region
    {
        public string Name { get; }
        /// <summary>
        /// Player owning given region. Null means no owner.
        /// </summary>
        public Player Owner { get; set; }
        /// <summary>
        /// Represents region group it belongs to.
        /// </summary>
        public RegionGroup RegionGroup { get; private set; }

        public Region(string name, RegionGroup regionGroup)
        {
            Name = name;
            RegionGroup = regionGroup;
        }
        
    }
}
