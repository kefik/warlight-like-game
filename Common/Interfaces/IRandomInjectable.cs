namespace Common.Interfaces
{
    using System;

    /// <summary>
    /// Class inheriting from this offers
    /// possibility to inject random
    /// (e.g. use for determinism in testing).
    /// </summary>
    public interface IRandomInjectable
    {
        /// <summary>
        /// Injects random into the instance.
        /// </summary>
        /// <param name="random"></param>
        void Inject(Random random);
    }
}