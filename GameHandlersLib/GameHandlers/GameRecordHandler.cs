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
        /// <summary>
        /// Copied game instance for purpose of
        /// record handling.
        /// </summary>
        internal Game Game { get; private set; }
        /// <summary>
        /// Player from which the game is to be observed
        /// (or null == GOD).
        /// </summary>
        internal Player PlayerPerspective { get; private set; }

        private readonly IMapImageProcessor mapImageProcessor;

        private ActionEnumerator currentActionEnumerator;

        /// <summary>
        /// Rounds that can be traversed by this handler.
        /// </summary>
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

        /// <summary>
        /// Returns first action that is before current action enumerator
        /// position and satisfies specified predicate.
        /// </summary>
        /// <param name="predicate">Condition that action has to satisfy.</param>
        /// <returns>Action that satisfies the predicate or null.</returns>
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

        /// <summary>
        /// Obtains current action (meaning first not played).
        /// </summary>
        /// <returns></returns>
        internal IAction GetCurrentAction()
        {
            return currentActionEnumerator.GetCurrentAction();
        }

        /// <summary>
        /// Reports whether the record handler is
        /// on the newest position it can see.
        /// </summary>
        /// <returns></returns>
        public bool IsOnNewestPosition()
        {
            return GetCurrentAction() == null;
        }

        /// <summary>
        /// Moves perspective to the next action the given
        /// <see cref="PlayerPerspective"/> can observe.
        /// </summary>
        public bool MoveToNextAction()
        {
            bool wasSuccessful;
            bool foundTheAction;
            do
            {
                // get current action and whether it is related to players perspective
                // move to next action if there's any
                wasSuccessful = MoveToNextActionPrivate(out bool foundTheCorrectPlayerPerspectiveViewedAction);
                foundTheAction = foundTheCorrectPlayerPerspectiveViewedAction;

                // repeat while you can move to next action
                // and it does not concern our player perspective
                // only if it is fog of war
                // (skip moves you cannot see)
            } while (wasSuccessful && Game.IsFogOfWar && !foundTheAction);

            mapImageProcessor.RedrawMap(Game, PlayerPerspective);

            return wasSuccessful;
        }

        private bool MoveToNextActionPrivate(out bool foundThePlayerPerspectiveViewedAction)
        {
            if (!MoveToNextFirstValid())
            {
                foundThePlayerPerspectiveViewedAction = false;
                return false;
            }

            // get current action and whether it is related to players perspective
            // must be called before the playing itself so
            // attack defender has correct owner
            foundThePlayerPerspectiveViewedAction =
                PlayerPerspective == null
                || currentActionEnumerator
                    .GetCurrentAction()
                    ?.IsCloseOrRelatedTo(PlayerPerspective) == true;

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
        /// Moves perspective to the previous action the given
        /// <see cref="PlayerPerspective"/> can observe.
        /// </summary>
        public bool MoveToPreviousAction()
        {
            bool wasSuccessful;
            bool foundTheAction;
            do
            {
                // move to previous action
                wasSuccessful = MoveToPreviousActionPrivate(out bool foundTheCorrectAction);
                foundTheAction = foundTheCorrectAction;

                // repeat while you can move to previous action
                // and it does not concern our player perspective
                // only if it is fog of war
                // (skip moves you cannot see)
            } while (wasSuccessful && Game.IsFogOfWar && !foundTheAction);

            mapImageProcessor.RedrawMap(Game, PlayerPerspective);

            return wasSuccessful;
        }

        private bool MoveToPreviousActionPrivate(out bool foundTheCorrectPlayerPerspectiveViewedAction)
        {
            // cannot move backwards => return false
            if (!MoveToPreviousFirstValid())
            {
                foundTheCorrectPlayerPerspectiveViewedAction = false;
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
                    foundTheCorrectPlayerPerspectiveViewedAction = false;
                    return false;
            }

            // get first previous action that gives
            // us information about the current state
            // for IsCloseOrRelated function
            // get current action and whether it is related to players perspective
            // must be tested after reverting the last action
            // with to have correct attack defender owner (otherwise if
            // the defending region was conquered, owner would be incorrect)
            foundTheCorrectPlayerPerspectiveViewedAction = PlayerPerspective == null
                                                           || currentActionEnumerator
                                                               .GetCurrentAction()
                                                               ?.IsCloseOrRelatedTo(PlayerPerspective) == true;

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
            int roundIndex = currentActionEnumerator.RoundIndex;
            do
            {
                wasSuccessful |= MoveToNextActionPrivate(out _);
                // action index == 0 => its beginning of the new round
                // => round has been reset
                
                // last move of round is invalid => first valid is deploy
                // => gets played and moves action index to 1 =>
                // we have to revert it to the beginning of the round
                if (currentActionEnumerator.RoundIndex != roundIndex
                    && currentActionEnumerator.ActionIndex != 0)
                {
                    MoveToPreviousActionPrivate(out _);
                    break;
                }
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
                wasSuccessful |= MoveToPreviousActionPrivate(out _);
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

        /// <summary>
        /// Returns currently displayed round number.
        /// </summary>
        /// <returns></returns>
        public int GetDisplayedRoundNumber()
        {
            return currentActionEnumerator.RoundIndex;
        }

        /// <summary>
        /// Returns the game record to state like
        /// <see cref="deployment"/> never happened.
        /// </summary>
        /// <param name="deployment">Deployment to revert.</param>
        private void RevertDeploymentAction(Deployment deployment)
        {
            var concernedRegion = deployment.Region;

            RevertByFirstConcernedRegion(concernedRegion);
        }

        /// <summary>
        /// Returns the game record to the state like
        /// <see cref="attack"/> never happened.
        /// </summary>
        /// <param name="attack">Attack to revert.</param>
        private void RevertAttackAction(Attack attack)
        {
            var attackingRegion = attack.Attacker;
            var defendingRegion = attack.Defender;

            // revert by both concerned regions
            RevertByFirstConcernedRegion(attackingRegion);
            RevertByFirstConcernedRegion(defendingRegion);
        }

        /// <summary>
        /// Finds first previous entry specifying this
        /// region state and refreshes current state based
        /// on that information.
        /// </summary>
        /// <param name="concernedRegion"></param>
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