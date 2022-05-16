using Models;
using Network;
using Managers;
using UnityEngine;
using Charlotte.Proto;

namespace Services
{
    class ChatService : Singleton<ChatService>
    {
        public ChatService()
        {
            MessageDistributer.Instance.Subscribe<ChatResponse>(this.OnChat);  //发送聊天信息
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<ChatResponse>(this.OnChat);
        }

        public void Init()
        {

        }

        /// <summary>
        /// 发送聊天信息
        /// </summary>
        /// <param name="channel">聊天频道</param>
        /// <param name="content">聊天内容</param>
        /// <param name="toid">私聊者ID</param>
        /// <param name="toName">私聊者名称</param>
        public void SendChat(ChatChannel channel,string content,int toid,string toName)
        {
            Debug.Log("->SendChat");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.Chat = new ChatRequest();
            message.Request.Chat.Message = new ChatMessage();
            message.Request.Chat.Message.Channel = channel;
            message.Request.Chat.Message.FromId = User.Instance.CurrentCharacter.Id;
            message.Request.Chat.Message.FromName = User.Instance.CurrentCharacter.Name;
            message.Request.Chat.Message.ToId = toid;
            message.Request.Chat.Message.ToName = toName;
            message.Request.Chat.Message.Message = content;
            NetClient.Instance.SendMessage(message);
        }

        /// <summary>
        /// 聊天信息回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnChat(object sender, ChatResponse message)
        {
            if (message.Result == Result.Success)
            {
                ChatManager.Instance.AddMessages(ChatChannel.Local,message.locaMessages);
                ChatManager.Instance.AddMessages(ChatChannel.World, message.worldMessages);
                ChatManager.Instance.AddMessages(ChatChannel.System, message.systemMessages);
                ChatManager.Instance.AddMessages(ChatChannel.Private, message.privateMessages);
                ChatManager.Instance.AddMessages(ChatChannel.Team, message.teamMessages);
                ChatManager.Instance.AddMessages(ChatChannel.Guild, message.guildMessages);
            }
            else
                ChatManager.Instance.AddSystemMessage(message.Errorcode.ToString());
        }
    }
}
