using Common;
using System.Linq;
using Charlotte.Proto;
using GameServer.Entities;
using GameServer.Services;
using System.Collections.Generic;

namespace GameServer.Managers
{  
    /// <summary>
    /// 好友管理器
    /// </summary>
    class FriendManager
    {
        /// <summary>
        /// 所有者
        /// </summary>
        Character Owner;
        /// <summary>
        /// 好友集合
        /// </summary>
        List<NFriendInfo> friends = new List<NFriendInfo>();
        /// <summary>
        /// 好友状态是否改变
        /// </summary>
        private bool friendChanged = false;

        public FriendManager(Character owner)
        {
            this.Owner = owner;
            this.InitFriends();
        }

        /// <summary>
        /// 获取好友信息
        /// </summary>
        /// <param name="list">NFriendInfo</param>
        public void GetFriendInfos(List<NFriendInfo> list)
        {
            foreach (var f in this.friends)
            {
                list.Add(f);
            }
        }

        /// <summary>
        /// 初始化好友列表
        /// </summary>
        public void InitFriends()
        {
            this.friends.Clear();
            foreach (var friend in this.Owner.Data.Friends)
            {
                this.friends.Add(GetFriendInfo(friend));
            }
        }

        /// <summary>
        /// 添加好友
        /// </summary>
        /// <param name="friend">好友角色</param>
        public void AddFriend(Character friend)
        {
            TCharacterFriend tf = new TCharacterFriend()
            {
                FriendID = friend.Id,
                FriendName = friend.Data.Name,
                Class = friend.Data.Class,
                Level = friend.Data.Level,
            };
            this.Owner.Data.Friends.Add(tf);
            friendChanged = true;
        }

        /// <summary>
        /// 删除对方好友列表中的自己
        /// </summary>
        /// <param name="friendid">自己ID</param>
        /// <returns></returns>
        public bool RemoveFriendByFriendId(int friendid)
        {
            var removeItem = this.Owner.Data.Friends.FirstOrDefault(v => v.FriendID == friendid);
            if (removeItem != null)
            {
                DBService.Instance.Entities.TCharacterFriends.Remove(removeItem);
            }
            friendChanged = true;
            return true;
        }

        /// <summary>
        /// 删除自己列表中对方好友
        /// </summary>
        /// <param name="id">好友ID</param>
        /// <returns></returns>
        public bool RemoveFriendByID(int id)
        {
            var removeItem = this.Owner.Data.Friends.FirstOrDefault(v => v.Id == id);
            if (removeItem != null)
            {
                DBService.Instance.Entities.TCharacterFriends.Remove(removeItem);
            }
            friendChanged = true;
            return true;
        }

        /// <summary>
        /// 获取好友信息
        /// </summary>
        /// <param name="friend">TCharacterFriend</param>
        /// <returns>NFriendInfo</returns>
        public NFriendInfo GetFriendInfo(TCharacterFriend friend)
        {
            NFriendInfo friendInfo = new NFriendInfo();
            var character = CharacterManager.Instance.GetCharacter(friend.FriendID);
            friendInfo.friendInfo = new NCharacterInfo();
            friendInfo.Id = friend.Id;
            if (character == null)
            {
                friendInfo.friendInfo.Id = friend.FriendID;
                friendInfo.friendInfo.Name = friend.FriendName;
                friendInfo.friendInfo.Class = (CharacterClass)friend.Class;
                friendInfo.friendInfo.Level = friend.Level;
                friendInfo.Status = 0;
            }
            else
            {
                friendInfo.friendInfo = character.GetBasicInfo();
                friendInfo.friendInfo.Name = character.Info.Name;
                friendInfo.friendInfo.Class = character.Info.Class;
                friendInfo.friendInfo.Level = character.Info.Level;
                if (friend.Level != character.Info.Level)
                    friend.Level = character.Info.Level;

                character.FriendManager.UpdateFriendInfo(this.Owner.Info, 1);
                friendInfo.Status = 1;
            }
            Log.InfoFormat("Owner [{0}:{1}] GetFriendInfo [{2}:{3}] ",this.Owner.Id, this.Owner.Info.Name, friendInfo.friendInfo.Id , friendInfo.Status);
            return friendInfo;
        }

        /// <summary>
        /// 获取好友信息
        /// </summary>
        /// <param name="frinedId">好友ID</param>
        /// <returns></returns>
        public NFriendInfo GetFriendInfo(int frinedId)
        {
            foreach (var kv in this.friends)
            {
                if (kv.friendInfo.Id == frinedId)
                    return kv;
            }
            return null;
        }

        /// <summary>
        /// 更新好友列表信息
        /// </summary>
        /// <param name="friendinfo">NCharacterInfo</param>
        /// <param name="status">状态</param>
        public void UpdateFriendInfo(NCharacterInfo friendinfo, int status)
        {
            foreach (var kv in this.friends)
            {
                if (kv.friendInfo.Id == friendinfo.Id) //更新在线状态
                {
                    kv.Status = status;
                    break;
                }
            }
            this.friendChanged = true;
        }

        /// <summary>
        /// 好友离线事件
        /// </summary>
        public void offlineNotify()
        {
            foreach (var friendInfo in this.friends)
            {
                var friend = CharacterManager.Instance.GetCharacter(friendInfo.friendInfo.Id);
                if (friend != null)
                    friend.FriendManager.UpdateFriendInfo(this.Owner.Info,0); //自己的信息更新给好友
            }
        }

        /// <summary>
        /// 后处理事件
        /// </summary>
        /// <param name="message"></param>
        public void PostProcess(NetMessageResponse message)
        {
            if (friendChanged)
            {
                Log.InfoFormat("FriendManager PostProcess -> Character [{0}:{1}]", this.Owner.Id,this.Owner.Info.Name);
                this.InitFriends();
                if (message.friendList == null)
                {
                    message.friendList = new FriendListResponse();
                    message.friendList.Friends.AddRange(this.friends); //当前好友列表添加一遍
                }
                friendChanged = false;
            }
        }


    }
}
