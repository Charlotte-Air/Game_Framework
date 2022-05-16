using Common.Data;
using Charlotte.Proto;

namespace Models
{
    public class Quest
    {
        /// <summary>
        /// 任务表缓存
        /// </summary>
        public QuestDefine Define;

        /// <summary>
        /// 网络任务信息缓存
        /// </summary>
        public NQuestInfo Info;

        public Quest()
        {

        }

        public Quest(NQuestInfo info)
        {
            this.Info = info;
            this.Define = DataManager.Instance.Quests[info.QuestId];
        }

        public Quest(QuestDefine define)
        {
            this.Define = define;
            this.Info = null;
        }

    }
}
