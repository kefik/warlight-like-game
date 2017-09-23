namespace GameObjectsLib
{
    /// <summary>
    ///     Provides ability to refresh the situation of given object.
    /// </summary>
    public interface IRefreshable
    {
        /// <summary>
        ///     Refreshes given game object.
        /// </summary>
        void Refresh();
    }

    /// <summary>
    ///     Provides ability to refresh the situation of given object based on parameter.
    /// </summary>
    /// <typeparam name="T">Type.</typeparam>
    public interface IRefreshable<in T>
    {
        /// <summary>
        ///     Refreshes object situation based on parameter.
        /// </summary>
        /// <param name="argument">Parameter.</param>
        void Refresh(T argument);
    }
}