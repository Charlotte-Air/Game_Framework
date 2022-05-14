namespace Network
{
    public interface INetSession //获取字节流接口
    {
        byte[] GetResponse();
    }

    public interface IPostResponser //后处理接口
    {
        void PostProcess(Charlotte.Proto.NetMessageResponse message);
    }
}
