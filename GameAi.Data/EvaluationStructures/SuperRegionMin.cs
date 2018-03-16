namespace GameAi.Data.EvaluationStructures
{
    /// <summary>
    /// Represents minimized version of SuperRegion class for the purpose of calculation
    /// </summary>
    public struct SuperRegionMin
    {
        private class SuperRegionMinStatic
        {
            public int Id { get; internal set; }
            public string Name { get; internal set; }
            public int[] RegionsIds { get; set; }
            public int Bonus { get; }
            
            public SuperRegionMinStatic(int id, int bonus)
            {
                Id = id;
                Bonus = bonus;
            }
        }

        private SuperRegionMinStatic Static { get; }

        /// <summary>
        /// Id of this super region.
        /// </summary>
        public int Id
        {
            get { return Static.Id; }
            set
            {
                Static.Id = value;
            }
        }

        /// <summary>
        /// Gets or sets <see cref="SuperRegionMin"/> name.
        /// </summary>
        public string Name
        {
            get { return Static.Name; }
            set { Static.Name = value; }
        }

        /// <summary>
        /// Represents owner of this region.
        /// </summary>
        public byte OwnerId { get; set; }

        /// <summary>
        /// Represents bonus given to the owner of this region per round.
        /// </summary>
        public int Bonus
        {
            get { return Static.Bonus; }
        }

        /// <summary>
        /// Represents regions that belong to the given SuperRegion.
        /// </summary>
        public int[] RegionsIds
        {
            get { return Static.RegionsIds; }
            set { Static.RegionsIds = value; }
        }

        public SuperRegionMin(int superRegionId, int bonusArmy, byte owningPlayer = 0)
        {
            Static = new SuperRegionMinStatic(superRegionId, bonusArmy);

            OwnerId = owningPlayer;
        }

        public override string ToString()
        {
            return $"{Id}, {Name}";
        }
    }
}