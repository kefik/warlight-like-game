namespace WinformsUI.BotsPerformanceTesting
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using Client.Entities;
    using Common.Extensions;
    using GameHandlersLib;
    using GameHandlersLib.GameHandlers;
    using GameObjectsLib.Game;
    using GameObjectsLib.GameMap;
    using GameObjectsLib.GameRestrictions;
    using GameObjectsLib.Players;

    public class DuelBotPerformanceTest
    {
        protected TextWriter ResultWriter { get; }

        protected TimeSpan TimeForMove { get; }

        protected AiPlayer FirstPlayer { get; set; }
        protected AiPlayer SecondPlayer { get; set; }

        public DuelBotPerformanceTest(TimeSpan timeForMove, TextWriter resultWriter,
            AiPlayer firstPlayer, AiPlayer secondPlayer)
        {
            TimeForMove = timeForMove;
            ResultWriter = resultWriter;

            FirstPlayer = firstPlayer;
            SecondPlayer = secondPlayer;
        }

        public virtual (int FirstBotWinCount, int GamePlayedCount) Run(TimeSpan timeToRunTests)
        {
            Stopwatch stopwatch = new Stopwatch();
            int firstBotWinsCount = 0;
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
                        GameType.Simulator,
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
                        .StartOrContinueEvaluationAsync(TimeForMove)
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

                    // first is defeated => second must've won
                    if (players[0].IsDefeated(game.RoundNumber))
                    {
                        ResultWriter.WriteLine($"{((AiPlayer)players[1]).BotType.GetDisplayName()} WON");
                    }
                    else
                    {
                        ResultWriter.WriteLine($"{((AiPlayer)players[0]).BotType.GetDisplayName()} WON");
                        firstBotWinsCount++;
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

            return (firstBotWinsCount, gamesCount);
        }
    }
}