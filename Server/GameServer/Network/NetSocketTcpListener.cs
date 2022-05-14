using System;
using System.Net;
using System.Net.Sockets;

namespace Network
{
    /// <summary>
    /// Socket/TCP监听
    /// </summary>
    public class NetSocketTcpListener : IDisposable
    {
        #region Fields
        private IPEndPoint endPoint;
        private Socket listenerSocket; 
        private Int32 connectionBacklog;
        private SocketAsyncEventArgs args;
        #endregion

        #region Properties
        /// <summary>
        /// 连接积压的长度
        /// </summary>
        public Int32 ConnectionBacklog
        {
            get { return connectionBacklog; }
            set
            {
                lock (this)
                {
                    if (IsRunning)
                        throw new InvalidOperationException("Property cannot be changed while server running.");
                    else
                        connectionBacklog = value;
                }
            }
        }
        /// <summary>
        /// 要绑定监听Socket的IPEndPoint
        /// </summary>
        public IPEndPoint EndPoint
        {
            get { return endPoint; }
            set
            {
                lock (this)
                {
                    if (IsRunning)
                        throw new InvalidOperationException("Property cannot be changed while server running.");
                    else
                        endPoint = value;
                }
            }
        }
        /// <summary>
        /// 是否在监听
        /// </summary>
        public Boolean IsRunning
        {
            get { return listenerSocket != null; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// 监听地址与端口的Socket连接
        /// </summary>
        /// <param name="address">要监听的地址.</param>
        /// <param name="port">监听的端口.</param>
        /// <param name="connectionBacklog">连接backlog.</param>
        public NetSocketTcpListener(String address, Int32 port, Int32 connectionBacklog):this(IPAddress.Parse(address), port, connectionBacklog){ }

        /// <summary>
        /// 监听地址与端口的Socket连接
        /// </summary>
        /// <param name="address">要监听的地址.</param>
        /// <param name="port">监听的端口.</param>
        /// <param name="connectionBacklog">连接backlog.</param>
        public NetSocketTcpListener(IPAddress address, Int32 port, Int32 connectionBacklog):this(new IPEndPoint(address, port), connectionBacklog){ }

        /// <summary>
        /// 监听地址与端口的Socket连接
        /// </summary>
        /// <param name="endPoint">要监听的端点.</param>
        /// <param name="connectionBacklog">连接backlog.</param>
        public NetSocketTcpListener(IPEndPoint endPoint, Int32 connectionBacklog)
        {
            this.endPoint = endPoint;
            args = new SocketAsyncEventArgs();
            args.Completed += OnSocketAccepted;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// 开始监听Socket连接
        /// </summary>
        public void Start()
        {
            lock (this)
            {
                if (!IsRunning)
                {
                    listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    listenerSocket.Bind(endPoint);
                    listenerSocket.Listen(connectionBacklog);
                    BeginAccept(args);
                }
                else
                    throw new InvalidOperationException("The Server is already running.");
            }

        }

        /// <summary>
        /// 停止侦听Socket连接
        /// </summary>
        public void Stop()
        {
            lock (this)
            {
                if (listenerSocket == null)
                {
                    return;
                }
                listenerSocket.Close();
                listenerSocket = null;
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// 异步监听连接
        /// </summary>
        /// <param name="args"></param>
        private void BeginAccept(SocketAsyncEventArgs args)
        {
            args.AcceptSocket = null;
            listenerSocket.AcceptAsync(args);
            //listenerSocket.InvokeAsyncMethod(new SocketAsyncMethod(listenerSocket.AcceptAsync),OnSocketAccepted, args);
        }

        /// <summary>
        /// 异步处理完时调用
        /// </summary>
        /// <param name="sender">发送者.</param>
        /// <param name="socketEvent">用于操作的异步套接字</param>
        private void OnSocketAccepted(object sender, SocketAsyncEventArgs socketEvent)
        {
            SocketError error = socketEvent.SocketError;
            if (socketEvent.SocketError == SocketError.OperationAborted)
            {
                return; //Server was stopped
            }
            if (socketEvent.SocketError == SocketError.Success)
            {
                Socket handler = socketEvent.AcceptSocket;
                OnSocketConnected(handler);
            }
            lock (this)
            {
                BeginAccept(socketEvent);
            }
        }
        #endregion

        #region Events

        /// <summary>
        /// 接收到连接时触发事件
        /// </summary>
        public event EventHandler<Socket> SocketConnected;

        /// <summary>
        /// 触发Socket连接事件
        /// </summary>
        /// <param name="client">客户端</param>
        private void OnSocketConnected(Socket client)
        {
            if (SocketConnected != null)
            {
                SocketConnected(this, client);
            }
        }

        #endregion

        #region IDisposable Members

        private Boolean disposed = false;

        ~NetSocketTcpListener()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    Stop();
                    if (args != null)
                    {
                        args.Dispose();
                    }
                }
                disposed = true;
            }
        }
        #endregion
    }
}
