using System.IO;
using Newtonsoft.Json;

namespace Network
{
    class NetConfig
    {
        class NetConfigData
        {
            public string ServerIP { get; set; }
            public int ServerPort { get; set; }
            public string DBServerIP { get; set; }
            public int DBServerPort { get; set; }
            public string DBUser { get; set; }
            public string DBPass { get; set; }
        }

        static NetConfigData conig;
        public static string ServerIP { get => conig.ServerIP; }
        public static int ServerPort { get => conig.ServerPort; }
        public static string DBServerIP { get => conig.DBServerIP; }
        public static int DBServerPort { get => conig.DBServerPort; }
        public static string DBUser { get => conig.DBUser; }
        public static string DBPass { get => conig.DBPass; }

        public static void LoadConfig(string filename)
        {
            string json = File.ReadAllText(filename);
            conig = JsonConvert.DeserializeObject<NetConfigData>(json);
        }
    }
}
