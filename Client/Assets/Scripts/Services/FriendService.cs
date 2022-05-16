using System;
using Models;
using Network;
using Managers;
using UnityEngine;
using Charlotte.Proto;
using UnityEngine.Events;

namespace Services
{
    class FriendService : Singleton<FriendService>, IDisposable
    {
        /// <summary>
        /// 更新好友列表事件
        /// </summary>
        public UnityAction OnFriendUpdate;
        public FriendService()
        {
            MessageDistributer.Instance.Subscribe<FriendAddRequest>(this.OnFriendAddRequest);       //添加好友请求
            MessageDistributer.Instance.Subscribe<FriendAddResponse>(this.OnFriendAddResponse);  //添加好友响应
            MessageDistributer.Instance.Subscribe<FriendListResponse>(this.OnFriendList);                    //好友列表响应
            MessageDistributer.Instance.Subscribe<FriendRemoveResponse>(this.OnFriendRemove);     //删除好友响应
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<FriendAddRequest>(this.OnFriendAddRequest);
            MessageDistributer.Instance.Unsubscribe<FriendAddResponse>(this.OnFriendAddResponse);
            MessageDistributer.Instance.Unsubscribe<FriendListResponse>(this.OnFriendList);
            MessageDistributer.Instance.Unsubscribe<FriendRemoveResponse>(this.OnFriendRemove);
        }

        public void Init()
        {

        }

        /// <summary>
        /// 发送添加好友请求
        /// </summary>
        /// <param name="friendId">发送者ID</param>
        /// <param name="friendName">发送者名称</param>
        public void SendFriendAddRequest(int friendId,string friendName)
        {
            Debug.Log("->SendFriendAdd");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.friendAddReq = new FriendAddRequest();
            message.Request.friendAddReq.FromId = User.Instance.CurrentCharacter.Id;
            message.Request.friendAddReq.FromName = User.Instance.CurrentCharacter.Name;
            message.Request.friendAddReq.ToId = friendId;
            message.Request.friendAddReq.ToName = friendName;
            NetClient.Instance.SendMessage(message);
        }

        /// <summary>
        /// 添加好友回调
        /// </summary>
        public void SendFriendAddResponse(bool accept, FriendAddRequest response)
        {
            Debug.Log("->SendFriendAdd");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.friendAddRes = new FriendAddResponse();
            message.Request.friendAddRes.Result = accept ? Result.Success : Result.Failed;
            message.Request.friendAddRes.Errorcode = accept ? (uint)100/*"对方同意"*/ : 101/*"对方拒绝"*/;
            message.Request.friendAddRes.Request = response;
            NetClient.Instance.SendMessage(message);
        }

        /// <summary>
        /// 收到添加好友请求
        /// </summary>
        /// <param name="sender"> 发送者 </param>
        /// <param name="request"> 请求消息 </param>
        private void OnFriendAddRequest(object sender, FriendAddRequest request)
        {
            var confirm=MessageBox.Show(string.Format("{0}请求加您为好友",request.FromName),"好友请求",MessageBoxType.Confirm,"同意","拒绝");
            confirm.OnYes =() =>
            {
                this.SendFriendAddResponse(true,request);
            };
            confirm.OnNo = () =>
            {
                this.SendFriendAddResponse(false, request);
            };
        }

        /// <summary>
        /// 收到添加好友响应
        /// </summary>
        /// <param name="sender"> 发送者 </param>
        /// <param name="message"> 响应消息 </param>
        private void OnFriendAddResponse(object sender, FriendAddResponse message)
        {
            if (message.Result == Result.Success)
            {
                if (User.Instance.CurrentCharacter.Name != message.Request.ToName)
                    MessageBox.Show(message.Request.ToName + "接受了您的请求", "添加好友成功");
                else
                    MessageBox.Show(message.Request.FromName + "接受了您的请求", "添加好友成功");
            }
            else if(message.Request != null)
            {
                if (User.Instance.CurrentCharacter.Name != message.Request.ToName)
                    MessageBox.Show(message.Request.ToName + "拒绝了您的请求", "添加好友失败");
                else
                    MessageBox.Show(message.Request.FromName + "拒绝了您的请求", "添加好友失败");
            }
            else
            {
                MessageBox.Show(string.Format(message.Errorcode.ToString()), "添加好友", MessageBoxType.Error);
            }
        }

        /// <summary>
        /// 好友列表更新
        /// </summary>
        private void OnFriendList(object sender, FriendListResponse message)
        {
            Debug.Log("->OnFriendList");
            FriendManager.Instance.allFriends = message.Friends;
            if (this.OnFriendUpdate != null)
                this.OnFriendUpdate();
        }

        /// <summary>
        /// 发送删除好友请求
        /// </summary>
        /// <param name="id">好友ID</param>
        /// <param name="friendId">发起者ID</param>
        public void SendFriendRemoveRequest(int id, int friendId)
        {
            Debug.Log("->SendFriendRemoveRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.friendRemove = new FriendRemoveRequest();
            message.Request.friendRemove.Id = id;
            message.Request.friendRemove.friendID = friendId;
            NetClient.Instance.SendMessage(message);
        }

        /// <summary>
        /// 删除好友回调
        /// </summary>
        private void OnFriendRemove(object sender, FriendRemoveResponse message)
        {
            if (message.Result == Result.Success)
                MessageBox.Show("删除成功", "删除好友");
            else
                MessageBox.Show("删除失败", "删除好友", MessageBoxType.Error);
        }




    }
}
