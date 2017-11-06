namespace GameHandlersLib.GameHandlers
{
    using System.Linq;
    using GameObjectsLib;
    using GameObjectsLib.GameRecording;
    using GameObjectsLib.Players;

    internal static class PlayerExtensions
    {
        /// <summary>
        /// Reports whether player is defeated.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public static bool IsDefeated(this Player player, GameState state)
        {
            return player.ControlledRegions.Count == 0 && state != GameState.GameBeginning;
        }

        /// <summary>
        /// Reports how much army can player deploy based on what he has deployed already.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="deploying"></param>
        /// <returns></returns>
        public static int GetArmyLeftToDeploy(this Player player, Deploying deploying)
        {
            int deployedUnitsSum = (from deployment in deploying.ArmiesDeployed
                                    where deployment.Region.Owner == player
                                    select deployment.Army - deployment.Region.Army).Sum();
            return player.GetIncome() - deployedUnitsSum;
        }
    }
}