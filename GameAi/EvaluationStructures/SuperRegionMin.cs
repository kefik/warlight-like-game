namespace GameAi.EvaluationStructures
{
    using GameObjectsLib.GameMap;

    /// <summary>
    /// Represents minimized version of SuperRegion class for the purpose of calculation
    /// </summary>
    public struct SuperRegionMin
    {
        private class SuperRegionMinStatic
        {
            public int Id { get; internal set; }
            public int[] RegionsIds { get; set; }
            public int Bonus { get; }

            public SuperRegionMinStatic(SuperRegion superRegion)
            {
                Id = superRegion.Id;
                Bonus = superRegion.Bonus;
            }

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
            internal set
            {
                Static.Id = value;
            }
        }

        /// <summary>
        /// Represents owner of this region.
        /// </summary>
        public byte PlayerEncoded { get; set; }

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

        internal SuperRegionMin(SuperRegion superRegion, byte playerEncoded = 0)
        {
            PlayerEncoded = playerEncoded;

            Static = new SuperRegionMinStatic(superRegion);
        }

        public SuperRegionMin(int superRegionId, int bonusArmy)
        {
            Static = new SuperRegionMinStatic(superRegionId, bonusArmy);

            PlayerEncoded = 0;
        }

        public override string ToString()
        {
            return Id.ToString();
        }
    }
}