using UnityEngine;
using UnityEngine.UI;
using Charlotte.Proto;

/// <summary>
/// 公会列表
/// </summary>
public class UIGuildItem : ListView.ListViewItem
{
    public Text Id;
    public Text Name;
    public Text count;
    public Text leader;
    public Image background;
    public Sprite normalBg;
    public Sprite selectedBg;
    public NGuildInfo Info;

    public override void onSelected(bool selected)
    {
        this.background.overrideSprite = selected ? normalBg : selectedBg;
    }

    /// <summary>
    /// 设置公会信息
    /// </summary>
    /// <param name="item">NGuildInfo</param>
    public void SetGuildInfo(NGuildInfo item)
    {
        this.Info = item;
        if (this.Id != null) this.Id.text = this.Info.Id.ToString();
        if (this.Name != null) this.Name.text = this.Info.GuildName.ToString();
        if (this.count != null) this.count.text = this.Info.memberCount.ToString();
        if (this.leader != null) this.leader.text = this.Info.Leadername.ToString();
    }
}
