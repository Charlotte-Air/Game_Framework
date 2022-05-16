using Models;
using Managers;
using UnityEngine;
using Common.Data;
using UnityEngine.UI;
using Charlotte.Proto;

/// <summary>
/// 任务信息
/// </summary>
public class UIQuestInfo : MonoBehaviour
{
    /// <summary>
    /// 标题
    /// </summary>
	public Text title;

    /// <summary>
    /// 目标标题
    /// </summary>
    public Text TargetTitle;

    /// <summary>
    /// 任务目标
    /// </summary>
    public Text[] targets;

    /// <summary>
    /// 任务描述
    /// </summary>
	public Text description;

    /// <summary>
    /// 任务奖励
    /// </summary>
	public Image[] rewardItems;

    /// <summary>
    /// 道具预设体
    /// </summary>
    public GameObject item;

    /// <summary>
    /// 奖励金币
    /// </summary>
	public Text rewardMoney;

	/// <summary>
	/// 奖励经验
	/// </summary>
	public Text rewardExp;

    /// <summary>
    /// 导航
    /// </summary>
    public Button NavButton;

    /// <summary>
    /// Data缓存
    /// </summary>
    private DataManager data = DataManager.Instance;

    /// <summary>
    /// NPC索引值
    /// </summary>
    private int npc = 0;

    /// <summary>
    /// 设置任务信息
    /// </summary>
    /// <param name="quest">Quest</param>
    public void SetQuestInfo(Quest quest)
    {
        if (quest.Define.Type == QuestType.Main)
            this.title.text = string.Format("[主线] {0}", quest.Define.Name);
        else
            this.title.text = string.Format("[支线] {0}", quest.Define.Name);
        if (quest.Info == null)
            this.description.text = quest.Define.Dialog;
        else
        {
            if (quest.Info.Status == QuestStatus.Complated)
                this.description.text = quest.Define.DialogFinish;
        }
        if (quest.Define.Target1 != QuestTarget.None)
        {
            this.TargetTitle.gameObject.SetActive(true);
            if (quest.Info != null)
            {
                if (quest.Define.Target1 == QuestTarget.Kill)
                {
                    if (quest.Define.Target1Num > 0)
                    {
                        this.targets[0].text = string.Format("[{0}]  {1} / {2}", quest.Define.Overview.ToString(), quest.Define.Target1Num.ToString(), quest.Info.Tgrgets[0].ToString());
                        this.targets[0].gameObject.SetActive(true);
                    }
                    if (quest.Define.Target2Num > 0)
                    {
                        this.targets[1].text = string.Format("[{0}]  {1} / {2}", quest.Define.Overview.ToString(), quest.Define.Target2Num.ToString(), quest.Info.Tgrgets[1].ToString());
                        this.targets[1].gameObject.SetActive(true);
                    }
                    if (quest.Define.Target3Num > 0)
                    {
                        this.targets[2].text = string.Format("[{0}]  {1} / {2}", quest.Define.Overview.ToString(), quest.Define.Target3Num.ToString(), quest.Info.Tgrgets[2].ToString());
                        this.targets[2].gameObject.SetActive(true);
                    }
                }
                else if (quest.Define.Target1 == QuestTarget.Item)
                {
                    if (quest.Define.Target1Num > 0)
                    {
                        this.targets[0].text = string.Format("[{0}]  {1} / {2}", quest.Define.Overview.ToString(), quest.Define.Target1Num.ToString(), quest.Info.Tgrgets[0].ToString());
                        this.targets[0].gameObject.SetActive(true);
                    }
                    if (quest.Define.Target2Num > 0)
                    {
                        this.targets[1].text = string.Format("[{0}]  {1} / {2}", quest.Define.Overview.ToString(), quest.Define.Target2Num.ToString(), quest.Info.Tgrgets[1].ToString());
                        this.targets[1].gameObject.SetActive(true);
                    }
                    if (quest.Define.Target3Num > 0)
                    {
                        this.targets[2].text = string.Format("[{0}]  {1} / {2}", quest.Define.Overview.ToString(), quest.Define.Target3Num.ToString(), quest.Info.Tgrgets[2].ToString());
                        this.targets[2].gameObject.SetActive(true);
                    }
                }
            }
            else
            {
                if (quest.Define.Target1 == QuestTarget.Kill)
                {
                    if (quest.Define.Target1Num > 0)
                    {
                        this.targets[0].text = string.Format("[{0}]  {1} / 0", quest.Define.Overview.ToString(), quest.Define.Target1Num.ToString());
                        this.targets[0].gameObject.SetActive(gameObject);
                    }
                    if (quest.Define.Target2Num > 0)
                    {
                        this.targets[1].text = string.Format("[{0}]  {1} / 0", quest.Define.Overview.ToString(), quest.Define.Target2Num.ToString());
                        this.targets[1].gameObject.SetActive(gameObject);
                    }
                    if (quest.Define.Target3Num > 0)
                    {
                        this.targets[2].text = string.Format("[{0}]  {1} / 0", quest.Define.Overview.ToString(), quest.Define.Target3Num.ToString());
                        this.targets[2].gameObject.SetActive(gameObject);
                    }
                }
                else if (quest.Define.Target1 == QuestTarget.Item)
                {
                    if (quest.Define.Target1Num > 0)
                    {
                        this.targets[0].text = string.Format("[{0}]  {1} / 0", quest.Define.Overview.ToString(), quest.Define.Target1Num.ToString());
                        this.targets[0].gameObject.SetActive(gameObject);
                    }
                    if (quest.Define.Target2Num > 0)
                    {
                        this.targets[1].text = string.Format("[{0}]  {1} / 0", quest.Define.Overview.ToString(), quest.Define.Target2Num.ToString());
                        this.targets[1].gameObject.SetActive(gameObject);
                    }
                    if (quest.Define.Target3Num > 0)
                    {
                        this.targets[2].text = string.Format("[{0}]  {1} / 0", quest.Define.Overview.ToString(), quest.Define.Target3Num.ToString());
                        this.targets[2].gameObject.SetActive(gameObject);
                    }
                }
            }
        }
        else
        {
            this.TargetTitle.gameObject.SetActive(false);
            this.targets[0].gameObject.SetActive(false);
            this.targets[1].gameObject.SetActive(false);
            this.targets[2].gameObject.SetActive(false);
        }
        if (quest.Define.RewardItem1 > 0)
        {
            var set = rewardItems[0].gameObject.transform.GetComponentInChildren<UIRewardItem>();
            if (set != null)
            {
                var def = data.Items[quest.Define.RewardItem1];
                set.SetRewardItem(def.Icon, quest.Define.RewardItem1Count.ToString(), def.ID);
            }
            else
            {
                GameObject go = Instantiate(item, rewardItems[0].transform);
                var ui = go.GetComponent<UIRewardItem>();
                var def = data.Items[quest.Define.RewardItem1];
                ui.SetRewardItem(def.Icon, quest.Define.RewardItem1Count.ToString(), def.ID);
            }
        }
        if (quest.Define.RewardItem2 > 0)
        {
            var set = rewardItems[1].gameObject.transform.GetComponentInChildren<UIRewardItem>();
            if (set != null)
            {
                var def = data.Items[quest.Define.RewardItem2];
                set.SetRewardItem(def.Icon, quest.Define.RewardItem2Count.ToString(), def.ID);
            }
            else
            {
                GameObject go = Instantiate(item, rewardItems[1].transform);
                var ui = go.GetComponent<UIRewardItem>();
                var def = data.Items[quest.Define.RewardItem2];
                ui.SetRewardItem(def.Icon, quest.Define.RewardItem2Count.ToString(), def.ID);
            }
        }
        if (quest.Define.RewardItem3 > 0)
        {
            var set = rewardItems[2].gameObject.transform.GetComponentInChildren<UIRewardItem>();
            if (set != null)
            {
                var def = data.Items[quest.Define.RewardItem3];
                set.SetRewardItem(def.Icon, quest.Define.RewardItem3Count.ToString(), def.ID);
            }
            else
            {
                GameObject go = Instantiate(item, rewardItems[2].transform);   //实例化，副节点设置为槽
                var ui = go.GetComponent<UIRewardItem>();
                var def = data.Items[quest.Define.RewardItem3]; //从道具管理器拿取配置表数据
                ui.SetRewardItem(def.Icon, quest.Define.RewardItem3Count.ToString(), def.ID); //设置图标和数量
            }
        }
        this.description.text = quest.Define.Overview.ToString();
        this.rewardMoney.text = quest.Define.RewardGold.ToString();
        this.rewardExp.text = quest.Define.RewardExp.ToString();

        if (quest.Info == null)  //找接任务NPC
            this.npc = quest.Define.AcceptNPC;
        else if (quest.Info.Status == QuestStatus.Complated) //找任务完成的NPC
            this.npc = quest.Define.SubmitNPC;

        foreach (var fitter in this.GetComponentsInChildren<ContentSizeFitter>()) //强制布局
        {
            fitter.SetLayoutVertical();
        }
    }

    /// <summary>
    /// 点击导航
    /// </summary>
    public void OnClickNav()
    {
        if (this.npc !=0)
        {
            Vector3 pos = NPCManager.Instance.GetNpcPosition(this.npc);
            User.Instance.CurrentCharacterObject.StartNav(pos);
            UIManager.Instance.Close<UIQuestSystem>();
        }
        else
            MessageBox.Show("抱歉~请先选择任务再开始导航", "提示", MessageBoxType.Information);
    }
}
