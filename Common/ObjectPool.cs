namespace Common
{
    using System.Collections.Concurrent;

    /// <summary>
    /// Pool whose purpose is to relieve the pressure on GC.
    /// </summary>
    /// <typeparam name="TType"></typeparam>
    public class ObjectPool<TType> where TType : class, new()
    {
        /// <summary>
        /// Free resources to be used by the pool.
        /// </summary>
        private readonly ConcurrentQueue<TType> freeResourcesQueue;

        public ObjectPool()
        {
            freeResourcesQueue = new ConcurrentQueue<TType>();
        }

        /// <summary>
        /// Represents default pool for given type.
        /// </summary>
        public static ObjectPool<TType> DefaultPool { get; } = new ObjectPool<TType>();

        /// <summary>
        /// Allocates an object of <see cref="TType"/> type and returns it.
        /// </summary>
        /// <returns></returns>
        public TType Allocate()
        {
            return freeResourcesQueue.TryDequeue(out var result) ? result : new TType();
        }

        /// <summary>
        /// Frees the object of <see cref="TType"/> type and adds it to the free resources in order to be used again.
        /// </summary>
        /// <param name="obj"></param>
        public void Free(TType obj)
        {
            freeResourcesQueue.Enqueue(obj);
        }
    }
}