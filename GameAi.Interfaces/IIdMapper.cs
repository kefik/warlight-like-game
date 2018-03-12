namespace GameAi.Interfaces
{
    public interface IIdMapper
    {
        bool TryGetNewId(int originalId, out int mappedId);
        bool TryGetOriginalId(int mappedId, out int originalId);

        int GetOriginalId(int mappedId);
        int GetNewId(int originalId);
    }
}