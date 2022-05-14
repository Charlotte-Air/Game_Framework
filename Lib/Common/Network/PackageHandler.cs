using System;
using ProtoBuf;
using System.IO;
using Charlotte.Proto;

namespace Network
{
    /// <summary>
    /// 数据包处理器接口
    /// </summary>
    public class PackageHandler : PackageHandler<object>
    {
        public PackageHandler(object sender) : base(sender)
        {

        }
    }

    /// <summary>
    /// 数据包处理器
    /// </summary>
    /// <typeparam name="T">消息类型</typeparam>
    public class PackageHandler<T>
    {
        public PackageHandler(T sender) //包处理器
        {
            this.sender = sender; //发送者
        }

        private MemoryStream stream = new MemoryStream(64 * 1024); //字节流容量
        private int readOffset = 0; //读取偏移量
        private T sender; //发送者

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="data">字节流</param>
        /// <param name="offset">偏移量</param>
        /// <param name="count">数量</param>
        public void ReceiveData(byte[] data,int offset,int count)
        {
            if(stream.Position + count > stream.Capacity)
            {
                throw new Exception("PackageHandler write buffer overflow");
            }
            stream.Write(data, offset, count);
            ParsePackage();
        }

        /// <summary>
        /// 打包消息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static byte[] PackMessage(Charlotte.Proto.NetMessage message)
        {
            byte[] package = null;
            using (MemoryStream ms = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(ms, message);
                package = new byte[ms.Length + 4];
                Buffer.BlockCopy(BitConverter.GetBytes(ms.Length), 0, package, 0, 4);
                Buffer.BlockCopy(ms.GetBuffer(), 0, package, 4, (int)ms.Length);
            }
            return package;
        }

        /// <summary>
        /// 提取消息
        /// </summary>
        /// <param name="packet">字节流</param>
        /// <param name="offset">偏移量</param>
        /// <param name="length">长度</param>
        /// <returns></returns>
        public static Charlotte.Proto.NetMessage UnpackMessage(byte[] packet,int offset,int length)
        {
            Charlotte.Proto.NetMessage message = null;
            using (MemoryStream ms = new MemoryStream(packet, offset, length))
            {
                message = ProtoBuf.Serializer.Deserialize<Charlotte.Proto.NetMessage>(ms);
            }
            return message;
        }

        /// <summary>
        /// 解析数据包
        /// </summary>
        /// <returns></returns>
        bool ParsePackage()
        {
            if (readOffset + 4 < stream.Position)
            {
                int packageSize = BitConverter.ToInt32(stream.GetBuffer(), readOffset);
                if (packageSize + readOffset + 4 <= stream.Position)//包是否有效
                {
                    Charlotte.Proto.NetMessage message = UnpackMessage(stream.GetBuffer(), this.readOffset + 4, packageSize);
                    if (message == null)
                    {
                        throw new Exception("PackageHandler ParsePackage faild,invalid package");
                    }
                    MessageDistributer<T>.Instance.ReceiveMessage(this.sender, message);
                    this.readOffset += (packageSize + 4);
                    return ParsePackage();
                }
            }
            if (this.readOffset > 0)//如果未接收完
            {
                long size = stream.Position - this.readOffset;
                if (this.readOffset < stream.Position)
                {
                    Array.Copy(stream.GetBuffer(), this.readOffset, stream.GetBuffer(), 0, stream.Position - this.readOffset);
                }
                //重置流
                readOffset = 0;
                stream.Position = size;
                stream.SetLength(size);
            }
            return true;
        }
    }
}
