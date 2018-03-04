namespace GameAi.Interfaces
{
    /// <summary>
    /// Class inheriting from this interface represents restriction used 
    /// for bot evaluation.
    /// </summary>
    public interface IRestriction
    {
        int PlayerId { get; set; }
    }
}