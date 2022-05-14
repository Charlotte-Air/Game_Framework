using Network;
using Common;
using System.Linq;
using Charlotte.Proto;
using GameServer.Entities;
using GameServer.Managers;

namespace GameServer.Services
{
    class FriendService : Singleton<FriendService>
    {
        public FriendService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<FriendAddRequest>(this.OnFriendAddRquest);         //添加好友
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<FriendAddResponse>(this.OnFriendAddResponse);  //添加好友回调
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<FriendRemoveRequest>(this.OnFriendRemove);       //删除好友
        }

        public void Init()
        {

        }

        /// <summary>
        /// 服务端处理添加好友请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        private void OnFriendAddRquest(NetConnection<NetSession> sender, FriendAddRequest request)
        {
            var toch =SessionManager.Instance.GetSession(request.ToId);
            if (toch == null)
            {
                sender.Session.Response.friendAddRes = new FriendAddResponse();
                sender.Session.Response.friendAddRes.Result = Result.Failed;
                sender.Session.Response.friendAddRes.Errorcode = (uint)ErrorCode.ErrorAddFriendUserNoOnline;
                sender.SendResponse();
                return;
            }
            Character character = sender.Session.Character;
            if (request.ToName == null)
            {
                Character toCharacter = CharacterManager.Instance.GetCharacter(request.ToId);
                request.ToName = toCharacter.Name;
            }
            Log.InfoFormat("FriendAddRequest-> From [{0}:{1}] To [{2}:{3}]",request.FromId,request.FromName,request.ToId,request.ToName);
            if (request.ToId == 0)
            {
                foreach (var cha in CharacterManager.Instance.Characters)  //如没有传入ID，则使用名称查找取出ID
                {
                    if (cha.Value.Data.Name == request.ToName)
                    {
                        request.ToId = cha.Key;
                        break;
                    }
                }
            }
            NetConnection<NetSession> friend = null;  //代表需要添加的好友
            if (request.ToId > 0)
            {
                if (character.FriendManager.GetFriendInfo(request.ToId) != null) //查找本身与需要添加的好友否存在
                {
                    sender.Session.Response.friendAddRes = new FriendAddResponse();
                    sender.Session.Response.friendAddRes.Result = Result.Failed;
                    sender.Session.Response.friendAddRes.Errorcode = (uint)ErrorCode.ErrorAddFriendUserExist;
                    sender.SendResponse();
                    return;
                }
                friend = SessionManager.Instance.GetSession(request.ToId);
            }
            if (friend == null)
            {
                sender.Session.Response.friendAddRes = new FriendAddResponse();
                sender.Session.Response.friendAddRes.Result = Result.Failed;
                sender.Session.Response.friendAddRes.Errorcode = (uint)ErrorCode.ErrorAddFriendUserExistTitle;
                sender.SendResponse();
                return;
            }
            Log.InfoFormat("ForwardRequest-> From [{0}:{1}] To [{2}:{3}]", request.FromId, request.FromName, request.ToId, request.ToName);
            friend.Session.Response.friendAddReq = request; //直接向添加好友那方转发消息
            friend.SendResponse();
        }

        /// <summary>
        /// 添加好友回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="response"></param>
        private void OnFriendAddResponse(NetConnection<NetSession> sender, FriendAddResponse response)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("FriendAddResponse-> Result [{0}] From [{1}:{2}] To [{3}:{4}]",response.Result, response.Request.FromId,response.Request.FromName, response.Request.ToId, response.Request.ToName);
            sender.Session.Response.friendAddRes = response;
            if (response.Result == Result.Success)   //好友请求成功
            {
                var requester = SessionManager.Instance.GetSession(response.Request.FromId); //获取原玩家的消息
                if (requester == null) //判断原玩家是否在线
                {
                    sender.Session.Response.friendAddRes.Result = Result.Failed;
                    sender.Session.Response.friendAddRes.Errorcode = (uint)ErrorCode.ErrorAddRequestUserNoOnline;
                }
                else  //原玩家在线
                {   
                    character.FriendManager.AddFriend(requester.Session.Character);   //A添加B
                    requester.Session.Character.FriendManager.AddFriend(character);   //B添加A
                    DBService.Instance.Save();
                    requester.Session.Response.friendAddRes = response;
                    requester.Session.Response.friendAddRes.Result = Result.Success;
                    requester.Session.Response.friendAddRes.Errorcode = (uint)ErrorCode.FriendAddSucced;
                    requester.SendResponse();
                }
                sender.SendResponse();
            }
            else
            {
                var requester = SessionManager.Instance.GetSession(response.Request.FromId);
                if (requester == null)
                {
                    sender.Session.Response.friendAddRes.Result = Result.Failed;
                    sender.Session.Response.friendAddRes.Errorcode = (uint)ErrorCode.ErrorAddFriendUserExistTitle;
                }
                requester.Session.Response.friendAddRes = response;
                requester.Session.Response.friendAddRes.Result = Result.Failed;
                requester.Session.Response.friendAddRes.Errorcode = (uint)ErrorCode.ErrorAddUserReject;
                requester.SendResponse();
            }
        }

        /// <summary>
        /// 服务端处理删除好友
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        private void OnFriendRemove(NetConnection<NetSession> sender, FriendRemoveRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("FriendRemove-> Character [{0}:{1}] FriendReletionID [{2}:{3}]", character.Id,character.Name, request.Id,request.friendID);
            sender.Session.Response.friendRemove = new FriendRemoveResponse();
            sender.Session.Response.friendRemove.Id = request.Id;
            if (character.FriendManager.RemoveFriendByID(request.Id))  //删除自己的好友
            {
                sender.Session.Response.friendRemove.Result = Result.Success;
                var friend = SessionManager.Instance.GetSession(request.friendID); //删除别人好友的自己
                if (friend != null)
                    friend.Session.Character.FriendManager.RemoveFriendByFriendId(character.Id); //好友在线方式
                else
                    this.RemoveFriend(request.friendID,character.Id); //好友离线删除方式
            }
            else
                sender.Session.Response.friendRemove.Result = Result.Failed;
            DBService.Instance.Save();
            sender.SendResponse();
        }

        /// <summary>
        /// 删除好友
        /// </summary>
        /// <param name="charId">自己ID</param>
        /// <param name="friendId">好友ID</param>
        private void RemoveFriend(int charId,int friendId)
        {
            var removeItem = DBService.Instance.Entities.TCharacterFriends.FirstOrDefault(v => v.TCharacterID==charId && v.FriendID==friendId);
            if (removeItem != null)
                DBService.Instance.Entities.TCharacterFriends.Remove(removeItem);
        }

    }
}
