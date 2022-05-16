using System;
using Network;
using Managers;
using UnityEngine;
using Charlotte.Proto;
using UnityEngine.Events;
using System.Collections.Generic;

namespace Services
{
    class GuildService : Singleton<GuildService>,IDisposable
    {
        /// <summary>
        /// 更新公会事件
        /// </summary>
        public UnityAction OnGuildUpdate;
        /// <summary>
        /// 加入公会事件
        /// </summary>
        public UnityAction OnJoinGuild;
        /// <summary>
        /// 离开公会事件
        /// </summary>
        public UnityAction OnLeaveGuild;
        /// <summary>
        /// 创建公会事件
        /// </summary>
        public UnityAction<bool> OnGuildCreateResult;
        /// <summary>
        /// 拉取公会列表事件
        /// </summary>
        public UnityAction<List<NGuildInfo>> OnGuildListResult;
        public GuildService()
        {
            MessageDistributer.Instance.Subscribe<GuildCreateResponse>(this.OnGuildCreate);
            MessageDistributer.Instance.Subscribe<GuildListResponse>(this.OnGuildList);
            MessageDistributer.Instance.Subscribe<GuildJoinRequest>(this.OnGuildJoinRequest);
            MessageDistributer.Instance.Subscribe<GuildJoinResponse>(this.OnGuildJoinResponse);
            MessageDistributer.Instance.Subscribe<GuildResponse>(this.OnGuild);
            MessageDistributer.Instance.Subscribe<GuildLeaveResponse>(this.OnGuildLeave);
            MessageDistributer.Instance.Subscribe<GuildAdminRespnonse>(this.OnGuildAdmin);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<GuildCreateResponse>(this.OnGuildCreate);
            MessageDistributer.Instance.Unsubscribe<GuildListResponse>(this.OnGuildList);
            MessageDistributer.Instance.Unsubscribe<GuildJoinRequest>(this.OnGuildJoinRequest);
            MessageDistributer.Instance.Unsubscribe<GuildJoinResponse>(this.OnGuildJoinResponse);
            MessageDistributer.Instance.Unsubscribe<GuildResponse>(this.OnGuild);
            MessageDistributer.Instance.Unsubscribe<GuildLeaveResponse>(this.OnGuildLeave);
            MessageDistributer.Instance.Unsubscribe<GuildAdminRespnonse>(this.OnGuildAdmin);
        }

        public void Init()
        {

        }

        /// <summary>
        /// 发送创建公会请求
        /// </summary>
        /// <param name="guildName">公会名称</param>
        /// <param name="notice">公会介绍</param>
        public void SendGuildCreate(string guildName,string notice)
        {
            Debug.Log("->SendGuildCreate");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildCreate = new GuildCreateRequest();
            message.Request.guildCreate.Guildname = guildName;
            message.Request.guildCreate.Guildnotice = notice;
            NetClient.Instance.SendMessage(message);
        }

        /// <summary>
        /// 创建公会回调
        /// </summary>
        private void OnGuildCreate(object sender, GuildCreateResponse response)
        {
            Debug.LogFormat("->OnGuildCreateResponse-> {0}", response.Result);
            if (OnGuildCreateResult != null)
            {
                this.OnGuildCreateResult(response.Result == Result.Success); //通知给界面
            }
            if (response.Result == Result.Success)
            {
                GuildManager.Instance.Init(response.Guildinfo);
                MessageBox.Show(string.Format("{0} 公会创建成功", response.Guildinfo.GuildName), "公会");
            }
            else
            {
                MessageBox.Show(string.Format("{0} 公会创建失败", response.Guildinfo.GuildName), "公会",MessageBoxType.Error);
            }
        }

        /// <summary>
        /// 发送加入公会请求
        /// </summary>
        /// <param name="guildId">公会ID</param>
        public void SendGuildJoinRequest(int guildId)
        {
            Debug.Log("->SendGuildJoinRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildJoinReq = new GuildJoinRequest();
            message.Request.guildJoinReq.Apply = new NGuildApplyInfo();
            message.Request.guildJoinReq.Apply.GuildId = guildId;
            NetClient.Instance.SendMessage(message);
        }

        /// <summary>
        /// 发送申请列表-成员加入公会审批请求
        /// </summary>
        /// <param name="accept">是否同意</param>
        /// <param name="request">GuildJoinRequest</param>
        public void SendGuildJoinResponse(bool accept, GuildJoinRequest request)
        {
            Debug.Log("->SendGuildJoinResponse");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildJoinRes = new GuildJoinResponse();
            message.Request.guildJoinRes.Result = Result.Success;
            message.Request.guildJoinRes.Apply = request.Apply;
            message.Request.guildJoinRes.Apply.Result = accept ? ApplyResult.Accept : ApplyResult.Reject;
            NetClient.Instance.SendMessage(message);
        }

        /// <summary>
        /// 申请列表-成员加入公会审批回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        private void OnGuildJoinRequest(object sender, GuildJoinRequest request)
        {
            var confirm=MessageBox.Show(string.Format("[{0}] 申请加入公会",request.Apply.Name),"公会申请",MessageBoxType.Confirm);
            confirm.OnYes = () =>
            {
                this.SendGuildJoinResponse(true, request);
            };
            confirm.OnNo = () =>
            {
                this.SendGuildJoinResponse(false, request);
            };
        }

        /// <summary>
        /// 申请列表-成员审批结果回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="response"></param>
        private void OnGuildJoinResponse(object sender, GuildJoinResponse response)
        {
            Debug.LogFormat("GuildJoinResponse-> {0}", response.Result);
            if (response.Result == Result.Success)
            {
                MessageBox.Show(string.Format("{0}", response.Errorcode), "加入公会",MessageBoxType.Information).OnYes = () =>
                 {
                    if (this.OnJoinGuild != null)
                        this.OnJoinGuild();
                    if (this.OnGuildUpdate != null)
                        this.OnGuildUpdate();
                 };
            }
            else
            {
                MessageBox.Show(string.Format("{0}", response.Errorcode, "加入公会", MessageBoxType.Error));
            }
        }

        /// <summary>
        /// 公会信息请求
        /// </summary>
        private void OnGuild(object sender, GuildResponse message)
        {
            Debug.LogFormat("Guild-> {0} {1}:{2} ", message.Result, message.guildInfo.Id, message.guildInfo.GuildName);
            GuildManager.Instance.Init(message.guildInfo);
            if (this.OnGuildUpdate != null)
                this.OnGuildUpdate();
        }

        /// <summary>
        /// 发送离开公会请求
        /// </summary>
        public void SendGuildLeaveRequest()
        {
            Debug.Log("->SendGuildLeaveRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildLeave = new GuildLeaveRequest();
            NetClient.Instance.SendMessage(message);
        }

        /// <summary>
        /// 离开公会回调
        /// </summary>
        private void OnGuildLeave(object sender, GuildLeaveResponse response)
        {
            if (response.Result == Result.Success)
            {
                GuildManager.Instance.Init(null);
                if (this.OnGuildUpdate != null)
                    this.OnGuildUpdate();
                MessageBox.Show("离开公会成功", "公会通知", MessageBoxType.Information).OnYes = () =>
                 {
                    if(this.OnLeaveGuild != null)
                        this.OnLeaveGuild();
                 };
            }
            else
            {
                MessageBox.Show("离开公会失败", "公会通知", MessageBoxType.Error);
            }
        }

        /// <summary>
        /// 发送拉取公会列表请求
        /// </summary>
        public void SendGuildListRequest()
        {
            Debug.Log("->SendGuildListRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildList = new GuildListRequest();
            NetClient.Instance.SendMessage(message);
        }

        /// <summary>
        /// 请求公会列表回调
        /// </summary>
        private void OnGuildList(object sender, GuildListResponse response)
        {
            if (OnGuildListResult != null)
                this.OnGuildListResult(response.Guilds);
        }

        /// <summary>
        /// 发送加入公会请求
        /// </summary>
        /// <param name="accept">是否同意</param>
        /// <param name="apply">NGuildApplyInfo</param>
        public void SendGuildJoinApply(bool accept,NGuildApplyInfo apply)
        {
            Debug.Log("->SendGuildJoinApply");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildJoinRes = new GuildJoinResponse();
            message.Request.guildJoinRes.Apply = apply;
            message.Request.guildJoinRes.Apply.Result = accept ? ApplyResult.Accept : ApplyResult.Reject;
            NetClient.Instance.SendMessage(message);
        }

        /// <summary>
        /// 发送公会权限操作
        /// </summary>
        /// <param name="command">权限值</param>
        /// <param name="characterId">角色ID</param>
        internal void SendAdminCommand(GuildAdminCommand command,int characterId)
        {
            Debug.Log("->SendAdminCommand");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildAdmin = new GuildAdminRequest();
            message.Request.guildAdmin.Command = command;
            message.Request.guildAdmin.Target = characterId;
            NetClient.Instance.SendMessage(message);
        }

        /// <summary>
        /// 公会权限操作回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnGuildAdmin(object sender, GuildAdminRespnonse message)
        {
            Debug.LogFormat("GuildAdmin-> {0} {1}",message.Command,message.Resutl);
            if (message.Resutl == Result.Success)
            {
                if (message.Command != null)
                {
                    if (message.Command.Command == GuildAdminCommand.Kickout)
                    {
                        GuildManager.Instance.Init(null);
                        MessageBox.Show(string.Format("[{0}]成员已被踢出", message.Errorcode), "公会信息");
                        if (this.OnLeaveGuild != null)
                            this.OnLeaveGuild();
                    }
                    if (message.Command.Command == GuildAdminCommand.Promote) MessageBox.Show(string.Format("[{0}]成员已被晋升", message.Errorcode), "公会信息");
                    if (message.Command.Command == GuildAdminCommand.Depost) MessageBox.Show(string.Format("[{0}]成员已被罢免", message.Errorcode), "公会信息");
                    if (message.Command.Command == GuildAdminCommand.Tansfer) MessageBox.Show(string.Format("公会转让给[{0}]成员", message.Errorcode), "公会信息");
                    if (this.OnGuildUpdate != null)
                        this.OnGuildUpdate();
                }
            }
            else
            {
                if (message.Command.Command == GuildAdminCommand.Kickout) MessageBox.Show(string.Format("[{0}]成员踢出", message.Errorcode), "失败",MessageBoxType.Error);
                if (message.Command.Command == GuildAdminCommand.Promote) MessageBox.Show(string.Format("[{0}]成员晋升", message.Errorcode), "失败", MessageBoxType.Error);
                if (message.Command.Command == GuildAdminCommand.Depost) MessageBox.Show(string.Format("[{0}]成员罢免", message.Errorcode), "失败", MessageBoxType.Error);
                if (message.Command.Command == GuildAdminCommand.Tansfer) MessageBox.Show(string.Format("公会转让给[{0}]成员", message.Errorcode), "失败", MessageBoxType.Error);
            }
        }

    }
}
