using Common;
using Network;
using System.Collections.Generic;

namespace GameServer.Managers
{
    /// <summary>
    /// 消息管理器
    /// </summary>
    class SessionManager : Singleton<SessionManager>
    {
        /// <summary>
        /// 消息集合Key(角色ID) Value(NetSession)
        /// </summary>
        Dictionary<int, NetConnection<NetSession>> characterSessions = new Dictionary<int, NetConnection<NetSession>>();

        /// <summary>
        /// 添加玩家消息
        /// </summary>
        /// <param name="characterId"></param>
        /// <param name="session"></param>
        public void AddSession(int CharacterID, NetConnection<NetSession> session)
        {
            if(this.characterSessions.ContainsKey(CharacterID))
            {
                this.characterSessions[CharacterID] = session;
            }
            else
            {
                this.characterSessions.Add(CharacterID, session);
            }
        }

        /// <summary>
        /// 删除玩家消息
        /// </summary>
        /// <param name="characterId">角色ID</param>
        public void RemovSession(int CharacterID) => this.characterSessions.Remove(CharacterID);

        /// <summary>
        /// 获取玩家消息
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns></returns>
        public NetConnection<NetSession> GetSession(int characterId)
        {
            NetConnection<NetSession> session = null;
            this.characterSessions.TryGetValue(characterId, out session);
            return session;
        }
    }
}
