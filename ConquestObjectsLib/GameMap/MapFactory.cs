using ConquestObjectsLib.GameMap.Templates;

namespace ConquestObjectsLib.GameMap
{
    /// <summary>
    /// Factory for creating map from templates.
    /// </summary>
    public static class MapFactory
    {
        /// <summary>
        /// Method constructing map depending on the parameter.
        /// </summary>
        /// <param name="map">Decides what kind of template use to construct the map.</param>
        /// <returns>Map</returns>
        public static Map GetMap(MapType map)
        {
            switch (map)
            {
                case MapType.World:
                    return new World();
                default:
                    return null;
            }
        }
    }
}
