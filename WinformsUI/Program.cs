#define DEBUG_CONSOLE
//#define PERFORMANCE_TESTING

#if !DEBUG
#undef DEBUG_CONSOLE
#endif

namespace WinformsUI
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;
    using BotsPerformanceTesting;
    using GameAi.Data;
    using GameObjectsLib.Players;

    internal static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
#if DEBUG_CONSOLE
            if (!Debugger.IsAttached)
            {
                AllocConsole();
                Debug.Listeners.Add(new ConsoleTraceListener());
            }
#endif

#if !PERFORMANCE_TESTING
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainGameForm());
#endif

#if PERFORMANCE_TESTING
            using (var writer = new StreamWriter("TEST_RESULT.log"))
            {
                writer.WriteLine($"MCTS bot vs aggressive bot, time 5 seconds for move");
                DuelBotPerformanceTest test =
                    new DuelBotPerformanceTest(TimeSpan
                        .FromMilliseconds(5000), writer,
                        new AiPlayer(Difficulty.Hard, "MCTSBot", KnownColor.GreenYellow, GameBotType.MonteCarloTreeSearchBot),
                        new AiPlayer(Difficulty.Hard, "Aggressive", KnownColor.Red, GameBotType.AggressiveBot));
                var (mctsWinCount, gamesCount) =
                    test.Run(TimeSpan.FromMinutes(6));
                writer.WriteLine();
                writer.WriteLine($"MCTS win ratio: {mctsWinCount} / {gamesCount}");
                writer.WriteLine();
                writer.WriteLine();

                writer.WriteLine($"MCTS bot vs Smart random bot, time 5 seconds for move");
                var test2 = new DuelBotPerformanceTest(TimeSpan
                            .FromMilliseconds(5000), writer,
                        new AiPlayer(Difficulty.Hard, "MCTSBot", KnownColor.GreenYellow, GameBotType.MonteCarloTreeSearchBot),
                        new AiPlayer(Difficulty.Hard, "Smart random bot", KnownColor.Red, GameBotType.SmartRandomBot));
                var (mctsWinCount2, gamesCount2) =
                    test2.Run(TimeSpan.FromMinutes(8));
                writer.WriteLine();
                writer.WriteLine($"MCTS win ratio: {mctsWinCount2} / {gamesCount2}");
                writer.WriteLine();
                writer.WriteLine();
            }


#endif
        }

#if DEBUG_CONSOLE

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AllocConsole();
#endif
    }
}
