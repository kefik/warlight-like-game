namespace WinformsUI.BotsPerformanceTesting
{
    using System;
    using System.IO;
    using GameObjectsLib.Players;

    public abstract class DuelBotPerformanceTest
    {
        protected TimeSpan First { get; }
        protected TimeSpan Second { get; }

        protected TextWriter ResultWriter { get; }

        protected AiPlayer FirstPlayer { get; set; }
        protected AiPlayer SecondPlayer { get; set; }

        protected DuelBotPerformanceTest(TimeSpan first, TimeSpan second,
            TextWriter resultWriter)
        {
            this.First = first;
            this.Second = second;
            ResultWriter = resultWriter;
        }
    }
}