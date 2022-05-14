using System;

namespace GameServer
{
    class ServerHelper
    {
        public static void Run()
        {
            bool run = true;
            while (run)
            {
                Console.Write(">");
                string line = Console.ReadLine();
                switch (line.ToLower().Trim()) //增加字符命令
                {
                    case "exit": run = false; break;
                    default: Help(); break;
                }
            }
        }

        public static void Help()
        {
            Console.Write
                (@"
Help:
    exit    Exit Game Server
    help    Show Help
                ");
        }
    }
}
