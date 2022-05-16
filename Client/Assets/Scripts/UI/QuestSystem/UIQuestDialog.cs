using Models;
using UnityEngine;
using Charlotte.Proto;

/// <summary>
/// 任务对话框
/// </summary>
public class UIQuestDialog : UIWindow
{
    public Quest quest;
    /// <summary>
    /// 任务信息
    /// </summary>
    public UIQuestInfo questInfo;

    /// <summary>
    /// 接受任务面板
    /// </summary>
    public GameObject openButtons;

    /// <summary>
    /// 提交任务面板
    /// </summary>
    public GameObject submitButtons;

    /// <summary>
    /// 设置任务对话框
    /// </summary>
    public void SetQuest(Quest quest)
    {
        this.quest = quest;
        this.questInfo.SetQuestInfo(quest);
        if (this.quest.Info == null) //判断是否新任务
        {
            openButtons.SetActive(true);
            submitButtons.SetActive(false);
        }
        else
        {
            if (this.quest.Info.Status == QuestStatus.Complated)
            {
                openButtons.SetActive(false);
                submitButtons.SetActive(true);
            }
            else
            {
                openButtons.SetActive(false);
                submitButtons.SetActive(false);
            }
        }
    }

}
