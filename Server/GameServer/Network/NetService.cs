using Common;
using System.Net;
using System.Net.Sockets;

namespace Network
{
    class NetService
    {
        static NetSocketTcpListener ServerListener;
        public bool Init(int port)
        {
            ServerListener = new NetSocketTcpListener("127.0.0.1", GameServer.Properties.Settings.Default.ServerPort, 10);
            ServerListener.SocketConnected += OnSocketConnected;
            return true;
        }

        public void Start()
        {
            Log.Warning("Starting Listener..."); //启动监听
            ServerListener.Start();
            MessageDistributer<NetConnection<NetSession>>.Instance.Start(8);
            Log.Warning("NetService -> Started");
        }

        public void Stop()
        {
            Log.Warning("Stop NetService...");
            ServerListener.Stop();
            Log.Warning("Stoping Message Handler...");
            MessageDistributer<NetConnection<NetSession>>.Instance.Stop();
        }

        private void OnSocketConnected(object sender, Socket socket)
        {
            IPEndPoint clientIP = (IPEndPoint)socket.RemoteEndPoint;
            //****************一级验证、黑名单处理************************


            //************************************************************
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            NetSession session = new NetSession();
            NetConnection<NetSession> connection = new NetConnection<NetSession>
            (
                socket,
                args,
                new NetConnection<NetSession>.DataReceivedCallback(DataReceived),
                new NetConnection<NetSession>.DisconnectedCallback(Disconnected), 
                session
            );
            Log.WarningFormat("Client[{0}]] Connected", clientIP);
        }


        /// <summary>
        /// 连接断开回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void Disconnected(NetConnection<NetSession> sender, SocketAsyncEventArgs socket)
        {
            //Performance.ServerConnect = Interlocked.Decrement(ref Performance.ServerConnect);
            sender.Session.Disconnected();
            Log.WarningFormat("Client[{0}] Disconnected", socket.RemoteEndPoint);
        }


        /// <summary>
        /// 接受数据回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void DataReceived(NetConnection<NetSession> sender, DataEventArgs dataEvent)
        {
            Log.WarningFormat("Client[{0}] DataReceived Len:{1}", dataEvent.RemoteEndPoint, dataEvent.Length);
            lock (sender.packageHandler) //由包处理器处理封包
            {
                sender.packageHandler.ReceiveData(dataEvent.Data, 0, dataEvent.Data.Length);
            }
            //PacketsPerSec = Interlocked.Increment(ref PacketsPerSec);
            //RecvBytesPerSec = Interlocked.Add(ref RecvBytesPerSec, e.Data.Length);
        }
    }
}
