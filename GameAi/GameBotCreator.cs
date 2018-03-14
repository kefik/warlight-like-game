namespace GameAi
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BotStructures;
    using BotStructures.AggressiveBot;
    using BotStructures.MCTS;
    using Data;
    using Data.EvaluationStructures;
    using Data.GameRecording;
    using Data.Restrictions;
    using FormatConverters;
    using GameObjectsLib;
    using GameObjectsLib.Game;
    using GameObjectsLib.GameRecording;
    using GameObjectsLib.Players;
    using Interfaces;

    internal class GameBotCreator
    {
        public IOnlineBot<BotTurn> Create(GameBotType gameBotType,
            MapMin map,
            Difficulty difficulty,
            byte playerEncoded, bool isFogOfWar,
            Restrictions restrictions)
        {
            PlayerPerspective playerPerspective =
                new PlayerPerspective(map, playerEncoded);
            InitializeVisibility(ref playerPerspective, isFogOfWar);

            switch (gameBotType)
            {
                case GameBotType.MonteCarloTreeSearchBot:
                    return new MonteCarloTreeSearchBot(playerPerspective, difficulty, isFogOfWar,
                        restrictions);
                case GameBotType.AggressiveBot:
                    return new AggressiveBot(playerPerspective, difficulty,
                        isFogOfWar, restrictions);
                default:
                    throw new ArgumentOutOfRangeException(nameof(gameBotType), gameBotType, null);
            }
        }

        /// <summary>
        /// Initializes visibility of all regions based on <see cref="PlayerPerspective.PlayerId"/>.
        /// </summary>
        /// <param name="playerPerspective"></param>
        /// <param name="isFogOfWar"></param>
        internal void InitializeVisibility(ref PlayerPerspective playerPerspective, bool isFogOfWar)
        {
            if (!isFogOfWar)
            {
                for (int index = 0; index < playerPerspective.MapMin.RegionsMin.Length; index++)
                {
                    playerPerspective.MapMin.RegionsMin[index].IsVisible = true;
                }
            }
            else
            {
                for (int index = 0; index < playerPerspective.MapMin.RegionsMin.Length; index++)
                {
                    ref var regionMin = ref playerPerspective.MapMin.RegionsMin[index];

                    // region is mine or neighbour to my region => it is visible
                    if (playerPerspective.IsRegionMine(regionMin)
                        || playerPerspective.IsNeighbourToAnyMyRegion(regionMin))
                    {
                        regionMin.IsVisible = true;
                    }
                    else
                    {
                        regionMin.IsVisible = false;
                    }
                }
            }
        }
    }
}