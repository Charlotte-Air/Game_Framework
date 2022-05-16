using UnityEngine.UI;
using Charlotte.Proto;

public class UITeamItem : ListView.ListViewItem
{
    /// <summary>
    /// 队伍名称
    /// </summary>
    public Text nickname;

    /// <summary>
    /// 职业图标
    /// </summary>
    public Image classIcon;

    /// <summary>
    /// 队长图标
    /// </summary>
    public Image IeaderIcon;

    /// <summary>
    /// 队伍索引值
    /// </summary>
    public int idx;

    /// <summary>
    /// 角色信息缓存
    /// </summary>
    public NCharacterInfo Inrfo;

    /// <summary>
    /// 设置成员信息
    /// </summary>
    /// <param name="idx">索引值</param>
    /// <param name="item">成员</param>
    /// <param name="isLeader">是否是队长</param>
    public void SetMemberInfo(int idx, NCharacterInfo item, bool isLeader)
    {
        this.idx = idx;
        this.Inrfo = item;
        if (this.nickname != null) this.nickname.text = this.Inrfo.Level.ToString().PadRight(4) + this.Inrfo.Name;
        if(this.classIcon != null) this.classIcon.overrideSprite=SpriteManager.Instance.classIcons[(int)this.Inrfo.Class-1];
        if(this.IeaderIcon != null) this.IeaderIcon.gameObject.SetActive(isLeader);
    }
}
