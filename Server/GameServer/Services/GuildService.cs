using Network;
using Common;
using Charlotte.Proto;
using GameServer.Entities;
using GameServer.Managers;

namespace GameServer.Services
{
    class GuildService : Singleton<GuildService>
    {
        public GuildService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildCreateRequest>(this.OnGuildCreate);               //创建公会
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildListRequest>(this.OnGuildList);                         //公会列表
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildJoinRequest>(this.OnGuildJoinRequest);          //加入公会
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildJoinResponse>(this.OnGuildJoinResponse);     //加入公会回调
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildLeaveRequest>(this.OnGuildLeave);                  //离开公会
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildAdminRequest>(this.OnGuildAdmin);               //权限指令处理
        }

        public void Init()
        {
            GuildManager.Instance.Init();
        }

        /// <summary>
        /// 服务端处理创建公会请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        private void OnGuildCreate(NetConnection<NetSession> sender, GuildCreateRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("GuildCreateRequest-> GuildName [{0}]  Character [{1}]:{2}",request.Guildname,character.Id,character.Name);
            sender.Session.Response.guildCreate = new GuildCreateResponse();
            if (character.Guild != null)
            {
                sender.Session.Response.guildCreate.Result = Result.Failed;
                sender.Session.Response.guildCreate.Errorcode = (uint)ErrorCode.ErrorGuildExist;
                sender.SendResponse();
                return;
            }
            if (GuildManager.Instance.CheckNameExisted(request.Guildname))
            {
                sender.Session.Response.guildCreate.Result = Result.Failed;
                sender.Session.Response.guildCreate.Errorcode = (uint)ErrorCode.ErrorGuildNameExist;
                sender.SendResponse();
                return;
            }
            GuildManager.Instance.CreateGuild(request.Guildname, request.Guildnotice, character);
            sender.Session.Response.guildCreate.Guildinfo = character.Guild.GuildInfo(character);
            sender.Session.Response.guildCreate.Result = Result.Success;
            sender.SendResponse();
        }

        /// <summary>
        /// 服务端处理拉取公会列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        private void OnGuildList(NetConnection<NetSession> sender, GuildListRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("GuildList-> Character [{0}:{1}]", character.Id, character.Name);
            sender.Session.Response.guildList = new GuildListResponse();
            sender.Session.Response.guildList.Guilds.AddRange(GuildManager.Instance.GetGuildsInfo());
            sender.SendResponse();
        }

        /// <summary>
        /// 服务端处理用户加入公会请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        private void OnGuildJoinRequest(NetConnection<NetSession> sender, GuildJoinRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("GuildJoinRequest-> GuildID [{0}] Character [{1}:{2}]", request.Apply.GuildId, request.Apply.characterId, request.Apply.Name);
            var guild = GuildManager.Instance.GetGuild(request.Apply.GuildId);
            if (guild == null) //效验公会是否存在
            {
                sender.Session.Response.guildJoinRes = new GuildJoinResponse();
                sender.Session.Response.friendAddRes.Result = Result.Failed;
                sender.Session.Response.friendAddRes.Errorcode = (uint)ErrorCode.ErrorGuildNoExist;
                sender.SendResponse();
                return;
            }
            request.Apply.characterId = character.Data.ID;
            request.Apply.Name = character.Data.Name;
            request.Apply.Class = character.Data.Class;
            request.Apply.Level = character.Data.Level;
            var leader = SessionManager.Instance.GetSession(guild.Data.LeaderID);
            if (leader != null) //判断会长是否在线
            {
                leader.Session.Response.guildJoinReq = request;
                leader.SendResponse(); //给会长发送玩家申请加入公会请求
            }
            else
            {
                if (guild.JoinApply(request.Apply)) //加入公会申请列表
                {
                    sender.Session.Response.guildJoinRes = new GuildJoinResponse();
                    sender.Session.Response.guildJoinRes.Result = Result.Failed;
                    sender.Session.Response.guildJoinRes.Errorcode = (uint)ErrorCode.ErrorGuildApprovalTitle;
                    sender.SendResponse();
                }
                else
                {
                    sender.Session.Response.guildJoinRes = new GuildJoinResponse();
                    sender.Session.Response.guildJoinRes.Result = Result.Failed;
                    sender.Session.Response.guildJoinRes.Errorcode = (uint)ErrorCode.ErrorGuildApplyExist;
                    sender.SendResponse();
                }
            }
        }

        /// <summary>
        /// 用户加入公会回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="response"></param>
        private void OnGuildJoinResponse(NetConnection<NetSession> sender, GuildJoinResponse response)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("GuildJoinResponse-> GuildID [{0}] Character [{1}:{2}]",response.Apply.GuildId,response.Apply.characterId,response.Apply.Name);
            var guild = GuildManager.Instance.GetGuild(response.Apply.GuildId);
            if (response.Result == Result.Success)
            {
                guild.JoinAppove(response.Apply);
            }
            var requester = SessionManager.Instance.GetSession(response.Apply.characterId);
            if (requester != null) //判断申请加入公会的用户是否在线
            {
                requester.Session.Character.Guild = guild;
                requester.Session.Response.guildJoinRes = response;
                requester.Session.Response.guildJoinRes.Result = Result.Success;
                requester.Session.Response.guildJoinRes.Errorcode = (uint)ErrorCode.GuildAddSucced;
                requester.SendResponse();
            }
        }

        /// <summary>
        /// 服务端处理用户离开公会
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        private void OnGuildLeave(NetConnection<NetSession> sender, GuildLeaveRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("GuildLeave-> Character [{0}:{1}]", character.Id, character.Name);
            sender.Session.Response.guildLeave = new GuildLeaveResponse();
            if (character.Guild.Leave(character))
            {
                sender.Session.Response.guildLeave.Result = Result.Success;
                sender.Session.Response.guildLeave.Errorcode = (uint)ErrorCode.GuildExitsSucced;
                sender.Session.Response.guildLeave.Guilds.AddRange(GuildManager.Instance.GetGuildsInfo());
                sender.SendResponse();
            }
            else
            {
                sender.Session.Response.guildLeave.Result = Result.Failed;
                sender.Session.Response.guildLeave.Errorcode = (uint)ErrorCode.ErrorGuildExitsFail;
                sender.SendResponse();
            }

        }

        /// <summary>
        /// 服务端处理公会指令请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnGuildAdmin(NetConnection<NetSession> sender, GuildAdminRequest message)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("GuildAdmin-> Character:[{0}]:{1}",character.Id,character.Name);
            sender.Session.Response.guildAdmin = new GuildAdminRespnonse();
            if (character.Guild == null)
            {
                sender.Session.Response.guildAdmin.Resutl = Result.Failed;
                sender.Session.Response.guildAdmin.Errorcode = (uint)ErrorCode.ErrorAdminGuildFail;
                sender.SendResponse();
                return;
            }
            character.Guild.ExecuteAdmin(message.Command, message.Target, character.Id);
            var CharacterTarget = DBService.Instance.Entities.Characters.Find(message.Target);
            var target = SessionManager.Instance.GetSession(message.Target); //取目标是否在线
            if (target != null)
            {
                target.Session.Response.guildAdmin = new GuildAdminRespnonse();
                target.Session.Response.guildList = new GuildListResponse();
                target.Session.Response.guildList.Guilds.AddRange(GuildManager.Instance.GetGuildsInfo());
                target.Session.Response.guildAdmin.Resutl = Result.Success;
               // target.Session.Response.guildAdmin.Errormsg = "" + CharacterTarget.Name;
                target.Session.Response.guildAdmin.Command = message;
                target.SendResponse();
            }
            else
            {
                sender.Session.Response.guildAdmin.Resutl = Result.Success;
                //sender.Session.Response.guildAdmin.Errormsg = "" + CharacterTarget.Name;
                sender.Session.Response.guildAdmin.Command = message;
                sender.SendResponse();
            }

        }
    }
}
