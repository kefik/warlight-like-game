namespace GameObjectsLib
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GameMap;
    using ProtoBuf;

    /// <summary>
    ///     Represents deploying phase of the game.
    /// </summary>
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    public struct Deploying
    {
        /// <summary>
        ///     Represents armies deployed in the deploying phase in given regions.
        ///     Int represents armies that will be occuppying this region after this stage.
        /// </summary>
        public List<Deployment> ArmiesDeployed { get; }

        public Deploying(List<Deployment> armiesDeployed)
        {
            ArmiesDeployed = armiesDeployed;
        }

        /// <summary>
        ///     Calculates how many units can given player deploy to fulfill his maximum.
        /// </summary>
        /// <param name="player">Given player.</param>
        /// <returns>Units left to deploy for given player.</returns>
        public int GetUnitsLeftToDeploy(Player.Player player)
        {
            int income = player.GetIncome();
            int alreadyDeployed = (from deploy in ArmiesDeployed
                                   where deploy.Region.Owner == player
                                   select deploy.Army - deploy.Army).Sum();
            return income - alreadyDeployed;
        }

        /// <summary>
        /// Reports whether exists deployment with the specified region.
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        private bool ContainsDeploymentWithThisRegion(Region region)
        {
            return ArmiesDeployed.Any(x => x.Region == region);
        }

        /// <summary>
        /// Returns deployment with the specified region.
        /// </summary>
        /// <param name="region"></param>
        /// <param name="deployment"></param>
        /// <returns></returns>
        private bool TryGetDeploymentWithThisRegion(Region region, out Deployment deployment)
        {
            bool existsDeployment = ArmiesDeployed.Any(x => x.Region == region);
            deployment = !existsDeployment ? default(Deployment) : ArmiesDeployed.First(x => x.Region == region);
            return ContainsDeploymentWithThisRegion(region);
        }

        /// <summary>
        /// Adds deployment to the deployed list.
        /// </summary>
        /// <param name="region"></param>
        /// <param name="newArmy">Army that will be added to the deployment.</param>
        public void AddDeployment(Region region, int newArmy)
        {
            if (TryGetDeploymentWithThisRegion(region, out Deployment deployment))
            {
                ArmiesDeployed.Remove(deployment);
            }
            ArmiesDeployed.Add(new Deployment(region, newArmy));
        }

        /// <summary>
        /// Resets everything that has been deployed.
        /// </summary>
        public void ResetDeploying()
        {
            ArmiesDeployed?.Clear();
        }
    }
}
