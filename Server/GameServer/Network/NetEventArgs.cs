using System;
using System.Net;

namespace Network
{
    /// <summary>
    /// 字节流事件参数
    /// </summary>
    public class DataEventArgs : EventArgs
    {
        public IPEndPoint RemoteEndPoint { get; set; }
        public Byte[] Data { get; set; }
        public Int32 Offset { get; set; }
        public Int32 Length { get; set; }
    }
}
