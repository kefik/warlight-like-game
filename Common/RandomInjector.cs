namespace Common
{
    using System;
    using System.Collections.Generic;
    using Interfaces;

    /// <summary>
    /// Component which can inject random generators
    /// into <see cref="IRandomInjectable"/> instances.
    /// </summary>
    public class RandomInjector
    {
        /// <summary>
        /// Instance of random that will be injected
        /// to specified instances.
        /// </summary>
        private readonly Random random;

        public RandomInjector(Random random)
        {
            this.random = random;
        }

        /// <summary>
        /// Injects <see cref="random"/> into specified instances.
        /// </summary>
        /// <param name="injectables"></param>
        public void Inject(IEnumerable<IRandomInjectable> injectables)
        {
            foreach (IRandomInjectable randomInjectable in injectables)
            {
                randomInjectable.Inject(random);
            }
        }
    }
}