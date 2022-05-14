using Network;
using Common;
using Charlotte.Proto;
using GameServer.Entities;
using GameServer.Managers;

namespace GameServer.Services
{
    class TeamService : Singleton<TeamService>
    {
        public TeamService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<TeamInviteRequest>(this.OnTeamInviteRquest);         //组队请求
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<TeamInviteResponse>(this.OnTeamInviteResponse);  //组队响应
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<TeamLeaveRequest>(this.OnTeamLeave);                   //离开队伍
        }

        public void Init()
        {
            TeamManager.Instance.Init();
        }

        /// <summary>
        /// 服务端处理组队请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        private void OnTeamInviteRquest(NetConnection<NetSession> sender, TeamInviteRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("TeamInviteRquest-> From [{0}:{1}] To [{2}:{3}]", request.FromId, request.FromName, request.ToId, request.ToName);
            NetConnection<NetSession> target = SessionManager.Instance.GetSession(request.ToId);
            if (target == null) //判断是否在线
            {
                sender.Session.Response.teamInviteRes = new TeamInviteResponse();
                sender.Session.Response.teamInviteRes.Result = Result.Failed;
                sender.Session.Response.teamInviteRes.Errorcode = (uint)ErrorCode.ErrorTeamUserNoOnline;
                sender.SendResponse();
                return;
            }
            if (target.Session.Character.Team != null) //判断对方是否有队伍
            {
                sender.Session.Response.teamInviteRes = new TeamInviteResponse();
                sender.Session.Response.teamInviteRes.Result = Result.Failed;
                sender.Session.Response.teamInviteRes.Errorcode = (uint)ErrorCode.ErrorTeamUserExistTeam;
                sender.SendResponse();
                return;
            }
            //转发请求
            Log.InfoFormat("ForwardTeamInviteRequest-> From [{0}:{1}] To [{2}:{3}]",request.FromId,request.FromName,request.ToId,request.ToName);
            target.Session.Response.teamInviteReq = request;
            target.SendResponse();
        }

        /// <summary>
        /// 服务端处理组队响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="response"></param>
        private void OnTeamInviteResponse(NetConnection<NetSession> sender, TeamInviteResponse response)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("TeamInviteResponse-> Character [{0}:{1}] Result [{2}] From [{3}:{4}] To [{5}:{6}]", character.Id,character.Name, response.Result, response.Request.FromId,response.Request.FromName, response.Request.ToId,response.Request.ToName);
            sender.Session.Response.teamInviteRes = response;
            if (response.Result == Result.Success)
            {    //接受组队请求
                var requester = SessionManager.Instance.GetSession(response.Request.FromId); 
                if (requester == null) //效验发起组队的用户是否在线
                {
                    sender.Session.Response.teamInviteRes.Result = Result.Failed;
                    sender.Session.Response.teamInviteRes.Errorcode = (uint)ErrorCode.ErrorAddRequestUserNoOnline;
                }
                else
                {
                    TeamManager.Instance.AddTeamMember(requester.Session.Character, character);
                    requester.Session.Response.teamInviteRes = response;
                    requester.SendResponse(); //发送给邀请者
                }
            }
            sender.SendResponse(); //发送给组队者
        }

        /// <summary>
        /// 服务端处理离开队伍请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        private void OnTeamLeave(NetConnection<NetSession> sender, TeamLeaveRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("TeamLeave-> CharacterID [{0}] TeamID [{1}:{2}]", character.Id,request.TeamId,request.characterId);
            character.Team.Leave(character);
            sender.Session.Response.teamLeave = new TeamLeaveResponse();
            sender.Session.Response.teamLeave.characterId = request.characterId;
            sender.Session.Response.teamLeave.Result = Result.Success;
            sender.SendResponse();
        }

    }
}
