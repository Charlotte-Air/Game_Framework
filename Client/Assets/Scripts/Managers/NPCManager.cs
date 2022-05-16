using UnityEngine;
using Common.Data;
using System.Collections.Generic;

namespace Managers
{
    /// <summary>
    /// NPC管理器
    /// </summary>
    class NPCManager : Singleton<NPCManager>
    {
        /// <summary>
        /// 交互事件
        /// </summary>
        /// <param name="npc"></param>
        /// <returns></returns>
        public delegate bool NpcActionHandler(NpcDefine npc);
        /// <summary>
        /// 交互事件集合
        /// </summary>
        private Dictionary<NpcFunction, NpcActionHandler> evntMap = new Dictionary<NpcFunction, NpcActionHandler>();
        /// <summary>
        /// NPC位置集合
        /// </summary>
        private Dictionary<int, Vector3> npcPositions = new Dictionary<int, Vector3>();

        /// <summary>
        ///NPC事件注册
        /// </summary>
        public void RegisterNpcEvent(NpcFunction function, NpcActionHandler action)
        {
            if (!evntMap.ContainsKey(function))
                evntMap[function] = action;
            else
                evntMap[function] += action;
        }

        /// <summary>
        /// 获取NPC数据
        /// </summary>
        /// <param name="npcID">NPCID</param>
        /// <returns>NpcDefine</returns>
        public NpcDefine GetNpcDefine(int npcID)
        {
            NpcDefine npc = null;
            DataManager.Instance.Npcs.TryGetValue(npcID, out npc);
            return npc;
        }

        /// <summary>
        /// 交互事件
        /// </summary>
        /// <param name="npcID">NPCID</param>
        /// <returns></returns>
        public bool Interactive(int npcID)
        {
            if (DataManager.Instance.Npcs.ContainsKey(npcID))
            { 
                var npc = DataManager.Instance.Npcs[npcID];
                return Interactive(npc);
            }
            return false;
        }

        /// <summary>
        ///  交互事件
        /// </summary>
        /// <param name="npc">NpcDefine</param>
        /// <returns></returns>
        public bool Interactive(NpcDefine npc)
        {
            if (DoTaskInteractive(npc)) //任务交互
                return true;
            else if(npc.Type == NpcType.Functional) //功能交互
                return DoFunctionInteractive(npc); 
            return false;
        }

        /// <summary>
        /// 执行任务交互
        /// </summary>
        /// <param name="npc">NpcDefine</param>
        /// <returns></returns>
        private bool DoTaskInteractive(NpcDefine npc)
        {
            var status = QuestManager.Instance.GetQuestStatusByNpc(npc.ID);
            if (status == NpcQuestStatus.None)
                return false;
            else if (status == NpcQuestStatus.Incomplete && npc.Type == NpcType.Functional)
                return false;
            return QuestManager.Instance.OnpeNpcQuest(npc.ID);
        }

        /// <summary>
        /// 执行功能交互
        /// </summary>
        /// <param name="npc">NpcDefine</param>
        /// <returns></returns>
        private bool DoFunctionInteractive(NpcDefine npc)
        {
            if (npc.Type != NpcType.Functional)
                return false;
            if (!evntMap.ContainsKey(npc.Function))
                return false;
            return evntMap[npc.Function](npc);
        }

        /// <summary>
        /// 更新NPC位置
        /// </summary>
        /// <param name="npc">NPCID</param>
        /// <param name="pos">Vector3.Pos</param>
        internal void UpdateNpcPosition(int npc, Vector3 pos)
        {
            this.npcPositions[npc] = pos;
        }

        /// <summary>
        /// 获取NPC位置
        /// </summary>
        /// <param name="npc">NPCID</param>
        /// <returns></returns>
        internal Vector3 GetNpcPosition(int npc)
        {
            return this.npcPositions[npc];
        }
    }
}
