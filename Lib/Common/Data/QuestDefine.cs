using Charlotte.Proto;
using System.ComponentModel;

namespace Common.Data
{
    /// <summary>
    /// 任务类型
    /// </summary>
    public enum QuestType
    {
        [Description("主线")]
        Main,
        [Description("支线")]
        Branch,
    }

    /// <summary>
    /// 任务目标类型
    /// </summary>
    public enum QuestTarget
    {
        None,
        Kill,
        Item,
    }

    /// <summary>
    /// 任务表
    /// </summary>
    public class QuestDefine
    {
        /// <summary>
        /// 任务 ID
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 任务名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 任务等级
        /// </summary>
        public int LimitLevel { get; set; }

        /// <summary>
        /// 职业限制
        /// </summary>
        public CharacterClass LimitClass { get; set; }

        /// <summary>
        /// 前置任务
        /// </summary>
        public int PreQuest { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public QuestType Type { get; set; }

        /// <summary>
        /// 接取
        /// </summary>
        public int AcceptNPC { get; set; }

        /// <summary>
        /// 提交
        /// </summary>
        public int SubmitNPC { get; set; }

        /// <summary>
        /// 概述
        /// </summary>
        public string Overview { get; set; }

        /// <summary>
        /// 对话
        /// </summary>
        public string Dialog { get; set; }
        /// <summary>
        /// 接受任务对话
        /// </summary>
        public string DialogAccept { get; set; }
        /// <summary>
        /// 拒绝任务对话
        /// </summary>
        public string DialogDeny { get; set; }
        /// <summary>
        /// 未完成对话
        /// </summary>
        public string DialogIncomplete { get; set; }
        /// <summary>
        /// 完成对话
        /// </summary>
        public string DialogFinish { get; set; }

        /// <summary>
        /// 目标类型1
        /// </summary>
        public QuestTarget Target1 { get; set; }
        /// <summary>
        /// 目标1 ID
        /// </summary>
        public int Target1ID { get; set; }
        /// <summary>
        /// 目标1 数量
        /// </summary>
        public int Target1Num { get; set; }
        /// <summary>
        /// 目标类型1
        /// </summary>
        public QuestTarget Target2 { get; set; }
        /// <summary>
        /// 目标2 ID
        /// </summary>
        public int Target2ID { get; set; }
        /// <summary>
        /// 目标2 数量
        /// </summary>
        public int Target2Num { get; set; }
        /// <summary>
        /// 目标类型1
        /// </summary>
        public QuestTarget Target3 { get; set; }
        /// <summary>
        /// 目标3 ID
        /// </summary>
        public int Target3ID { get; set; }
        /// <summary>
        /// 目标3 数量
        /// </summary>
        public int Target3Num { get; set; }

        /// <summary>
        /// 奖励金钱
        /// </summary>
        public int RewardGold { get; set; }
        /// <summary>
        /// 奖励经验
        /// </summary>
        public int RewardExp { get; set; }
        /// <summary>
        /// 奖励道具1
        /// </summary>
        public int RewardItem1 { get; set; }
        /// <summary>
        /// 奖励道具1数量
        /// </summary>
        public int RewardItem1Count { get; set; }
        /// <summary>
        /// 奖励道具2
        /// </summary>
        public int RewardItem2 { get; set; }
        /// <summary>
        /// 奖励道具2数量
        /// </summary>
        public int RewardItem2Count { get; set; }
        /// <summary>
        /// 奖励道具3
        /// </summary>
        public int RewardItem3 { get; set; }
        /// <summary>
        /// 奖励道具3数量
        /// </summary>
        public int RewardItem3Count { get; set; }
    }
}
