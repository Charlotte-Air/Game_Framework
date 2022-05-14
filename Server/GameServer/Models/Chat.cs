using Charlotte.Proto;
using GameServer.Entities;
using GameServer.Managers;

namespace GameServer.Models
{
    class Chat
    {
        /// <summary>
        /// 所有者
        /// </summary>
        Character Owner;
        public int localIdx;
        public int worldIdx;
        public int systemIdx;
        public int teamIdx;
        public int guildIdx;

        public Chat(Character owner)
        {
            this.Owner = owner;
        }

        /// <summary>
        /// 后处理器事件
        /// </summary>
        /// <param name="message"></param>
        public void PostProcess(NetMessageResponse message)
        {
            if (message.Chat == null)
            {
                message.Chat = new ChatResponse();
                message.Chat.Result = Result.Success;
            }
            this.localIdx = ChatManager.Instance.GetLocalMessages(this.Owner.Info.mapId[0], this.localIdx, message.Chat.locaMessages);
            this.worldIdx = ChatManager.Instance.GetWorldMessages(this.worldIdx, message.Chat.worldMessages);
            this.systemIdx = ChatManager.Instance.GetSystemMessages(this.systemIdx, message.Chat.systemMessages);
            if (this.Owner.Team != null)
                this.teamIdx = ChatManager.Instance.GetTeamMessages(this.Owner.Team.Id,this.teamIdx, message.Chat.teamMessages);
            if (this.Owner.Guild != null)
                this.guildIdx = ChatManager.Instance.GetGuildMessages(this.Owner.Guild.id, this.guildIdx, message.Chat.guildMessages);
        }
    }
}
