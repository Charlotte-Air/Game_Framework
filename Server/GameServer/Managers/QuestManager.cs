using Network;
using System.Linq;
using Charlotte.Proto;
using Common.Data;
using GameServer.Entities;
using GameServer.Services;
using System.Collections.Generic;

namespace GameServer.Managers
{   
    /// <summary>
    /// 任务管理器
    /// </summary>
    class QuestManager
    {
        /// <summary>
        /// 所有者
        /// </summary>
        Character Owner;

        public QuestManager(Character owner)
        {
            this.Owner = owner;
        }

        /// <summary>
        /// 获取任务信息列表
        /// </summary>
        /// <param name="list">NQuestInfo</param>
        public void GetQuestInfos(List<NQuestInfo> list)
        {
            foreach (var quest in this.Owner.Data.Quests)
            {
                list.Add(GetQuestInfo(quest));
            }
        }

        /// <summary>
        /// 获取网络任务信息
        /// </summary>
        /// <param name="quest">DB TCharacterQuest</param>
        /// <returns>NQuestInfo</returns>
        public NQuestInfo GetQuestInfo(TCharacterQuest quest)
        {
            return new NQuestInfo()
            {
                QuestId = quest.QuestId,
                QuestGuid = quest.Id,
                Status = (QuestStatus)quest.Status,
                Tgrgets = new int[3]
                    {
                        quest.Target1,
                        quest.Target2,
                        quest.Target3,
                    }
            };
        }

        /// <summary>
        /// 接受任务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="questId">任务ID</param>
        /// <returns></returns>
        public Result AcceptQuest(NetConnection<NetSession> sender, int questId)
        {
            Character character = sender.Session.Character; //获取当前角色
            QuestDefine quest;
            if (DataManager.Instance.Quests.TryGetValue(questId,out quest)) //效验任务
            {
                var dbquest = DBService.Instance.Entities.TCharacterQuests.Create();
                dbquest.QuestId = quest.ID;
                if (quest.Target1 == QuestTarget.None) //判断任务是否有目标
                {
                    dbquest.Status = (int)QuestStatus.Complated; //没有目标直接完成
                }
                else
                {
                    dbquest.Status = (int)QuestStatus.InProgress; //有目标的 
                }
                sender.Session.Response.questAccept.Quest = this.GetQuestInfo(dbquest);
                character.Data.Quests.Add(dbquest);
                DBService.Instance.Save();
                return Result.Success;
            }
            else
            {
                sender.Session.Response.questAccept.Errorcode = (uint)ErrorCode.ErrorQueueNoExist;
                return Result.Failed;
            }
        }

        /// <summary>
        /// 提交任务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="questId">任务ID</param>
        /// <returns></returns>
        public Result SubmitQuest(NetConnection<NetSession> sender, int questId)
        {
            Character character = sender.Session.Character; //获取当前角色
            QuestDefine quest;
            if (DataManager.Instance.Quests.TryGetValue(questId, out quest)) //效验任务
            {
                var dbquest = character.Data.Quests.Where(q => q.QuestId == questId).FirstOrDefault();
                if (dbquest != null)
                {
                    if (dbquest.Status != (int) QuestStatus.Complated) //判断任务是否完成
                    {
                        sender.Session.Response.questSubmit.Errorcode = (uint)ErrorCode.ErrorQueueUnfinished;
                        return Result.Failed;
                    }
                    dbquest.Status = (int) QuestStatus.Finished; //设置任务结束
                    sender.Session.Response.questSubmit.Quest = this.GetQuestInfo(dbquest);
                    RewardHandle(character, quest);
                    DBService.Instance.Save();
                    return Result.Success;
                }
                sender.Session.Response.questSubmit.Errorcode = (uint)ErrorCode.ErrorQueueNoExist;
                return Result.Failed;
            }
            else
            {
                sender.Session.Response.questSubmit.Errorcode = (uint)ErrorCode.ErrorQueueNoExist;
                return Result.Failed;
            }
        }

        void RewardHandle(Character character, QuestDefine quest)
        {
            if (quest.RewardGold > 0)
            {
                character.Gold += quest.RewardGold;
            }
            if (quest.RewardExp > 0)
            {
                //character.Exp += quest.RewardExp;
            }
            if (quest.RewardItem1 > 0)
            {
                character.ItemManager.AddItem(quest.RewardItem1, quest.RewardItem1Count);
            }
            if (quest.RewardItem2 > 0)
            {
                character.ItemManager.AddItem(quest.RewardItem2, quest.RewardItem2Count);
            }
            if (quest.RewardItem3 > 0)
            {
                character.ItemManager.AddItem(quest.RewardItem3, quest.RewardItem3Count);
            }
        }
    }
}
