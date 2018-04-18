#define DEBUG_CONSOLE
//#define PERFORMANCE_TESTING

#if !DEBUG
#undef DEBUG_CONSOLE
#endif

namespace WinformsUI
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;
    using BotsPerformanceTesting;

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
                writer.WriteLine($"Aggressive bot vs MCTS bot, time 5 seconds for move");
                MonteCarloTreeSearchVsAggressiveTest test =
                    new MonteCarloTreeSearchVsAggressiveTest(TimeSpan
                        .FromMilliseconds(5000), writer);
                var (mctsWinsCount, gamesCount) =
                    test.Run(TimeSpan.FromMinutes(5));

                writer.WriteLine();
                writer.WriteLine();
                writer.WriteLine($"MCTS win ratio: {mctsWinsCount} / {gamesCount}");
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
