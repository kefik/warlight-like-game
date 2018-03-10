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
    using IMapImageProcessor = MapHandlers.IMapImageProcessor;

    /// <summary>
    /// Handles playing game record.
    /// </summary>
    internal class GameRecordHandler
    {
        internal Game Game { get; private set; }
        private readonly IMapImageProcessor mapImageProcessor;

        private ActionEnumerator currentActionEnumerator;

        internal IList<ILinearizedRound> Rounds
        {
            get { return Game.AllRounds; }
        }
        
        public GameRecordHandler(IMapImageProcessor mapImageProcessor, Game game)
        {
            this.mapImageProcessor = mapImageProcessor;
            Load(game);
        }

        public void Load(Game game)
        {
            using (var ms = game.GetStreamForSerializedGame())
            {
                var copiedGame =
                    (Game)SerializationObjectWrapper.Deserialize(ms).Value;
                this.Game = copiedGame;
                this.Game.ReconstructOriginalGraph();
            }

            currentActionEnumerator = new ActionEnumerator(Game.AllRounds);
        }

        private IAction GetFirstPreviousActionOrDefault(Predicate<IAction> predicate)
        {
            ActionEnumerator actionEnumerator = currentActionEnumerator;
            
            while (actionEnumerator.MovePrevious())
            {
                var currentAction = actionEnumerator.GetCurrentAction();
                if (predicate(currentAction))
                {
                    return currentAction;
                }
            }
            return null;
        }

        internal IAction GetCurrentAction()
        {
            return currentActionEnumerator.GetCurrentAction();
        }

        /// <summary>
        /// Moves perspective to the next action.
        /// </summary>
        public bool MoveToNextAction()
        {
            bool wasSuccessful = MoveToNextActionPrivate();

            mapImageProcessor.RedrawMap(Game, null);

            return wasSuccessful;
        }

        private bool MoveToNextActionPrivate()
        {
            IAction currentAction = currentActionEnumerator.GetCurrentAction();

            if (currentAction == null)
            {
                return false;
            }

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
                    return false;
            }

            currentActionEnumerator.MoveNext();
            return true;
        }

        /// <summary>
        /// Moves record perspective to the previous action.
        /// </summary>
        public bool MoveToPreviousAction()
        {
            bool wasSuccessful = MoveToPreviousActionPrivate();

            mapImageProcessor.RedrawMap(Game, null);

            return wasSuccessful;
        }

        private bool MoveToPreviousActionPrivate()
        {
            // get first previous action that gives
            // us information about the current state

            // cannot move backwards => return false
            if (!currentActionEnumerator.MovePrevious())
            {
                return false;
            }

            var actionToRevert = currentActionEnumerator.GetCurrentAction();

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
                default:
                    return false;
            }
            return true;
        }

        public bool MoveToNextRound()
        {
            int roundIndex = currentActionEnumerator.RoundIndex;

            // moving to next round is successful
            // if at least one action has been moved
            bool wasSuccessful = false;
            do
            {
                wasSuccessful |= MoveToNextActionPrivate();
                // roundIndex == currentRoundIndex => moved the round
            } while (roundIndex != currentActionEnumerator.RoundIndex);
            
            mapImageProcessor.RedrawMap(Game, null);

            return wasSuccessful;
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
                        action.Defender.Owner = action.PostAttackMapChange.DefendingRegionOwner;
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