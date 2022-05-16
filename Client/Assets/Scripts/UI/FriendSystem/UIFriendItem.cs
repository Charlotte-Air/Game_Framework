using UnityEngine;
using UnityEngine.UI;
using Charlotte.Proto;

/// <summary>
/// 好友列表
/// </summary>
public class UIFriendItem : ListView.ListViewItem
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
    public Text Level;

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
    /// 好友信息缓存
    /// </summary>
    public NFriendInfo Info;
    public override void onSelected(bool selected)
    {
        if (selected)
            this.background.overrideSprite =selectedBg; //高亮
        else
            this.background.overrideSprite =normalBg;   //取消
    }

    /// <summary>
    /// 设置好友列表信息
    /// </summary>
    /// <param name="item">NFriendInfo</param>
    public void SetFriendInfo(NFriendInfo item)
    {
        this.Info = item;
        if (this.nickname != null) this.nickname.text = this.Info.friendInfo.Name.ToString();
        if (this.@class != null) this.@class.text = this.Info.friendInfo.Class.ToString();
        if (this.Level != null) this.Level.text = this.Info.friendInfo.Level.ToString();
        if (this.status != null) this.status.text = this.Info.Status == 1 ? "在线" : "离线";
    }
}
