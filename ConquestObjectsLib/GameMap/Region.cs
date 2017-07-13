namespace ConquestObjectsLib.GameMap
{
    class Region
    {
        public string Name { get; }
        /// <summary>
        /// Player owning given region. Null means no owner.
        /// </summary>
        public Player Owner { get; set; }

        public Region(string name)
        {
            Name = name;
        }
    }
}
