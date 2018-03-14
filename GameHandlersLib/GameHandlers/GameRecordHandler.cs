namespace GameHandlersLib.GameHandlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using GameObjectsLib.Game;
    using GameObjectsLib.GameMap;
    using GameObjectsLib.GameRecording;
    using GameObjectsLib.NetworkCommObjects;
    using GameObjectsLib.Players;
    using MapHandlers;
    using IMapImageProcessor = MapHandlers.IMapImageProcessor;

    /// <summary>
    /// Handles playing game record.
    /// </summary>
    internal class GameRecordHandler
    {
        internal Game Game { get; private set; }
        internal Player PlayerPerspective { get; private set; }
        private readonly IMapImageProcessor mapImageProcessor;

        private ActionEnumerator currentActionEnumerator;

        internal IList<ILinearizedRound> Rounds
        {
            get { return Game.AllRounds; }
        }
        
        public GameRecordHandler(IMapImageProcessor mapImageProcessor,
            Game game, Player playerPerspective)
        {
            this.mapImageProcessor = mapImageProcessor;
            Load(game, playerPerspective);
        }

        /// <summary>
        /// Loads game, deep-copying it for the <see cref="GameRecordHandler"/>
        /// instance.
        /// </summary>
        /// <param name="game">Game to load.</param>
        /// <param name="playerPerspective">Player perspective.</param>
        public void Load(Game game, Player playerPerspective)
        {
            Game = game.DeepCopy();

            currentActionEnumerator = new ActionEnumerator(Game.AllRounds);
            PlayerPerspective = Game.Players.FirstOrDefault(x => x == playerPerspective);

            mapImageProcessor.RedrawMap(Game, playerPerspective);
        }

        private IAction GetFirstPreviousActionOrDefault(Predicate<IAction> predicate)
        {
            ActionEnumerator actionEnumerator = currentActionEnumerator;
            
            while (actionEnumerator.MovePrevious())
            {
                var currentAction = actionEnumerator.GetCurrentAction();
                if (predicate(currentAction) && currentAction.RanSuccessfully)
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

        public bool IsOnNewestPosition()
        {
            return GetCurrentAction() == null;
        }

        /// <summary>
        /// Moves perspective to the next action.
        /// </summary>
        public bool MoveToNextAction()
        {
            bool wasSuccessful;
            bool foundTheAction;
            ActionEnumerator tempEnumerator;
            do
            {
                // temp enumerator
                tempEnumerator = currentActionEnumerator;
                tempEnumerator.MoveNext();

                // get current action and whether it is related to players perspective
                foundTheAction =
                    PlayerPerspective == null
                    || tempEnumerator
                        .GetCurrentAction()
                        ?.IsCloseOrRelatedTo(PlayerPerspective) == true;

                // move to next action if there's any
                wasSuccessful = MoveToNextActionPrivate();

                // repeat while you can move to next action
                // and it does not concern our player perspective
                // only if it is fog of war
                // (skip moves you cannot see)
            } while (wasSuccessful && Game.IsFogOfWar && !foundTheAction);

            mapImageProcessor.RedrawMap(Game, PlayerPerspective);

            return wasSuccessful;
        }

        private bool MoveToNextActionPrivate()
        {
            if (!MoveToNextFirstValid())
            {
                return false;
            }
            IAction currentAction = currentActionEnumerator.GetCurrentAction();

            switch (currentAction)
            {
                case Deployment action:
                    action.Region.Army = action.Army;
                    break;
                case Attack action:
                    var mapChange = action.PostAttackMapChange;
                    action.Attacker.Army = mapChange.AttackingRegionArmy;
                    action.Defender.Army = mapChange.DefendingRegionArmy;
                    action.Defender.ChangeOwner(mapChange.DefendingRegionOwner);
                    break;
                case Seize action:
                    action.Region.ChangeOwner(action.SeizingPlayer);
                    break;
                default:
                    return false;
            }

            currentActionEnumerator.MoveNext();
            return true;
        }

        private bool MoveToNextFirstValid()
        {
            do
            {
                IAction currentAction = currentActionEnumerator
                    .GetCurrentAction();

                if (currentAction?.RanSuccessfully == true)
                {
                    return true;
                }
            } while (currentActionEnumerator.MoveNext());

            return false;
        }

        private bool MoveToPreviousFirstValid()
        {
            while (currentActionEnumerator.MovePrevious())
            {
                IAction currentAction = currentActionEnumerator
                    .GetCurrentAction();

                if (currentAction?.RanSuccessfully == true)
                {
                    return true;
                }
            };

            return false;
        }

        /// <summary>
        /// Moves record perspective to the previous action.
        /// </summary>
        public bool MoveToPreviousAction()
        {
            bool wasSuccessful;
            bool foundTheAction;
            ActionEnumerator tempEnumerator;
            do
            {
                // temp enumerator ... we need to have correct game context
                // for IsCloseOrRelated function
                tempEnumerator = currentActionEnumerator;
                tempEnumerator.MovePrevious();

                // get current action and whether it is related to players perspective
                foundTheAction = PlayerPerspective == null
                                           || tempEnumerator
                                               .GetCurrentAction()
                                               ?.IsCloseOrRelatedTo(PlayerPerspective) == true;

                // move to previous action
                wasSuccessful = MoveToPreviousActionPrivate();
                
                // repeat while you can move to previous action
                // and it does not concern our player perspective
                // only if it is fog of war
                // (skip moves you cannot see)
            } while (wasSuccessful && Game.IsFogOfWar && !foundTheAction);

            mapImageProcessor.RedrawMap(Game, PlayerPerspective);

            return wasSuccessful;
        }

        private bool MoveToPreviousActionPrivate()
        {
            // get first previous action that gives
            // us information about the current state

            // cannot move backwards => return false
            if (!MoveToPreviousFirstValid())
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
                    action.Region.ChangeOwner(null);
                    break;
                default:
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Moves current state of the game to the next round.
        /// </summary>
        /// <returns>
        /// True, if it successfully moved, false otherwise.
        /// </returns>
        public bool MoveToNextRound()
        {
            // moving to next round is successful
            // if at least one action has been moved
            bool wasSuccessful = MoveToNextRoundPrivate();

            mapImageProcessor.RedrawMap(Game, PlayerPerspective);

            return wasSuccessful;
        }

        private bool MoveToNextRoundPrivate()
        {
            // moving to next round is successful
            // if at least one action has been moved
            bool wasSuccessful = false;
            do
            {
                wasSuccessful |= MoveToNextActionPrivate();
                // action index == 0 => its beginning of the new round
                // => round has been reset
            } while (currentActionEnumerator.ActionIndex != 0);

            return wasSuccessful;
        }

        /// <summary>
        /// Moves current state of the game to the previous round.
        /// </summary>
        /// <returns>
        /// True, if it successfully moved, false otherwise.
        /// </returns>
        public bool MoveToPreviousRound()
        {
            // moving to previous round is successful
            // if at least one move was successful
            bool wasSuccessful = MoveToPreviousRoundPrivate();

            mapImageProcessor.RedrawMap(Game, PlayerPerspective);

            return wasSuccessful;
        }

        private bool MoveToPreviousRoundPrivate()
        {
            // moving to previous round is successful
            // if at least one move was successful
            bool wasSuccessful = false;
            do
            {
                wasSuccessful |= MoveToPreviousActionPrivate();
                // action index == 0 => its beginning of the round
                // => round has been reset
            } while (currentActionEnumerator.ActionIndex != 0);

            return wasSuccessful;
        }

        /// <summary>
        /// Moves the context to the beginning before the first
        /// round.
        /// </summary>
        /// <returns>True, if it does anything, otherwise false.</returns>
        public bool MoveToBeginning()
        {
            bool wasSuccessful;
            bool continueEvaluation = true;

            int i = 0;
            do
            {
                // move to previous round while function returns true
                continueEvaluation &= MoveToPreviousRoundPrivate();

                // first iteration and function returned false
                // => it wasnt successful
                if (i == 0 && !continueEvaluation)
                {
                    wasSuccessful = false;
                }
                else
                {
                    wasSuccessful = true;
                }
                i++;
            } while (continueEvaluation);

            mapImageProcessor.RedrawMap(Game, PlayerPerspective);

            return wasSuccessful;
        }

        /// <summary>
        /// Moves the context to the end after the last round.
        /// </summary>
        /// <returns>True, if it moves.</returns>
        public bool MoveToEnd()
        {
            bool wasSuccessful;
            bool continueEvaluation = true;

            int i = 0;
            do
            {
                // move to previous round while function returns true
                continueEvaluation &= MoveToNextRoundPrivate();

                // first iteration and function returned false
                // => it wasnt successful
                if (i == 0 && !continueEvaluation)
                {
                    wasSuccessful = false;
                }
                else
                {
                    wasSuccessful = true;
                }
                i++;
            } while (continueEvaluation);

            mapImageProcessor.RedrawMap(Game, PlayerPerspective);

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

                    // change owner
                    action.Region.ChangeOwner(action.DeployingPlayer);
                    break;
                case Attack action:
                    // attacker => get army what was there after previous attacking
                    if (action.Attacker == concernedRegion)
                    {
                        // revert army
                        action.Attacker.Army = action.PostAttackMapChange.AttackingRegionArmy;

                        // chage owner
                        action.Attacker.ChangeOwner(action.AttackingPlayer);
                    }
                    // defender => get army that was there after previous defending
                    else if (action.Defender == concernedRegion)
                    {
                        action.Defender.Army
                            = action.PostAttackMapChange.DefendingRegionArmy;

                        // change owner
                        action.Defender.ChangeOwner(action
                            .PostAttackMapChange.DefendingRegionOwner);
                    }
                    else
                    {
                        // cannot happen
                        throw new ArgumentException();
                    }
                    break;
                case Seize action:
                    // revert seize => seized owner was previously null
                    // TODO: default not fixed value
                    action.Region.Army = 2;

                    action.Region.ChangeOwner(action.SeizingPlayer);
                    break;
                default:
                    // there is no concerned action => set up default values
                    concernedRegion.Army = 2;

                    // if the region has owner => remove it from his ownership
                    concernedRegion.ChangeOwner(null);
                    break;
            }
        }
    }
}