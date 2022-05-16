using Models;
using Services;
using System.Text;
using UnityEngine;
using Charlotte.Proto;
using UnityEngine.Events;
using System.Collections.Generic;

namespace Managers
{
    /// <summary>
    /// 聊天管理器
    /// </summary>
    class ChatManager : Singleton<ChatManager>
    {
        /// <summary>
        /// 频道类型
        /// </summary>
        public enum LocalChannel
        {
            All = 0,          //所有
            Local = 1,      //本地
            World = 2,    //世界
            Team = 3,     //队伍
            Guild = 4,	 //公会
            Private = 5,   //私聊
        }
        
        /// <summary>
        /// 聊天内容更新
        /// </summary>
        public UnityAction OnChat { get; internal set; }
        /// <summary>
        /// 私聊者ID
        /// </summary>
        public int PrivateID = 0;
        /// <summary>
        /// 私聊者名称
        /// </summary>
        public string PrivateName = "";

        public LocalChannel displayChannel;
        public LocalChannel sendChannel;
        public ChatChannel SendChannel
        {
            get
            {
                switch (sendChannel)
                {
                    case LocalChannel.Local: return ChatChannel.Local;
                    case LocalChannel.World: return ChatChannel.World;
                    case LocalChannel.Team: return ChatChannel.Team;
                    case LocalChannel.Guild: return ChatChannel.Guild;
                    case LocalChannel.Private: return ChatChannel.Private;
                }
                return ChatChannel.Local;
            }
        }

        private ChatChannel[] ChannelFilter = new ChatChannel[6]
        {
            ChatChannel.Local | ChatChannel.World | ChatChannel.Guild | ChatChannel.Team | ChatChannel.Private | ChatChannel.System,
            ChatChannel.Local,
            ChatChannel.World,
            ChatChannel.Team,
            ChatChannel.Guild,
            ChatChannel.Private
        };

        public List<ChatMessage>[] Messages = new List<ChatMessage>[6]
        {
            new List<ChatMessage>(),
            new List<ChatMessage>(),
            new List<ChatMessage>(),
            new List<ChatMessage>(),
            new List<ChatMessage>(),
            new List<ChatMessage>()
        };

        public void  Init()
        {
            foreach (var messages in this.Messages)
            {
                messages.Clear();
            }
        }

        /// <summary>
        /// 发送聊天
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="toId">私聊ID</param>
        /// <param name="toName">私聊名称</param>
        public void SendChat(string content, int toId = 0, string toName = "")
        {
            ChatService.Instance.SendChat(this.SendChannel, content, toId, toName);
        }

        /// <summary>
        /// 设置发送频道
        /// </summary>
        /// <param name="channel">频道类型</param>
        /// <returns></returns>
        public bool SetSendChannel(LocalChannel channel)
        {
            if (channel == LocalChannel.Team)
            {
                if (User.Instance.TeamInfo == null)
                {
                    this.AddSystemMessage("你没有加入任何队伍");
                    return false;
                }
            }
            if (channel == LocalChannel.Guild)
            {
                if (GuildManager.Instance.guildInfo == null && GuildManager.Instance.myMemberInfo == null)
                {
                    this.AddSystemMessage("你没有加入任何公会");
                    return false;
                }
            }
            this.sendChannel = channel;
            Debug.LogFormat("Set->Channel [{0}]", this.sendChannel);
            return true;
        }

        /// <summary>
        /// 发起私聊 
        /// </summary>
        /// <param name="targetId">私聊ID</param>
        /// <param name="targetName">私聊名字</param>
        internal void StartPrivateChat(int targetId, string targetName)
        {
            this.PrivateID = targetId;
            this.PrivateName = targetName;
            this.sendChannel = LocalChannel.Private;
            if (this.OnChat != null)
                this.OnChat();
        }

        /// <summary>
        /// 添加聊天消息
        /// </summary>
        /// <param name="channel">频道类型</param>
        /// <param name="messages">消息列表</param>
        internal void AddMessages(ChatChannel channel,List<ChatMessage> messages)
        {
            for (int ch = 0; ch < 6 ;ch++) //遍历每个频道
            {
                if ((this.ChannelFilter[ch] & channel) == channel)  //可使用过滤器，过滤频道
                    this.Messages[ch].AddRange(messages);
            }
            if (this.OnChat != null)
                this.OnChat();
        }

        /// <summary>
        /// 添加系统消息
        /// </summary>
        /// <param name="message">内容</param>
        /// <param name="from"></param>
        public void AddSystemMessage(string message, string from = "")
        {
            this.Messages[(int)LocalChannel.All].Add(new ChatMessage()
                {
                    Channel = ChatChannel.System,
                    Message = message,
                    FromName = from
                }
            );
            if (OnChat != null)
                this.OnChat();
        }

        /// <summary>
        /// 获取当前消息
        /// </summary>
        /// <returns>消息内容</returns>
        public string GetCurrentMessages()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var message in this.Messages[(int)displayChannel])
            {
                sb.AppendLine(FormatMessage(message));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 聊天清空
        /// </summary>
        public void ChatClear()
        {
            foreach (var messages in this.Messages)
            {
                messages.Clear();
            }
        }

        /// <summary>
        /// 聊天消息处理
        /// </summary>
        /// <param name="message">ChatMessage</param>
        /// <returns></returns>
        private string FormatMessage(ChatMessage message)
        {
            switch (message.Channel)
            {
                case ChatChannel.Local:
                    return string.Format("[本地]{0}{1}", FormatFromPlayer(message), message.Message);
                case ChatChannel.World:
                    return string.Format("[世界]{0}{1}", FormatFromPlayer(message), message.Message);
                case ChatChannel.System:
                    return string.Format("[系统]{0}", message.Message);
                case ChatChannel.Private:
                    return string.Format("[私聊]{0}{1}", FormatFromPlayer(message), message.Message);
                case ChatChannel.Team:
                    return string.Format("[队伍]{0}{1}", FormatFromPlayer(message), message.Message);
                case ChatChannel.Guild:
                    return string.Format("[公会]{0}{1}", FormatFromPlayer(message), message.Message);
            }
            return "";
        }

        /// <summary>
        /// 消息处理
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private string FormatFromPlayer(ChatMessage message)
        {
            if (message.FromId == User.Instance.CurrentCharacter.Id)
                return "<a name=\"\" class=\"player\">[我]</a>";
            else
                return string.Format("<a name=\"c:{0}:{1}\" class=\"player\">[{1}]</a>", message.FromId,message.FromName);
        }
    }
}
