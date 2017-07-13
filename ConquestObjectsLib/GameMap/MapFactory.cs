using ConquestObjectsLib.GameMap.Templates;

namespace ConquestObjectsLib.GameMap
{
    static class MapFactory
    {
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
