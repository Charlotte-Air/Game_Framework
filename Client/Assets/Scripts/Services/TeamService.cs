using System;
using Models;
using Network;
using UnityEngine;
using Charlotte.Proto;
using Assets.Scripts.Managers;

namespace Services
{
    class TeamService : Singleton<TeamService>, IDisposable
    {
        public void Init()
        {

        }

        public TeamService()
        {
            MessageDistributer.Instance.Subscribe<TeamInviteRequest>(this.OnTeamInviteRequest);         //邀请组队请求
            MessageDistributer.Instance.Subscribe<TeamInviteResponse>(this.OnTeamInviteResponse);    //邀请组队响应
            MessageDistributer.Instance.Subscribe<TeamInfoResponse>(this.OnTeamInfo);                         //组队列表请求
            MessageDistributer.Instance.Subscribe<TeamLeaveResponse>(this.OnTeamLeave);                   //离开队伍请求
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<TeamInviteRequest>(this.OnTeamInviteRequest);
            MessageDistributer.Instance.Unsubscribe<TeamInviteResponse>(this.OnTeamInviteResponse);
            MessageDistributer.Instance.Unsubscribe<TeamInfoResponse>(this.OnTeamInfo);
            MessageDistributer.Instance.Unsubscribe<TeamLeaveResponse>(this.OnTeamLeave);
        }

        /// <summary>
        /// 发送邀请组队请求
        /// </summary>
        /// <param name="friendId">发起人ID</param>
        /// <param name="friendName">发起人名称</param>
        public void SendTeamInviteRequest(int friendId, string friendName)
        {
            Debug.Log("->SendTeamInviteRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.teamInviteReq = new TeamInviteRequest();
            message.Request.teamInviteReq.FromId = User.Instance.CurrentCharacter.Id;
            message.Request.teamInviteReq.FromName = User.Instance.CurrentCharacter.Name;
            message.Request.teamInviteReq.ToId = friendId;
            message.Request.teamInviteReq.ToName = friendName;
            NetClient.Instance.SendMessage(message);
        }

        /// <summary>
        /// 邀请组队回调
        /// </summary>
        /// <param name="accept">是否同意</param>
        /// <param name="request"></param>
        public void SendTeamInviteResponse(bool accept,TeamInviteRequest request)
        {
            Debug.Log("->SendTeamInviteResponse");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.teamInviteRes = new TeamInviteResponse();
            message.Request.teamInviteRes.Result = accept ? Result.Success : Result.Failed;
            message.Request.teamInviteRes.Errorcode = accept ? (uint)102/*"组队成功"*/ : 103/*"对方拒绝了组队请求"*/;
            message.Request.teamInviteRes.Request = request;
            NetClient.Instance.SendMessage(message);
        }

        /// <summary>
        /// 收到邀请组队请求
        /// </summary>
        /// <param name="sender"> 发送者 </param>
        /// <param name="request"> 请求消息 </param>
        private void OnTeamInviteRequest(object sender, TeamInviteRequest request)
        {
            var confirm = MessageBox.Show(string.Format("{0}邀请你加入队伍", request.FromName), "好友请求", MessageBoxType.Confirm, "接受", "拒绝");
            confirm.OnYes = () =>
            {
                this.SendTeamInviteResponse(true,request);
            };
            confirm.OnNo = () =>
            {
                this.SendTeamInviteResponse(false,request);
            };
        }

        /// <summary>
        /// 收到组队邀请响应
        /// </summary>
        /// <param name="sender"> 发送者 </param>
        /// <param name="message"> 响应消息 </param>
        private void OnTeamInviteResponse(object sender, TeamInviteResponse response)
        {
            if (response.Result == Result.Success)
            {
                TeamManager.Instance.ShowTeamUI(true);
                MessageBox.Show(response.Request.ToName + "加入您的队伍", "邀请组队成功");
            }
            else
                MessageBox.Show(response.Errorcode.ToString(),"邀请组队失败");
        }

        /// <summary>
        /// 队伍列表更新
        /// </summary>
        private void OnTeamInfo(object sender, TeamInfoResponse message)
        {
            Debug.Log("->TeamInfo");
            TeamManager.Instance.UpdateTeamInfo(message.Team);
        }

        /// <summary>
        /// 发送离开队伍请求
        /// </summary>
        /// <param name="id"></param>
        public void SendTeamLevaeRequest(int id)
        {
            Debug.Log("->SendTeamLevaeRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.teamLeave = new TeamLeaveRequest();
            message.Request.teamLeave.TeamId = User.Instance.TeamInfo.Id;
            message.Request.teamLeave.characterId = User.Instance.CurrentCharacter.Id;
            NetClient.Instance.SendMessage(message);
        }

        /// <summary>
        /// 离开队伍回调
        /// </summary>
        private void OnTeamLeave(object sender, TeamLeaveResponse response)
        {
            if (response.Result == Result.Success)
            {
                TeamManager.Instance.UpdateTeamInfo(null);
                MessageBox.Show("退出成功", "退出队伍");
            }
            else
                MessageBox.Show("退出失败", "退出队伍", MessageBoxType.Error);
        }

    }
}
