namespace WinformsUI.BotsPerformanceTesting
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Client.Entities;
    using FormatConverters;
    using GameAi;
    using GameAi.Data;
    using GameHandlersLib;
    using GameHandlersLib.GameHandlers;
    using GameObjectsLib.Game;
    using GameObjectsLib.GameMap;
    using GameObjectsLib.GameRestrictions;
    using GameObjectsLib.Players;

    public class MonteCarloTreeSearchVsAggressiveTest
        : DuelBotPerformanceTest
    {
        public MonteCarloTreeSearchVsAggressiveTest(
            TimeSpan monteCarloTreeSearchBotTimeSpan, TextWriter resultWriter) : base(
            new TimeSpan(0, 0, 0, 0, 100), monteCarloTreeSearchBotTimeSpan, resultWriter)
        {
            FirstPlayer = new AiPlayer(Difficulty.Hard, "Aggressive", KnownColor.Red, GameBotType.AggressiveBot);
            SecondPlayer = new AiPlayer(Difficulty.Hard, "MCTSBot", KnownColor.GreenYellow, GameBotType.MonteCarloTreeSearchBot);
        }

        public (int MCTSWinsCount, int GamesCount) Run(TimeSpan timeToRunTests)
        {
            Stopwatch stopwatch = new Stopwatch();
            int mctsWinCount = 0;
            int gamesCount = 0;

            stopwatch.Start();
            do
            {
                IList<Player> players =
                    new List<Player>()
                    {
                        new AiPlayer(FirstPlayer.Difficulty, FirstPlayer.Name, FirstPlayer.Color, FirstPlayer.BotType),
                        new AiPlayer(SecondPlayer.Difficulty, SecondPlayer.Name, SecondPlayer.Color, SecondPlayer.BotType)
                    };
                Game game;
                using (UtilsDbContext db = new UtilsDbContext())
                {
                    var mapInfo = db.Maps.First();

                    var map = new Map(mapInfo.Id, mapInfo.Name,
                        mapInfo.PlayersLimit, mapInfo.TemplatePath);

                    game = new GameFactory().CreateGame(
                        db.SimulationRecords.Select(x => x.Id)
                            .AsEnumerable()
                            .LastOrDefault() + 1, GameType.Simulator,
                        map, players, false,
                        new GameObjectsRestrictionsGenerator(map,
                            players, 2).Generate());
                }

                // establish log file
                string logFileName = $"{ConfigurationManager.AppSettings["SimulationRecordStoragePath"]}/{game.Id}.log";
                Debug.Listeners.Add(new TextWriterTraceListener(
                    new StreamWriter(logFileName, append: true)));


                BotEvaluationHandler botEvaluationHandler =
                    new BotEvaluationHandler(game);

                try
                {
                    botEvaluationHandler
                        .StartOrContinueEvaluationAsync(Second)
                        .Wait();
                }
                catch (AggregateException exc)
                {
                    var gameFinishedExc =
                        exc.InnerExceptions[0] as
                            GameFinishedException;

                    if (gameFinishedExc == null)
                    {
                        throw new ArgumentException(
                            "Different exc thrown => problem");
                    }

                    ResultWriter.Write($"GAME ID {game.Id}, Rounds = {game.RoundNumber}, ");
                    if (players[0].IsDefeated(game.RoundNumber))
                    {
                        ResultWriter.WriteLine($"MCTS bot");
                        mctsWinCount++;
                    }
                    else
                    {
                        ResultWriter.WriteLine($"Aggresssive bot");
                    }
                    gamesCount++;

                    // save game to the database
                    using (UtilsDbContext db = new UtilsDbContext())
                    {
                        game.Save(db);
                    }
                }
                finally
                {
                    // close log file
                    TraceListener lastListener =
                        Debug.Listeners[Debug.Listeners.Count - 1];
                    lastListener.Close();
                    Debug.Listeners.RemoveAt(
                        Debug.Listeners.Count - 1);
                }
            } while (stopwatch.Elapsed < timeToRunTests);

            stopwatch.Stop();

            return (mctsWinCount, gamesCount);
        }
    }
}