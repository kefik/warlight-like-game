namespace Server.UI
{
    using System;
    using System.Net;

    internal class Program
    {
        private static void Main(string[] args)
        {
            const string ipAddress = "127.0.0.1";
            Console.WriteLine(ipAddress);
            using (WarlightServer server = WarlightServer.Create(IPAddress.Parse(ipAddress), 5000))
            {
                server.Run(10);
            }
            //using (var db = new WarlightDbContext())
            //{
            //    foreach (var item in db.StartedGames)
            //    {
            //        Console.WriteLine(item);
            //    }
            //}
        }
    }
}
