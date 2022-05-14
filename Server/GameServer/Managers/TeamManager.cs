using Common;
using GameServer.Entities;
using GameServer.Models;
using System.Collections.Generic;

namespace GameServer.Managers
{
    class TeamManager : Singleton<TeamManager>
    {
        /// <summary>
        /// 队伍集合
        /// </summary>
        public List<Team> Teams = new List<Team>();
        /// <summary>
        /// 角色队伍集合
        /// </summary>
        public Dictionary<int, Team> CharacterTeams = new Dictionary<int, Team>();

        public void Init()
        {

        }

        /// <summary>
        /// 获取队伍成员
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>Team</returns>
        public Team GetTeamByCharacter(int characterId)
        {
            Team team = null;
            this.CharacterTeams.TryGetValue(characterId, out team);
            return team;
        }

        /// <summary>
        /// 添加队伍成员
        /// </summary>
        /// <param name="leader">队长</param>
        /// <param name="member">成员</param>
        public void AddTeamMember(Character leader, Character member)
        {
            if (leader.Team == null) //效验队长是否存在队伍
                leader.Team = CreateTeam(leader);
            leader.Team.AddMember(member);
        }

        /// <summary>
        /// 创建队伍
        /// </summary>
        /// <param name="leader">队长</param>
        /// <returns>Team</returns>
        Team CreateTeam(Character leader)
        {
            Team team = null;
            for (int i = 0; i < this.Teams.Count; i++) //遍历所有队伍
            {
                team = this.Teams[i];
                if (team.Members.Count == 0) //如果存在空队伍直接使用
                {
                    team.AddMember(leader);
                    return team;
                }
            }
            team = new Team(leader);
            this.Teams.Add(team);
            team.Id = this.Teams.Count;
            return team;
        }

    }
}
