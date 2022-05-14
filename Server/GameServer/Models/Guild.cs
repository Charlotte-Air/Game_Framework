using System;
using Common;
using System.Linq;
using Common.Utils;
using Charlotte.Proto;
using GameServer.Entities;
using GameServer.Services;
using GameServer.Managers;
using System.Collections.Generic;

namespace GameServer.Models
{
    class Guild
    {
        /// <summary>
        /// 公会缓存
        /// </summary>
        public TGuild Data;

        /// <summary>
        /// 公会ID
        /// </summary>
        public int id { get{return this.Data.Id; } }

        /// <summary>
        /// 公会姓名
        /// </summary>
        public string Name { get { return this.Data.Name; } }

        ///<summary>
        /// 时间戳
        /// </summary>
        public double timestamp;

        public Guild(TGuild guild)
        {
            this.Data = guild;
        }

        /// <summary>
        /// 公会申请
        /// </summary>
        /// <param name="apply">NGuildApplyInfo</param>
        /// <returns></returns>
        internal bool JoinApply(NGuildApplyInfo apply)
        {
            var oldApply = this.Data.Applies.FirstOrDefault(v => v.CharacterId == apply.characterId);
            if (oldApply != null)  //效验用户是否申请过
            {
                return false;
            }
            var dbApply = DBService.Instance.Entities.GuildApplies.Create();
            dbApply.TGuildId = apply.GuildId;
            dbApply.CharacterId = apply.characterId;
            dbApply.Name = apply.Name;
            dbApply.Class = apply.Class;
            dbApply.Level = apply.Level;
            dbApply.ApplyTime = DateTime.Now;
            DBService.Instance.Entities.GuildApplies.Add(dbApply);
            this.Data.Applies.Add(dbApply);
            DBService.Instance.Save();
            this.timestamp = TimeUtil.timestamp;
            return true;
        }

        /// <summary>
        /// 公会审批
        /// </summary>
        /// <param name="apply">NGuildApplyInfo</param>
        /// <returns></returns>
        internal bool JoinAppove(NGuildApplyInfo apply)
        {
            var oldApply = this.Data.Applies.FirstOrDefault(v => v.CharacterId == apply.characterId && v.Result == 0);
            if (oldApply !=null && apply.Result != ApplyResult.Accept)  //效验请求是否存在
            {
                return false;
            }
            if (apply.Result == ApplyResult.Accept)
            {
                this.AddMember(apply.characterId, apply.Name, apply.Class, apply.Level, GuildTitle.None);
            }
            if (oldApply != null) DBService.Instance.Entities.GuildApplies.Remove(oldApply);
            DBService.Instance.Save();
            this.timestamp = TimeUtil.timestamp;
            return true;
        }

        /// <summary>
        /// 添加成员
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="name">名称</param>
        /// <param name="class">职业</param>
        /// <param name="level">等级</param>
        /// <param name="title">时间</param>
        public void AddMember(int characterId, string name, int @class, int level, GuildTitle title)
        {
            DateTime now = DateTime.Now;
            TGuildMember dbMember = new TGuildMember()
            {
                CharacterId = characterId,
                Name = name,
                Class = @class,
                Level = level,
                Title = (int) title,
                JoinTime = now,
                LastTime = now,
            };
            this.Data.Members.Add(dbMember);
            var character = CharacterManager.Instance.GetCharacter(characterId);
            if (character != null)
            {
                character.Data.GuildId = this.id;
            }
            else
            {
                TCharacter dbchar = DBService.Instance.Entities.Characters.SingleOrDefault(c => c.ID == characterId);
                dbchar.GuildId = this.id;
            }
            timestamp = TimeUtil.timestamp;
        }

        /// <summary>
        /// 成员离开
        /// </summary>
        /// <param name="character">成员</param>
        /// <returns></returns>
        public bool Leave(Character character)
        {
            Log.InfoFormat("LeaveGuild-> Character [{0}:{1}]", character.Id, character.Name);
            var membr = DBService.Instance.Entities.GuildMembers.FirstOrDefault(v => v.CharacterId == character.Id);
            if (membr != null)
            {
                var guilds = DBService.Instance.Entities.Guilds.FirstOrDefault(v => v.LeaderID == membr.CharacterId);
                if (guilds != null)
                {
                    character.Data.GuildId = 0;
                    GuildManager.Instance.Guilds.Remove(character.Guild.id);
                    DBService.Instance.Entities.Guilds.Remove(guilds);
                    DBService.Instance.Entities.GuildMembers.Remove(membr);
                    DBService.Instance.Save();
                    this.Data.Members.Remove(membr);
                    timestamp = TimeUtil.timestamp;
                    return true;
                }
                else
                {
                    character.Data.GuildId = 0;
                    DBService.Instance.Entities.GuildMembers.Remove(membr);
                    DBService.Instance.Save();
                    this.Data.Members.Remove(membr);
                    timestamp = TimeUtil.timestamp;
                    return true;
                }
            }
            else
            {
                timestamp = TimeUtil.timestamp;
                return false;
            }
        }

        /// <summary> 公会指令处理 </summary>
        /// <param name="command"> 指令信息 </param>
        /// <param name="targetId">  被执行者ID </param>
        /// <param name="sourceId"> 发起者ID </param>
        internal void ExecuteAdmin(GuildAdminCommand command, int targetId, int sourceId)
        {
            var target = GetDBmember(targetId);
            var source = GetDBmember(sourceId);
            switch (command)
            {
                case GuildAdminCommand.Promote: //执行晋升职务
                    target.Title = (int) GuildTitle.VicePresident;
                    break;
                case GuildAdminCommand.Depost:  //执行罢免职务
                    target.Title = (int) GuildTitle.None;
                    break;
                case GuildAdminCommand.Tansfer: //执行转让公会
                    target.Title = (int) GuildTitle.President;
                    source.Title = (int) GuildTitle.None;
                    this.Data.LeaderID = targetId;
                    this.Data.LeaderName = target.Name;
                    break;
                case GuildAdminCommand.Kickout: //执行踢出成员
                    TCharacter character = DBService.Instance.Entities.Characters.FirstOrDefault(v => v.ID == targetId);
                    if (character != null)
                    {
                        var membr = this.Data.Members.FirstOrDefault(v => v.CharacterId == character.ID);
                        {
                            character.GuildId = 0;
                            DBService.Instance.Entities.GuildMembers.Remove(membr);
                            this.Data.Members.Remove(membr);
                        }
                    }
                    break;
            }
            DBService.Instance.Save();
            timestamp = TimeUtil.timestamp;
        }

        /// <summary>
        /// 后处理事件
        /// </summary>
        /// <param name="from"></param>
        /// <param name="message"></param>
        public void PostProcess(Character from,NetMessageResponse message)
        {
            if (message.Guild == null)
            {
                message.Guild = new GuildResponse();
                message.Guild.Result = Result.Success;
                message.Guild.guildInfo = this.GuildInfo(from);
            }
        }

        /// <summary>
        /// 公会信息
        /// </summary>
        /// <param name="from">Character</param>
        /// <returns>NGuildInfo</returns>
        internal NGuildInfo GuildInfo(Character from)
        {
            NGuildInfo info = new NGuildInfo()
            {
                Id = this.id,
                GuildName = this.Name,
                Notice = this.Data.Notice,
                leaderId = this.Data.LeaderID,
                Leadername = this.Data.LeaderName,
                createTime = (long) TimeUtil.GetTimestamp(this.Data.CreateTime),
                memberCount = this.Data.Members.Count,
            };
            if (from != null)
            {
                info.Members.AddRange(GetMemberInfos()); //返回公会成员的信息
                if(from.Id == this.Data.LeaderID)                     //判断请求消息是否是队长
                    info.Applies.AddRange(GetApplyInfos());    //返回公会成员的信息
            }
            return info;
        }


        /// <summary>
        /// DB数据 -> Net数据
        /// 获取公会成员消息
        /// </summary>
        /// <returns>NGuildMemberInfo</returns>
        List<NGuildMemberInfo> GetMemberInfos()
        {
            List<NGuildMemberInfo> members = new List<NGuildMemberInfo>();
            foreach (var member in this.Data.Members)  //从数据库遍历信息
            {
                var memberInfo = new NGuildMemberInfo()  //生成网络信息
                {
                    Id = member.Id,
                    characterId = member.CharacterId,
                    Title = (GuildTitle)member.Title,
                    joinTime = (long)TimeUtil.GetTimestamp(member.JoinTime),
                    lastTime = (long)TimeUtil.GetTimestamp(member.LastTime),
                };
                var character = CharacterManager.Instance.GetCharacter(member.CharacterId);
                if (character != null) //判断成员是否在线
                {
                    memberInfo.Info = character.GetBasicInfo();
                    memberInfo.Status = 1;
                    member.Level = character.Data.Level;
                    member.Name = character.Data.Name;
                    member.LastTime = DateTime.Now;
                }
                else
                {
                    memberInfo.Info = this.GetMaemberInfo(member);
                    memberInfo.Status = 0;
                }
                members.Add(memberInfo);
            }
            return members;
        }


        /// <summary>
        /// DB数据 -> Net数据
        /// 获取公会成员信息
        /// </summary>
        /// <param name="member">TGuildMember</param>
        /// <returns>NCharacterInfo</returns>
        NCharacterInfo GetMaemberInfo(TGuildMember member)
        {
            return new NCharacterInfo()
            {
                Id = member.Id,
                Name = member.Name,
                Class = (CharacterClass)member.Class,
                Level = member.Level,
            };
        }

        /// <summary>
        /// DB数据 -> Net数据
        /// 获取申请列表信息
        /// </summary>
        /// <returns>NGuildApplyInfo</returns>
        List<NGuildApplyInfo> GetApplyInfos()
        {
            List<NGuildApplyInfo> applies = new List<NGuildApplyInfo>();
            foreach (var apply in this.Data.Applies)
            {
                if(apply.Result !=(int)ApplyResult.None) continue;
                applies.Add(new NGuildApplyInfo()
                {
                    characterId = apply.CharacterId,
                    GuildId = apply.TGuildId,
                    Class=apply.Class,
                    Level = apply.Level,
                    Name = apply.Name,
                    Result = (ApplyResult)apply.Result,
                });
            }
            return applies;
        }

        /// <summary>
        /// 获取DB成员数据
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>TGuildMember</returns>
        TGuildMember GetDBmember(int characterId)
        {
            foreach (var member in this.Data.Members)
            {
                if (member.CharacterId == characterId)
                    return member;
            }
            return null;
        }

    }
}
