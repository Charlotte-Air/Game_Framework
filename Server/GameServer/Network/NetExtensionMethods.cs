using System;
using System.Net.Sockets;

namespace GameServer.Network
{
    /// <summary>
    /// 表示Net3.5中一个新的Socket xxxAsync方法
    /// </summary>
    /// <param name="args"> SocketAsyncEventArgs与方法一起使用.</param>
    /// <returns>如果操作异步完成则返回true，否则返回false.</returns>
    public delegate Boolean SocketAsyncMethod(SocketAsyncEventArgs args);

    /// <summary>
    /// 保存helper方法、用于使用。Net3.5中的新Socket xxxAsync方法
    /// </summary>
    public static class NetExtensionMethods
    {
        /// <summary>
        /// 扩展方法来简化。Net3.5中新的Socket xxxAsync方法所需要的模式。
        /// </summary>
        /// <param name="socket">该方法所作用的套接字.</param>
        /// <param name="method">被调用的xxxAsync方法.</param>
        /// <param name="callback">方法的回调。注意:Completed事件必须已经被附加到相同的.</param>
        /// <param name="args">SocketAsyncEventArgs被用于此调用.</param>
        public static void InvokeAsyncMethod(this Socket socket, SocketAsyncMethod method, EventHandler<SocketAsyncEventArgs> callback, SocketAsyncEventArgs args)
        {
            if (!method(args))
            {
                callback(socket, args);
            }
        }
    }
}