using Charlotte.Proto;
using System.Collections.Generic;

namespace Managers
{   
    /// <summary>
    /// 好友管理器
    /// </summary>
    public class FriendManager : Singleton<FriendManager>
    {  
        /// <summary>
        /// 好友列表集合
        /// </summary>
        public List<NFriendInfo> allFriends = new List<NFriendInfo>();

        public void Init(List<NFriendInfo> friends)
        {
            this.allFriends = friends;
        }
    }
}
