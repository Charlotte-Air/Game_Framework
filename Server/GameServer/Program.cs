using System;
using Common;
using System.IO;

namespace GameServer
{
    class Program
    {
        static void Main(string[] args) 
        {
            FileInfo fi = new FileInfo("log4net.xml");
            log4net.Config.XmlConfigurator.ConfigureAndWatch(fi);
            Log.Init("GameServer");
            Log.Info("GameServer -> Init");
            GameServer server = new GameServer();
            server.Init();
            server.Start();
            Console.WriteLine("Game Server Running......");
            ServerHelper.Run();
            Log.Info("Game Server -> Exiting...");
            server.Stop();
            Log.Info("Game Server -> Exited");
        }
    }
}
