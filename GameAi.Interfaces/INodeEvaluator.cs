namespace GameAi.Interfaces
{
    public interface INodeEvaluator<in T>
    {
        double GetValue(T node);
    }
}