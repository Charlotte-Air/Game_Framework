using Common;
using UnityEngine;
using UnityEngine.UI;
using Charlotte.Proto;


/// <summary>
/// 公会信息
/// </summary>
public class UIGuildInfo : MonoBehaviour
{
    /// <summary>
    /// 公会名称
    /// </summary>
    public Text guildName;

    /// <summary>
    /// 公会ID
    /// </summary>
    public Text guildID;

    /// <summary>
    /// 公会会长
    /// </summary>
    public Text leader;

    /// <summary>
    /// 公会公告
    /// </summary>
    public Text notice;

    /// <summary>
    /// 公会成员数
    /// </summary>
    public Text memberNumber;

    private NGuildInfo info;
    /// <summary>
    /// 公会信息缓存
    /// </summary>
    public NGuildInfo Info
    {
        get { return info; }
        set
        {
            this.info = value;
            this.UpdateUI();
        }
    }

    /// <summary>
    /// 更新UI
    /// </summary>
	void UpdateUI () 
    {
        if (this.info == null)
        {
            this.guildName.text = "无";
            this.guildID.text = "ID：0";
            this.leader.text = "会长：无";
            this.notice.text = "";
            this.memberNumber.text = string.Format("成员数量：0/{0}", GameDefine.GuildMaxMemberCount);
        }
        else
        {
            this.guildName.text = this.info.GuildName;
            this.guildID.text = "ID：" + this.info.Id;
            this.leader.text = "会长：" + this.info.Leadername;
            this.notice.text = this.info.Notice;
            this.memberNumber.text = string.Format("成员数量：{0}/{1}", this.info.memberCount,GameDefine.GuildMaxMemberCount);
        }
	}
}
