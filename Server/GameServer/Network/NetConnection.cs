using System;
using System.Net;
using System.Net.Sockets;

namespace Network
{
    /// <summary>
    /// Network连接
    /// </summary>
    public class NetConnection<T>where T : INetSession
    {

        #region Internal Classes
        internal class State
        {
            public DataReceivedCallback dataReceived;
            public DisconnectedCallback disconnectedCallback;
            public Socket socket;
        }
        #endregion

        /// <summary>
        /// 用于通知侦听器服务器连接已收到数据的回调
        /// </summary>
        /// <param name="sender">回调的发送方.</param>
        /// <param name="e">包含接收数据的DataEventArgs对象.</param>
        public delegate void DataReceivedCallback(NetConnection<T> sender, DataEventArgs socketEvent);

        /// <summary>
        /// 用于通知侦听器服务器连接已断开的回调
        /// </summary>
        /// <param name="sender">回调的发送方.</param>
        /// <param name="e">ServerConnection使用的SocketAsyncEventArgs对象.</param>
        public delegate void DisconnectedCallback(NetConnection<T> sender, SocketAsyncEventArgs socketEvent);

        #region Fields

        private SocketAsyncEventArgs eventArgs; //异步Socket

        /// <summary>
        /// 包处理器
        /// </summary>
        public PackageHandler<NetConnection<T>> packageHandler;

        #endregion

        #region Constructor
        /// <summary>
        /// 连接到服务器异步监听
        /// </summary>
        /// <param name="socket">连接的socket</param>
        /// <param name="args">用于异步接收的SocketAsyncEventArgs</param>
        /// <param name="dataReceived">接收数据时调用的回调函数</param>
        /// <param name="disconnectedCallback">断开连接时调用的回调函数</param>
        public NetConnection(Socket socket, SocketAsyncEventArgs args, DataReceivedCallback dataReceived,
            DisconnectedCallback disconnectedCallback, T session)
        {
            lock (this)
            {
                this.packageHandler = new Network.PackageHandler<NetConnection<T>>(this);
                State state = new State()
                {
                    socket = socket,
                    dataReceived = dataReceived,
                    disconnectedCallback = disconnectedCallback
                };
                eventArgs = new SocketAsyncEventArgs();
                eventArgs.AcceptSocket = socket;
                eventArgs.Completed += ReceivedCompleted;
                eventArgs.UserToken = state;
                eventArgs.SetBuffer(new byte[64 * 1024],0, 64 * 1024);
                BeginReceive(eventArgs);
                this.session = session;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// 断开客户端
        /// </summary>
        public void Disconnect()
        {
            lock (this)
            {
                CloseConnection(eventArgs);
            }
        }

        #endregion

        #region MyRegion
        /// <summary>
        /// 向客户端发送数据
        /// </summary>
        /// <param name="data">要发送的数据</param>
        /// <param name="offset">数据中的偏移量</param>
        /// <param name="count">发送的数据量</param>
        public void SendData(Byte[] data, Int32 offset, Int32 count)
        {
            lock (this)
            {
                State state = eventArgs.UserToken as State;
                Socket socket = state.socket;
                if (socket.Connected)
                {
                    //socket.Send(data, offset, count, SocketFlags.None);
                    socket.BeginSend(data, 0, count, SocketFlags.None, new AsyncCallback(SendCallback), socket);
                }
            }
        }

        #endregion

        #region SendResponse
        /// <summary>
        /// 向客户端发送响应
        /// </summary>
        public void SendResponse()
        {
            byte[] data = session.GetResponse();
            this.SendData(data, 0, data.Length);
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;
                // Complete sending the data to the remote device.
                int bytesSent = client.EndSend(ar);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// 启动异步接收
        /// </summary>
        /// <param name="args">使用的SocketAsyncEventArgs.</param>
        private void BeginReceive(SocketAsyncEventArgs args)
        {
            lock (this)
            {
                Socket socket = (args.UserToken as State).socket;
                if (socket.Connected)
                {
                    args.AcceptSocket.ReceiveAsync(args);
                    //socket.InvokeAsyncMethod(new SocketAsyncMethod(socket.ReceiveAsync),ReceivedCompleted, args);
                }
            }
        }

        /// <summary>
        /// 当异步接收完成时调用
        /// </summary>
        /// <param name="sender">发送者.</param>
        /// <param name="args">用于操作的SocketAsyncEventArgs.</param>
        private void ReceivedCompleted(Object sender, SocketAsyncEventArgs args)
        {
            if (args.BytesTransferred == 0)
            {
                CloseConnection(args); //Graceful disconnect
                return;
            }
            if (args.SocketError != SocketError.Success)
            {
                CloseConnection(args); //NOT graceful disconnect
                return;
            }

            State state = args.UserToken as State;

            Byte[] data = new Byte[args.BytesTransferred];
            Array.Copy(args.Buffer, args.Offset, data, 0, data.Length);
            OnDataReceived(data, args.RemoteEndPoint as IPEndPoint, state.dataReceived);

            BeginReceive(args);
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <param name="args">用于连接的SocketAsyncEventArgs.</param>
        private void CloseConnection(SocketAsyncEventArgs args)
        {
            State state = args.UserToken as State;
            Socket socket = state.socket;
            try
            {
                socket.Shutdown(SocketShutdown.Both);
            }
            catch 
            {

            }
            //客户端进程已经关闭则退出
            socket.Close();
            socket = null;
            args.Completed -= ReceivedCompleted; //注意必须执行这条语句注销异步回调
            OnDisconnected(args, state.disconnectedCallback);
        }
        #endregion

        #region Events
        /// <summary>
        /// 触发数据接收
        /// </summary>
        /// <param name="data">接收到的数据.</param>
        /// <param name="remoteEndPoint">数据地址与端口号</param>
        /// <param name="callback">侦听器服务器连接已收到数据的回调</param>
        private void OnDataReceived(Byte[] data, IPEndPoint remoteEndPoint, DataReceivedCallback callback)
        {
            callback(this, new DataEventArgs() 
            {
                RemoteEndPoint = remoteEndPoint, Data = data, Offset =0, Length = data.Length  
            });
        }

        /// <summary>
        /// 触发断开连接
        /// </summary>
        /// <param name="args">连接的套接字操作</param>
        /// <param name="callback">侦听器服务器连接已断开的回调</param>
        private void OnDisconnected(SocketAsyncEventArgs args, DisconnectedCallback callback)
        {
            callback(this, args);
        }

        #endregion

        #region public Property

        /// <summary>
        /// 获取或设置连接的认证状态
        /// [true] 已认证 /
        /// [false] 未认证
        /// </summary>
        public bool Verified { get; set; }

        private T session;
        /// <summary>
        /// 获取会话对象
        /// </summary>
        public T Session { get => session; }

        #endregion
    }
}
