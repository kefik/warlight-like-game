namespace GameHandlersLib.GameHandlers
{
    using System;
    using System.Collections.Generic;
    using GameObjectsLib.GameRecording;

    /// <summary>
    /// Represents two-way enumerator through all rounds actions.
    /// </summary>
    internal struct ActionEnumerator
    {
        /// <summary>
        /// Index of round enumerator is currently at.
        /// </summary>
        public int RoundIndex { get; private set; }
        /// <summary>
        /// Represents index of action the enumerator is currently at.
        /// Precisely it denotes first action that wasn't played.
        /// </summary>
        public int ActionIndex { get; private set; }

        /// <summary>
        /// Rounds of the game.
        /// </summary>
        private readonly IList<ILinearizedRound> rounds;
        
        public ActionEnumerator(IList<ILinearizedRound> rounds)
        {
            this.rounds = rounds;

            RoundIndex = rounds.Count;
            ActionIndex = 0;
        }

        /// <summary>
        /// Moves to the next action if possible.
        /// </summary>
        /// <returns>True, if it can be moved to the next action.</returns>
        public bool MoveNext()
        {
            var action = GetAction(RoundIndex, ActionIndex + 1);
            if (action == null)
            {
                var incAction = GetAction(RoundIndex + 1, 0);

                // RoundIndex + 1 == rounds.Count =>
                // special case, when I've played all moves
                if (incAction == null
                    && RoundIndex + 1 != rounds.Count)
                {
                    return false;
                }

                RoundIndex++;
                ActionIndex = 0;
                return true;
            }
            
            ActionIndex++;
            return true;
        }

        /// <summary>
        /// Moves to the previous action if possible.
        /// </summary>
        /// <returns>True, if it can be moved to the previous action.</returns>
        public bool MovePrevious()
        {
            var action = GetAction(RoundIndex, ActionIndex - 1);
            if (action == null)
            {
                int? lastActionIndex = GetLastActionIndex(RoundIndex - 1);

                // invalid last action index => cannot decrement
                if (lastActionIndex == null)
                {
                    return false;
                }

                var incAction = GetAction(RoundIndex - 1,
                    lastActionIndex.Value);

                if (incAction == null)
                {
                    return false;
                }

                RoundIndex--;
                ActionIndex = lastActionIndex.Value;
                return true;
            }
            ActionIndex--;
            return true;
        }

        /// <summary>
        /// Returns first action that wasn't played or null.
        /// </summary>
        /// <returns></returns>
        public IAction GetCurrentAction()
        {
            return GetAction(RoundIndex, ActionIndex);
        }

        private int? GetLastActionIndex(int roundIndex)
        {
            // invalid => report invalid by -1
            if (roundIndex < 0 || roundIndex >= rounds.Count)
            {
                return null;
            }
            switch (rounds[roundIndex])
            {
                case LinearizedGameBeginningRound round:
                    return round.SelectedRegions.Count - 1;
                case LinearizedGameRound round:
                    return round.Deploying.ArmiesDeployed.Count
                           + round.Attacking.Attacks.Count - 1;
                default:
                    throw new ArgumentException();
            }
        }

        private IAction GetAction(int roundIndex, int actionIndex)
        {
            // arguments out of range => return null
            if (roundIndex >= rounds.Count || roundIndex < 0
                || actionIndex < 0)
            {
                return null;
            }

            switch (rounds[roundIndex])
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
                        int attackIndex = actionIndex -
                                          round.Attacking.Attacks.Count;
                        // attacks can overflow
                        return round.Attacking.Attacks.Count <= attackIndex ?
                            null : round.Attacking.Attacks[attackIndex];
                    }

                    // count > actionIndex
                    return round.Deploying.ArmiesDeployed[actionIndex];
                }
                default:
                    return null;
            }
        }
    }
}