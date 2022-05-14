using Common;
using Common.Utils;
using Charlotte.Proto;
using GameServer.Entities;
using System.Collections.Generic;

namespace GameServer.Models
{
    class Team
    {
        /// <summary>
        /// 队伍ID
        /// </summary>
        public int Id;
        /// <summary>
        /// 队长
        /// </summary>
        public Character Leader;
        /// <summary>
        /// 成员列表
        /// </summary>
        public List<Character> Members = new List<Character>();
        /// <summary>
        /// 时间戳
        /// </summary>
        public double timestamp;

        public Team(Character leader)
        {
            this.AddMember(leader);
        }

        /// <summary>
        /// 添加成员
        /// </summary>
        /// <param name="member">角色</param>
        public void AddMember(Character member)
        {
            if (this.Members.Count == 0) //如果是空队伍第一个成员成为队长
            {
                this.Leader = member;
            }
            this.Members.Add(member);
            member.Team = this;                     //把当前的队伍指定给这个成员
            timestamp = TimeUtil.timestamp; //更新时间戳
        }

        /// <summary>
        /// 离开组队
        /// </summary>
        /// <param name="member">成员</param>
        public void Leave(Character member)
        {
            Log.InfoFormat("TeamLeave-> Character [{0}:{1}]",this.Id,member.Info.Name);
            this.Members.Remove(member);
            if (member == this.Leader)
            {
                if (this.Members.Count > 0)
                    this.Leader = this.Members[0]; //队伍中还存在用户这个用户成为队长
                else
                    this.Leader = null;
            }
            member.Team = null;
            timestamp = TimeUtil.timestamp; //更新时间戳
        }

        /// <summary>
        /// 后处理器事件
        /// </summary>
        /// <param name="message"></param>
        public void PostProcess(NetMessageResponse message)
        {
            if (message.teamInfo==null)
            {
                message.teamInfo = new TeamInfoResponse();
                message.teamInfo.Result = Result.Success;
                message.teamInfo.Team = new NTeamInfo();
                message.teamInfo.Team.Id = this.Id;
                message.teamInfo.Team.Leader = this.Leader.Id;
                foreach (var member in this.Members)
                {
                    message.teamInfo.Team.Members.Add(member.GetBasicInfo());
                }
            }
        }
    }
}
