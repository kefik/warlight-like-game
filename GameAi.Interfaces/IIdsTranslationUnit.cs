namespace GameAi.Interfaces
{
    public interface IIdsTranslationUnit
    {
        bool TryGetNewId(int originalId, out int mappedId);
        bool TryGetOriginalId(int mappedId, out int originalId);
    }
}