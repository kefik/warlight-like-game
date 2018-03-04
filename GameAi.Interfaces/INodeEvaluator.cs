namespace GameAi.Interfaces
{
    /// <summary>
    /// Component that can evaluate specified node.
    /// </summary>
    /// <typeparam name="TNode">Type of the node.</typeparam>
    public interface INodeEvaluator<in TNode>
    {
        /// <summary>
        /// Obtains value of the node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        double GetValue(TNode node);
    }
}