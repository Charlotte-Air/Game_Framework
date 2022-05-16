using Models;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 任务列表
/// </summary>
public class UIQuestItem : ListView.ListViewItem
{
    /// <summary>
    /// 标签
    /// </summary>
    public Text title;

    /// <summary>
    /// 背景
    /// </summary>
    public Image background;

    /// <summary>
    /// 正常状态
    /// </summary>
    public Sprite normalBg;

    /// <summary>
    /// 选中状态
    /// </summary>
    public Sprite selectedBg;

    /// <summary>
    /// 任务信息缓存
    /// </summary>
    public Quest quest;

    public override void onSelected(bool selected)
    {
        this.background.overrideSprite = selected ? selectedBg : normalBg;
    }

    /// <summary>
    /// 设置任务信息
    /// </summary>
    public void SetQuestItemInfo(Quest item)
    {
        this.quest = item;
        if (this.title != null) this.title.text = this.quest.Define.Name;
    }

}
