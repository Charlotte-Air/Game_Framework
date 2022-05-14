using GameServer;
using Charlotte.Proto;
using GameServer.Entities;
using GameServer.Services;

namespace Network
{   
    class NetSession : INetSession
    {
        /// <summary>
        /// DB User缓存
        /// </summary>
        public TUser User { get; set; }

        /// <summary>
        /// DB 角色缓存
        /// </summary>
        public Character Character { get; set; }

        /// <summary>
        /// DB 实体缓存
        /// /summary>
        public NEntity Entity { get; set; }

        /// <summary>
        /// 后处理器响应
        /// </summary>
        public IPostResponser PostResponser { get; set; }

        internal void Disconnected()
        {
            this.PostResponser = null;
            if (this.Character != null)
                UserService.Instance.CharacterLeave(this.Character);
        }

        NetMessage response;
        /// <summary>
        /// Response创建
        /// </summary>
        public NetMessageResponse Response
        {
            get
            {
                if (response == null)
                    response = new NetMessage();
                if (response.Response == null)
                    response.Response = new NetMessageResponse();
                return response.Response;
            }
        }

        /// <summary>
        /// 获取Response字节流
        /// </summary>
        public byte[] GetResponse()
        {
            if (response != null)
            {
                if (PostResponser != null) //后处理器工作
                {
                    this.PostResponser.PostProcess(Response);
                }
                byte[] data = PackageHandler.PackMessage(response); //打包数据
                this.response = default;
                return data;
            }
            return null;
        }
    }
}
