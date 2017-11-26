namespace GameAi
{
    using System.Linq;

    public struct Map
    {
        public RegionMin[] RegionsMin { get; internal set; }
        public SuperRegionMin[] SuperRegionsMin { get; internal set; }

        public Map(RegionMin[] regionsMin, SuperRegionMin[] superRegionsMin)
        {
            RegionsMin = regionsMin;
            SuperRegionsMin = superRegionsMin;
        }

        /// <summary>
        /// Reconstruct graph of objects, reconnecting all same objects so its represented only by one instance.
        /// </summary>
        public void ReconstructGraph()
        {
            foreach (var superRegion in SuperRegionsMin)
            {
                foreach (var superRegionRegion in superRegion.Regions)
                {
                    superRegionRegion.SuperRegion = superRegion;

                    for (int i = 0; i < RegionsMin.Length; i++)
                    {
                        if (RegionsMin[i] == superRegionRegion)
                        {
                            RegionsMin[i] = superRegionRegion;
                        }
                    }
                }
            }

            foreach (var region in RegionsMin)
            {
                for (int i = 0; i < region.NeighbourRegions.Length; i++)
                {
                    var realRegion = RegionsMin.First(x => x.Id == region.NeighbourRegions[i].Id);
                    region.NeighbourRegions[i] = realRegion;
                }
            }
        }
    }
}