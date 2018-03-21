namespace GameAi.Data.EvaluationStructures
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class DistanceMatrix
    {
        /// <summary>
        /// (i, j) == path i to j, this array
        /// describes distance between i and j
        /// and previous node before j.
        /// matrix[i,j] means infinity.
        /// </summary>
        private (int Distance, int? IntermediateNodeIndex)[,] matrix;

        public DistanceMatrix(RegionMin[] regions)
        {
            Initialize(regions);
        }

        private void Initialize(RegionMin[] regions)
        {
            matrix = new (int Distance, int? Value)[regions.Length, regions.Length];

            for (int i = 0; i < regions.Length; i++)
            {
                for (int j = 0; j < regions.Length; j++)
                {
                    matrix[i, j] = (int.MaxValue / 3, null);
                }
            }

            // initialize to 0
            for (int i = 0; i < regions.Length; i++)
            {
                matrix[i, i] = (0, null);
            }

            // initialize to 1
            foreach (RegionMin region in regions)
            {
                foreach (var neighbourId in region.NeighbourRegionsIds)
                {
                    matrix[region.Id, neighbourId] = (1, null);
                    matrix[neighbourId, region.Id] = (1, null);
                }
            }

            // floyd-warshall
            for (int k = 0; k < regions.Length; k++)
            {
                for (int i = 0; i < regions.Length; i++)
                {
                    for (int j = 0; j < regions.Length; j++)
                    {
                        int previousValue = matrix[i, j].Distance;

                        matrix[i, j].Distance = Math.Min(matrix[i, j].Distance,
                            matrix[i, k].Distance + matrix[k, j].Distance);

                        if (previousValue != matrix[i, j].Distance)
                        {
                            // overwrite next node
                            matrix[i, j].IntermediateNodeIndex = k;
                        }

                        matrix[j, i] = matrix[i, j];
                    }
                }
            }
        }

        public IEnumerable<int> GetPath(RegionMin source, RegionMin destination)
        {
            int sourceId = source.Id;
            int destinationId = destination.Id;

            return GetPath(sourceId, destinationId).Distinct();
        }

        private IEnumerable<int> GetPath(int sourceId, int destinationId)
        {
            int? intermediate = matrix[sourceId, destinationId].IntermediateNodeIndex;
            if (intermediate == null)
            {
                return new List<int>()
                {
                    sourceId, destinationId
                };
            }
            else
            {
                var firstPart = GetPath(sourceId, intermediate.Value);
                var otherPart = GetPath(intermediate.Value, destinationId);

                return firstPart.Concat(otherPart);
            }
        }

        public int GetDistance(int firstRegionId, int secondRegionId)
        {
            return matrix[firstRegionId, secondRegionId].Distance;
        }

        public int GetMaximumDistance()
        {
            int maximumDistance = int.MinValue;
            foreach ((int distance, int? intermediateNodeIndex) in matrix)
            {
                if (distance > maximumDistance)
                {
                    maximumDistance = distance;
                }
            }

            return maximumDistance;
        }
    }
}