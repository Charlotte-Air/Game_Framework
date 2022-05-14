using System;
using Common;
using Common.Utils;
using Charlotte.Proto;
using GameServer.Entities;
using GameServer.Models;
using GameServer.Services;
using System.Collections.Generic;

namespace GameServer.Managers
{
    /// <summary>
    /// 公会管理器
    /// </summary>
    class GuildManager :Singleton<GuildManager>
    {
        /// <summary>
        /// 公会集合
        /// </summary>
        public Dictionary<int, Guild> Guilds = new Dictionary<int, Guild>();
        /// <summary>
        /// 公会名称集合
        /// </summary>
        public HashSet<string> GuildNames = new HashSet<string>();

        public void Init()
        {
            this.Guilds.Clear();
            foreach (var guild in DBService.Instance.Entities.Guilds)
            {
                this.AddGuild(new Guild(guild));
            }
        }

        /// <summary>
        /// 添加公会
        /// </summary>
        /// <param name="guild">公会ID</param>
        void AddGuild(Guild guild)
        {
            this.Guilds.Add(guild.id, guild);
            this.GuildNames.Add(guild.Name);
            guild.timestamp = TimeUtil.timestamp;
        }

        /// <summary>
        /// 检查公会名称是否存在
        /// </summary>
        /// <param name="name">公会名称</param>
        /// <returns></returns>
        public bool CheckNameExisted(string name)
        {
            return GuildNames.Contains(name);
        }

        /// <summary>
        /// 创建公会
        /// </summary>
        /// <param name="name">公会名称</param>
        /// <param name="notice">公会介绍</param>
        /// <param name="leader">角色</param>
        /// <returns></returns>
        public bool CreateGuild(string name, string notice, Character leader)
        {
            DateTime now =DateTime.Now;
            TGuild dbGuild = DBService.Instance.Entities.Guilds.Create();
            dbGuild.Name = name;
            dbGuild.Notice = notice;
            dbGuild.LeaderID = leader.Id;
            dbGuild.LeaderName = leader.Name;
            dbGuild.CreateTime = now;
            DBService.Instance.Entities.Guilds.Add(dbGuild);

            Guild guild = new Guild(dbGuild);
            guild.AddMember(leader.Id,leader.Name,leader.Data.Class,leader.Data.Level,GuildTitle.President);
            leader.Guild = guild;
            leader.Data.GuildId = dbGuild.Id;
            DBService.Instance.Save();
            this.AddGuild(guild);
            return true;
        }

        /// <summary>
        /// 获取公会
        /// </summary>
        /// <param name="guildId">公会ID</param>
        /// <returns>Guild</returns>
        internal Guild GetGuild(int guildId)
        {
            if (guildId == 0)
                return null;
            Guild guild = null;
            this.Guilds.TryGetValue(guildId, out guild);
            return guild;
        }

        /// <summary>
        /// 获取公会列表清单
        /// </summary>
        /// <returns>NGuildInfo</returns>
        internal List<NGuildInfo> GetGuildsInfo()
        {
            List<NGuildInfo> result = new List<NGuildInfo>();
            foreach (var kv in this.Guilds)
            {
                result.Add(kv.Value.GuildInfo(null));
            }
            return result;
        }

    }
}
