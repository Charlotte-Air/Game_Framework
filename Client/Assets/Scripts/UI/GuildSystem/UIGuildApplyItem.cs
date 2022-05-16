using System;
using Services;
using UnityEngine.UI;
using Charlotte.Proto;

/// <summary>
/// 申请成员列表
/// </summary>
public class UIGuildApplyItem : ListView.ListViewItem 
{
    /// <summary>
    /// 名称
    /// </summary>
    public Text Name;

    /// <summary>
    /// 等级
    /// </summary>
    public Text Level;

    /// <summary>
    /// 职业
    /// </summary>
    public Text @Class;

    /// <summary>
    /// 公会申请列表缓存
    /// </summary>
    public NGuildApplyInfo Info;

    /// <summary>
    /// 设置申请成员信息
    /// </summary>
    /// <param name="info">NGuildApplyInfo</param>
    public void SetGuildInfo(NGuildApplyInfo info)
    {
        this.Info = info;
        if (this.Name != null) this.Name.text = this.Info.Name;
        if (this.Class != null) this.Class.text = this.Info.Class.ToString();
        if (this.Level != null) this.Level.text = this.Info.Level.ToString();
    }

    /// <summary>
    /// 点击同意
    /// </summary>
    public void OnAccept()
    {
        MessageBox.Show(String.Format("需要通过[{0}]的公会申请嘛?",this.Info.Name),"审批申请",MessageBoxType.Confirm,"同意","拒绝").OnYes
            = () =>
            {
                GuildService.Instance.SendGuildJoinApply(true, this.Info);
            };
    }

    /// <summary>
    /// 点击拒绝
    /// </summary>
    public void OnDeline()
    {
        MessageBox.Show(String.Format("需要拒绝[{0}]的公会申请嘛?", this.Info.Name), "审批申请", MessageBoxType.Confirm, "同意", "拒绝").OnYes
            = () =>
            {
                GuildService.Instance.SendGuildJoinApply(false, this.Info);
            };
    }
}
