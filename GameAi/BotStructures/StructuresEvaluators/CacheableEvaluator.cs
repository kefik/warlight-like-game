namespace GameAi.BotStructures.StructuresEvaluators
{
    using System.Collections.Generic;
    using Data.EvaluationStructures;

    internal abstract class CacheableEvaluator
    {
        /// <summary>
        /// Dictionary where the key is SuperRegion Id
        /// and the value is cached result of static evaluation.
        /// </summary>
        protected Dictionary<int, double> StaticCache
            = new Dictionary<int, double>();

        protected abstract void InitializeCache(MapMin map);
    }
}