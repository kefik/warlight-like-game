namespace GameHandlersLib.GameHandlers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GameObjectsLib.Game;
    using GameObjectsLib.GameMap;
    using GameObjectsLib.GameRecording;
    using GameObjectsLib.NetworkCommObjects;
    using MapHandlers;

    /// <summary>
    /// Handles playing game record.
    /// </summary>
    public class GameRecordHandler
    {
        private Game game;
        private readonly MapImageProcessor mapImageProcessor;

        private int currentRoundIndex;
        /// <summary>
        /// With currentRoundIndex 
        /// denotes first action that wasn't played so far.
        /// </summary>
        private int currentActionIndex;

        private IList<ILinearizedRound> Rounds
        {
            get { return game.AllRounds; }
        }
        
        public GameRecordHandler(MapImageProcessor mapImageProcessor)
        {
            this.mapImageProcessor = mapImageProcessor;
        }

        public void Load(Game game)
        {
            using (var ms = game.GetStreamForSerializedGame())
            {
                var copiedGame =
                    (Game)SerializationObjectWrapper.Deserialize(ms).Value;
                this.game = copiedGame;
                this.game.ReconstructOriginalGraph();
            }

            currentRoundIndex = Rounds.Count - 1;
            currentActionIndex = GetLastActionIndex(currentRoundIndex);
        }

        private int GetLastActionIndex(int roundIndex)
        {
            switch (Rounds[roundIndex])
            {
                case LinearizedGameBeginningRound round:
                    return round.SelectedRegions.Count - 1;
                case LinearizedGameRound round:
                    return round.Deploying.ArmiesDeployed.Count
                           + round.Attacking.Attacks.Count - 1;
                default:
                    return 0;
            }
        }

        private (int roundIndex, int actionIndex) GetIncrementedActionIndex(int roundIndex, int actionIndex)
        {
            var action = GetAction(roundIndex, actionIndex + 1);
            if (action == null)
            {
                var incAction = GetAction(roundIndex + 1, 0);

                if (incAction == null)
                {
                    throw new ArgumentOutOfRangeException();
                }

                return (roundIndex + 1, 0);
            }

            return (roundIndex, actionIndex + 1);
        }

        private (int roundIndex, int actionIndex) GetDecrementedActionIndex(int roundIndex, int actionIndex)
        {
            var action = GetAction(roundIndex, actionIndex - 1);
            if (action == null)
            {
                int lastActionIndex = GetLastActionIndex(roundIndex - 1);
                var incAction = GetAction(roundIndex - 1,
                    lastActionIndex);

                if (incAction == null)
                {
                    throw new ArgumentOutOfRangeException();
                }

                return (roundIndex - 1, lastActionIndex);
            }

            return (roundIndex, actionIndex - 1);
        }

        private IAction GetCurrentAction()
        {
            return GetAction(currentRoundIndex, currentActionIndex);
        }

        private IAction GetAction(int roundIndex, int actionIndex)
        {
            switch (Rounds[roundIndex])
            {
                case LinearizedGameBeginningRound round:
                    // out of range => null
                    if (actionIndex >= round.SelectedRegions.Count)
                    {
                        return null;
                    }
                    return round.SelectedRegions[actionIndex];
                case LinearizedGameRound round:
                {
                    if (round.Deploying.ArmiesDeployed.Count == actionIndex)
                    {
                        // can overflow from the other side
                        return round.Attacking.Attacks.Count == 0 ?
                                null : round.Attacking.Attacks[0];
                    }
                    if (round.Deploying.ArmiesDeployed.Count < actionIndex)
                    {
                        // attacks can overflow
                        return round.Attacking.Attacks.Count < actionIndex ?
                                null : round.Attacking.Attacks[actionIndex];
                    }

                    // count > actionIndex
                    return round.Deploying.ArmiesDeployed[actionIndex];
                }
                default:
                    return null;
            }
        }

        private IAction GetFirstPreviousActionOrDefault(Predicate<IAction> predicate)
        {
            IAction currentAction;
            (int roundIndex, int actionIndex) = GetDecrementedActionIndex(currentRoundIndex, currentActionIndex);
            while ((currentAction = GetAction(roundIndex, actionIndex)) != null)
            {
                if (predicate(currentAction))
                {
                    return currentAction;
                }
                (roundIndex, actionIndex)
                    = GetDecrementedActionIndex(roundIndex, actionIndex);
            }
            return null;
        }

        /// <summary>
        /// Moves perspective to the next action.
        /// </summary>
        public void MoveToNextAction()
        {
            MoveToNextActionPrivate();

            mapImageProcessor.RedrawMap(game, null);
        }

        private void MoveToNextActionPrivate()
        {
            IAction currentAction = GetCurrentAction();

            switch (currentAction)
            {
                case Deployment action:
                    action.Region.Army = action.Army;
                    break;
                case Attack action:
                    var mapChange = action.PostAttackMapChange;
                    action.Attacker.Army = mapChange.AttackingRegionArmy;
                    action.Defender.Army = mapChange.DefendingRegionArmy;
                    action.Defender.Owner = mapChange.DefendingRegionOwner;
                    break;
                case Seize action:
                    action.Region.Owner = action.SeizingPlayer;
                    break;
                default:
                    break;
            }

            var (roundIndex, actionIndex) = GetIncrementedActionIndex(currentRoundIndex, currentActionIndex);
            currentRoundIndex = roundIndex;
            currentActionIndex = actionIndex;
        }

        /// <summary>
        /// Moves record perspective to the previous action.
        /// </summary>
        public void MoveToPreviousAction()
        {
            MoveToPreviousActionPrivate();

            mapImageProcessor.RedrawMap(game, null);
        }

        private void MoveToPreviousActionPrivate()
        {
            // get first previous action that gives
            // us information about the current state
            var (roundIndex, actionIndex) = GetDecrementedActionIndex(currentRoundIndex, currentActionIndex);
            currentRoundIndex = roundIndex;
            currentActionIndex = actionIndex;

            var actionToRevert = GetCurrentAction();

            switch (actionToRevert)
            {
                case Deployment action:
                    RevertDeploymentAction(action);
                    break;
                case Attack action:
                    RevertAttackAction(action);
                    break;
                case Seize action:
                    // revert seize => seized owner was previously null
                    action.Region.Owner = null;
                    break;
            }
        }

        public void MoveToNextRound()
        {
            int roundIndex = currentRoundIndex;
            do
            {
                MoveToNextActionPrivate();
            } while (roundIndex != currentRoundIndex);
            
            mapImageProcessor.RedrawMap(game, null);
        }

        private void RevertDeploymentAction(Deployment deployment)
        {
            var concernedRegion = deployment.Region;

            RevertByFirstConcernedRegion(concernedRegion);
        }

        private void RevertAttackAction(Attack attack)
        {
            var attackingRegion = attack.Attacker;
            var defendingRegion = attack.Defender;

            // revert by both concerned regions
            RevertByFirstConcernedRegion(attackingRegion);
            RevertByFirstConcernedRegion(defendingRegion);
        }

        private void RevertByFirstConcernedRegion(Region concernedRegion)
        {
            // get first previous action that have contains information about concernedRegion
            var firstConcernedAction = GetFirstPreviousActionOrDefault(x => x.DoesConcernRegion(concernedRegion));
            switch (firstConcernedAction)
            {
                case Deployment action:
                    // revert to army that was there after the previous deploy
                    action.Region.Army = action.Army;
                    break;
                case Attack action:
                    // attacker => get army what was there after previous attacking
                    if (action.Attacker == concernedRegion)
                    {
                        action.Attacker.Army = action.PostAttackMapChange.AttackingRegionArmy;
                    }
                    // defender => get army that was there after previous defending
                    else if (action.Defender == concernedRegion)
                    {
                        action.Defender.Army
                            = action.PostAttackMapChange.DefendingRegionArmy;
                    }
                    else
                    {
                        // cannot happen
                        throw new ArgumentException();
                    }
                    break;
                case Seize action:
                    // revert seize => seized owner was previously null
                    action.Region.Owner = null;
                    break;
            }
        }
    }
}