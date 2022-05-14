using Common;
using Common.Utils;
using Charlotte.Proto;
using GameServer.Entities;
using System.Collections.Generic;

namespace GameServer.Managers
{
    class ChatManager : Singleton<ChatManager>
    {
        /// <summary>
        /// 系统频道集合
        /// </summary>
        public List<ChatMessage> System = new List<ChatMessage>();
        /// <summary>
        /// 世界频道集合
        /// </summary>
        public List<ChatMessage> World = new List<ChatMessage>();
        /// <summary>
        /// 本地频道集合
        /// </summary>
        public Dictionary<int, List<ChatMessage>> Local = new Dictionary<int, List<ChatMessage>>();
        /// <summary>
        /// 队伍频道集合
        /// </summary>
        public Dictionary<int, List<ChatMessage>> Team = new Dictionary<int, List<ChatMessage>>();
        /// <summary>
        /// 公会频道集合
        /// </summary>
        public Dictionary<int, List<ChatMessage>> Guild = new Dictionary<int, List<ChatMessage>>();

        public void Init()
        {

        }

        /// <summary>
        /// 添加聊天消息
        /// </summary>
        /// <param name="from">用户</param>
        /// <param name="message">信息内容</param>
        public void AddMessage(Character from, ChatMessage message)
        {
            message.FromId = from.Id;
            message.FromName = from.Name;
            message.Time = TimeUtil.timestamp;
            switch (message.Channel)
            {
                case ChatChannel.Local:
                    this.AddLocalMessage(from.Info.mapId[0], message);
                    break;
                case ChatChannel.World:
                    this.AddWorldMessage(message);
                    break;
                case ChatChannel.System:
                    this.AddSystemMessage(message);
                    break;
                case ChatChannel.Team:
                    this.AddTeamMessage(from.Team.Id, message);
                    break;
                case ChatChannel.Guild:
                    this.AddGuildMessage(from.Guild.id, message);
                    break;
            }
        }

        /// <summary>
        /// 添加本地消息
        /// </summary>
        /// <param name="mapid">地图ID</param>
        /// <param name="message"></param>
        public void AddLocalMessage(int mapid,ChatMessage message)
        {
            if (!this.Local.TryGetValue(mapid, out List<ChatMessage> messages))
            {
                messages = new List<ChatMessage>();
                this.Local[mapid] = messages;
            }
            messages.Add(message);
        }

        /// <summary>
        /// 添加世界消息
        /// </summary>
        /// <param name="message"></param>
        public void AddWorldMessage(ChatMessage message)
        {
            this.World.Add(message);
        }

        /// <summary>
        /// 添加系统消息
        /// </summary>
        /// <param name="message"></param>
        public void AddSystemMessage(ChatMessage message)
        {
            this.System.Add(message);
        }

        /// <summary>
        /// 添加组队消息
        /// </summary>
        /// <param name="TeamId">队伍ID</param>
        /// <param name="message"></param>
        public void AddTeamMessage(int TeamId,ChatMessage message)
        {
            if (!this.Team.TryGetValue(TeamId, out List<ChatMessage> messages))
            {
                messages = new List<ChatMessage>();
                this.Team[TeamId] = messages;
            }
            messages.Add(message);
        }

        /// <summary>
        /// 添加公会消息
        /// </summary>
        /// <param name="guildId">公会ID</param>
        /// <param name="message"></param>
        public void AddGuildMessage(int guildId,ChatMessage message)
        {
            if (!this.Guild.TryGetValue(guildId, out List<ChatMessage> messages))
            {
                messages = new List<ChatMessage>();
                this.Guild[guildId] = messages;
            }
            messages.Add(message);
        }

        /// <summary>
        /// 获取本地消息
        /// </summary>
        /// <param name="mapid"></param>
        /// <param name="idx"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public int GetLocalMessages(int mapid, int idx, List<ChatMessage> result)
        {
            if (!this.Local.TryGetValue(mapid, out List<ChatMessage> messages))
                return 0;
            return GetNewMessages(idx, result, messages);
        }

        /// <summary>
        /// 获取世界消息
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public int GetWorldMessages(int idx, List<ChatMessage> result)
        {
            return GetNewMessages(idx, result,this.World);
        }

        /// <summary>
        /// 获取系统消息
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public int GetSystemMessages(int idx, List<ChatMessage> result)
        {
            return GetNewMessages(idx, result, this.System);
        }

        /// <summary>
        /// 获取组队消息
        /// </summary>
        /// <param name="Teamid">队伍ID</param>
        /// <param name="idx"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public int GetTeamMessages(int Teamid, int idx, List<ChatMessage> result)
        {
            if (!this.Team.TryGetValue(Teamid, out List<ChatMessage> messages))
                return 0;
            return GetNewMessages(idx, result, messages);
        }

        /// <summary>
        /// 获取公会消息
        /// </summary>
        /// <param name="guildId">公会ID</param>
        /// <param name="idx"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public int GetGuildMessages(int guildId, int idx, List<ChatMessage> result)
        {
            if (!this.Guild.TryGetValue(guildId, out List<ChatMessage> messages))
                return 0;
            return GetNewMessages(idx, result, messages);
        }

        /// <summary>
        /// 获取新消息
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="result"></param>
        /// <param name="messages"></param>
        /// <returns></returns>
        private int GetNewMessages(int idx, List<ChatMessage> result, List<ChatMessage> messages)
        {
            if (idx == 0)
            {
                if (messages.Count > GameDefine.MaxChatRecoredNums) //取前20条聊天记录
                    idx = messages.Count - GameDefine.MaxChatRecoredNums;
            }
            for (; idx < messages.Count; idx++) //从20条开始遍历
            {
                result.Add(messages[idx]);
            }
            return idx;
        }
    }
}
