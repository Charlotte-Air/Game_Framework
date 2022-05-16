using Models;
using Services;
using System.Linq;
using Charlotte.Proto;
using UnityEngine.Events;
using System.Collections.Generic;

namespace Managers
{
    /// <summary>
    /// 任务状态类型
    /// </summary>
    public enum NpcQuestStatus
    {
        None=0,             //无任务
        Complete=1,      //已完成可提交任务
        Avilable=2,         //可接受任务
        Incomplete=3,   //未完成任务
    }
    
    /// <summary>
    /// 任务管理器
    /// </summary>
    public class QuestManager : Singleton<QuestManager>
    {
        /// <summary>
        /// 更新任务状态事件
        /// </summary>
        public UnityAction<Quest> onQuestStatusChanged;
        /// <summary>
        /// 网络任务信息缓存
        /// </summary>
        public List<NQuestInfo> questInfos;
        /// <summary>
        /// 所有任务集合
        /// </summary>
        public Dictionary<int, Quest> allQuests = new Dictionary<int, Quest>();
        /// <summary>
        /// NPC任务进度状态集合
        /// </summary>
        public Dictionary<int,Dictionary<NpcQuestStatus,List<Quest>>> npcQuests=new Dictionary<int, Dictionary<NpcQuestStatus, List<Quest>>>();

        public void Init(List<NQuestInfo> quests)
        {
            this.questInfos = quests;
            allQuests.Clear();
            this.npcQuests.Clear();
            InitQuests();
        }

        /// <summary>
        /// 初始化任务列表
        /// </summary>
        void InitQuests()
        {
            foreach (var info in this.questInfos) //初始化已有任务
            {
                Quest quest = new Quest(info);
                this.allQuests[quest.Info.QuestId] = quest;
            }
            this.CheckAvailableQuest();
            foreach (var kv in this.allQuests)
            {
                this.AddNpcQuest(kv.Value.Define.AcceptNPC, kv.Value);
                this.AddNpcQuest(kv.Value.Define.SubmitNPC, kv.Value);
            }
        }

        /// <summary>
        /// 初始化可接任务
        /// </summary>
        void CheckAvailableQuest()
        {
            foreach (var kv in DataManager.Instance.Quests) //初始化可接任务
            {
                if (kv.Value.LimitClass != CharacterClass.None && kv.Value.LimitClass != User.Instance.CurrentCharacter.Class)
                    continue; //不符合职业
                if (kv.Value.LimitLevel > User.Instance.CurrentCharacter.Level)
                    continue; //不符合等级
                if (this.allQuests.ContainsKey(kv.Key))
                    continue; //任务已经存在
                if (kv.Value.PreQuest > 0)
                {
                    Quest preQuest;
                    if (this.allQuests.TryGetValue(kv.Value.PreQuest, out preQuest)) //获取前置任务
                    {
                        if (preQuest.Info == null)
                            continue; //前置任务未领取
                        if (preQuest.Info.Status != QuestStatus.Failed)
                            continue; //前置任务未完成
                    }
                    else
                        continue; //前置任务没接
                }
                Quest quest = new Quest(kv.Value);
                this.allQuests[quest.Define.ID] = quest;
            }
         }


        /// <summary>
        /// 添加NPC任务状态
        /// </summary>
        /// <param name="npcId">NPCID</param>
        /// <param name="quest">Quest</param>
        void AddNpcQuest(int npcId, Quest quest)
        {
            if (!this.npcQuests.ContainsKey(npcId)) //判断任务是否已经存在
                this.npcQuests[npcId] = new Dictionary<NpcQuestStatus, List<Quest>>();

            List<Quest> availables;
            List<Quest> complates;
            List<Quest> incomplates;

            if (!this.npcQuests[npcId].TryGetValue(NpcQuestStatus.Avilable, out availables))
            {
                availables = new List<Quest>();
                this.npcQuests[npcId][NpcQuestStatus.Avilable] = availables;
            }
            if (!this.npcQuests[npcId].TryGetValue(NpcQuestStatus.Complete, out complates))
            {
                complates = new List<Quest>();
                this.npcQuests[npcId][NpcQuestStatus.Complete] = complates;
            }
            if (!this.npcQuests[npcId].TryGetValue(NpcQuestStatus.Incomplete, out incomplates))
            {
                incomplates = new List<Quest>();
                this.npcQuests[npcId][NpcQuestStatus.Incomplete] = incomplates;
            }
            if (quest.Info == null) //所有任务状态拆分在不同NPC身上
            {
                if (npcId == quest.Define.AcceptNPC && !this.npcQuests[npcId][NpcQuestStatus.Avilable].Contains(quest))
                    this.npcQuests[npcId][NpcQuestStatus.Avilable].Add(quest);
            }
            else
            {
                if (quest.Define.SubmitNPC == npcId && quest.Info.Status == QuestStatus.Complated)//已经完成
                {
                    if (!this.npcQuests[npcId][NpcQuestStatus.Complete].Contains(quest))
                        this.npcQuests[npcId][NpcQuestStatus.Complete].Add(quest);
                }
                if (quest.Define.SubmitNPC == npcId && quest.Info.Status == QuestStatus.InProgress) //进行中
                {
                    if (!this.npcQuests[npcId][NpcQuestStatus.Incomplete].Contains(quest))
                        this.npcQuests[npcId][NpcQuestStatus.Incomplete].Add(quest);
                }
            }
        }

        /// <summary>
        ///  获取NPC任务进度状态
        /// </summary>
        /// <param name="npcId">NPCID</param>
        /// <returns>返回NpcQuestStatus类型</returns>
        public NpcQuestStatus GetQuestStatusByNpc(int npcId)
        {
            Dictionary<NpcQuestStatus, List<Quest>> status = new Dictionary<NpcQuestStatus, List<Quest>>();
            if (this.npcQuests.TryGetValue(npcId, out status)) //获取NPC任务 优先级：已完成->可接受->未完成
            {
                if (status[NpcQuestStatus.Complete].Count > 0)   //已经完成任务
                    return NpcQuestStatus.Complete;
                if (status[NpcQuestStatus.Avilable].Count > 0)      //可接受任务
                    return NpcQuestStatus.Avilable;
                if (status[NpcQuestStatus.Incomplete].Count > 0) //未完成任务
                    return NpcQuestStatus.Incomplete;
            }
            return NpcQuestStatus.None;
        }

        /// <summary>
        /// 打开NPC任务
        /// </summary>
        /// <param name="npcId">NPCID</param>
        /// <returns>返回NpcQuestStatus类型</returns>
        public bool OnpeNpcQuest(int npcId)
        {
            Dictionary<NpcQuestStatus, List<Quest>> status = new Dictionary<NpcQuestStatus, List<Quest>>();
            if (this.npcQuests.TryGetValue(npcId, out status))
            {
                if (status[NpcQuestStatus.Complete].Count > 0)   //已经完成任务
                    return ShowQuestDialog(status[NpcQuestStatus.Complete].First());
                if (status[NpcQuestStatus.Avilable].Count > 0)      //可接受任务
                    return ShowQuestDialog(status[NpcQuestStatus.Avilable].First());
                if (status[NpcQuestStatus.Incomplete].Count > 0) //未完成任务
                    return ShowQuestDialog(status[NpcQuestStatus.Incomplete].First());
            }
            return false;
        }

        /// <summary>
        /// 显示任务详情
        /// </summary>
        /// <param name="quest">Quest</param>
        /// <returns></returns>
        bool ShowQuestDialog(Quest quest)
        {
            if (quest.Info == null || quest.Info.Status == QuestStatus.Complated) //如果是接任务 或者 交任务 弹出任务详细框
            {
                UIQuestDialog dlg = UIManager.Instance.Show<UIQuestDialog>();
                dlg.SetQuest(quest);
                dlg.OnClose += OnQuestDialogClose;
                return true;
            }
            if (quest.Info != null || quest.Info.Status == QuestStatus.Complated) //网络Info不为空当前任务状态是完成
            {
                if (!string.IsNullOrEmpty(quest.Define.DialogIncomplete))
                    MessageBox.Show(quest.Define.DialogIncomplete);
            }
            return true;
        }

        /// <summary>
        /// 关闭任务详情处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="result"></param>
        void OnQuestDialogClose(UIWindow sender, UIWindow.WindowResult result)
        {
            UIQuestDialog dlg = (UIQuestDialog) sender;
            if (result == UIWindow.WindowResult.Yes)
            {
                if (dlg.quest.Info == null)
                    QuestService.Instance.SendQuestAccept(dlg.quest);//接任务
                else if (dlg.quest.Info.Status == QuestStatus.Complated)
                    QuestService.Instance.SendQuestSubmit(dlg.quest);//交任务
            }
            else if (result == UIWindow.WindowResult.NO)
                MessageBox.Show(dlg.quest.Define.DialogDeny);//拒绝任务
        }

        /// <summary>
        /// 更新NPC任务进度状态
        /// </summary>
        /// <param name="quest">NQuestInfo</param>
        /// <returns>Quest</returns>
        Quest RefreshQuestStatus(NQuestInfo quest)
         {
            this.npcQuests.Clear();
            Quest result;
            if (this.allQuests.ContainsKey(quest.QuestId)) //判断任务是否存在
            {
                this.allQuests[quest.QuestId].Info = quest; //更新老任务
                result = this.allQuests[quest.QuestId];
            }
            else //不存在创建
            {
                result = new Quest(quest);
                this.allQuests[quest.QuestId] = result;
            }
            CheckAvailableQuest();
            foreach (var kv in this.allQuests) //更新NPC任务状态
            {
                this.AddNpcQuest(kv.Value.Define.AcceptNPC, kv.Value);
                this.AddNpcQuest(kv.Value.Define.SubmitNPC, kv.Value);
            }
            if (onQuestStatusChanged != null)
                onQuestStatusChanged(result);
            return result;
        }

        /// <summary>
        /// 点击接受任务
        /// </summary>
        /// <param name="info">NQuestInfo</param>
        public void OnQuestAccepted(NQuestInfo info)
        {
            var quest = this.RefreshQuestStatus(info);
            MessageBox.Show(quest.Define.DialogAccept);
        }

        /// <summary>
        /// 点击提交任务
        /// </summary>
        /// <param name="info">NQuestInfo</param>
        public void OnQuestSubmited(NQuestInfo info)
        {
            var quest = this.RefreshQuestStatus(info);
            MessageBox.Show(quest.Define.DialogFinish);
        }
    }
}
