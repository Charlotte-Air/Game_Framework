using UnityEngine;
using Common.Utils;
using UnityEngine.UI;
using Charlotte.Proto;

/// <summary>
/// 公会成员列表
/// </summary>
public class UIGuildMemberItem : ListView.ListViewItem
{
    /// <summary>
    /// 名称
    /// </summary>
    public Text nickname;

    /// <summary>
    /// 职业
    /// </summary>
    public Text @class;

    /// <summary>
    /// 等级
    /// </summary>
    public Text level;

    /// <summary>
    /// 离线时间
    /// </summary>
    public Text title;

    /// <summary>
    /// 加入时间
    /// </summary>
    public Text joinTime;

    /// <summary>
    /// 状态
    /// </summary>
    public Text status;

    /// <summary>
    /// 正常状态
    /// </summary>
    public Image background;

    /// <summary>
    /// 未选中状态
    /// </summary>
    public Sprite normalBg;

    /// <summary>
    /// 选中状态
    /// </summary>
    public Sprite selectedBg;

    /// <summary>
    /// 公会成员缓存
    /// </summary>
    public NGuildMemberInfo Info;

    public override void onSelected(bool selected)
    {
        this.background.overrideSprite = selected ? normalBg : selectedBg;
    }

    /// <summary>
    /// 设置公会成员信息
    /// </summary>
    /// <param name="item">NGuildMemberInfo</param>
    public void SetGuildMemberInfo(NGuildMemberInfo item)
    {
        this.Info = item;
        if (this.nickname != null) this.nickname.text = this.Info.Info.Name;
        if (this.@class != null) this.@class.text = this.Info.Info.Class.ToString();
        if (this.level != null) this.level.text = this.Info.Info.Level.ToString();
        if (this.title != null)
        {
            if (this.Info.Title == GuildTitle.None)
                this.title.text = "成员";
            else if (this.Info.Title == GuildTitle.VicePresident)
                this.title.text = "管理员";
            else
                this.title.text = "会长";
        }
        if (this.joinTime != null) this.joinTime.text = TimeUtil.GetTime(this.Info.joinTime).ToShortDateString();
        if(this.status !=null) this.status.text=this.Info.Status==1 ? "在线" : TimeUtil.GetTime(this.Info.lastTime).ToShortDateString();
    }

}
