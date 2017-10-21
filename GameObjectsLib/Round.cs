﻿namespace GameObjectsLib
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    ///     Abstract predecessor of GameRound and GameBeginningRound.
    /// </summary>
    public abstract class Round
    {
        /// <summary>
        /// Validates the given round, returning exception if it is not valid.
        /// </summary>
        public abstract void Validate();

        /// <summary>
        /// Resets the round.
        /// </summary>
        public abstract void Reset();

        /// <summary>
        /// Processes the rounds, returning one linearized round.
        /// </summary>
        /// <param name="rounds">Rounds of consistent types (either all GameRound or all GameBeginningRound).</param>
        /// <returns></returns>
        public static Round Process(IList<Round> rounds)
        {
            var firstRound = rounds.FirstOrDefault();
            if (firstRound == null)
            {
                return null;
            }

            if (firstRound.GetType() == typeof(GameRound))
            {
                var convertedRounds = rounds.Cast<GameRound>().ToList();
                return GameRound.Process(convertedRounds);
            }
            else if (firstRound.GetType() == typeof(Round))
            {
                var convertedRounds = rounds.Cast<GameBeginningRound>().ToList();
                return GameBeginningRound.Process(convertedRounds);
            }
            else
            {
                throw new ArgumentException();
            }
        }
    }
}
