using Services;
using Managers;
using UnityEngine;
using Charlotte.Proto;

/// <summary>
/// 公会系统
/// </summary>
public class UIGuild : UIWindow
{
    /// <summary>
    /// 公会成员预设体
    /// </summary>
    public GameObject itemPrefab;

    /// <summary>
    /// 普通权限组
    /// </summary>
    public GameObject panelmember;

    /// <summary>
    /// 管理权限组
    /// </summary>
    public GameObject panelAdmin;

    /// <summary>
    /// 会长权限组
    /// </summary>
    public GameObject panelLeader;

    /// <summary>
    /// 视图对象
    /// </summary>
    public ListView listMain;

    /// <summary>
    /// 视图对象子节点
    /// </summary>
    public Transform itemRoot;

    /// <summary>
    /// 公会信息
    /// </summary>
    public UIGuildInfo uiInfo;

    /// <summary>
    /// 成员列表
    /// </summary>
    public UIGuildMemberItem selectedItem;

	void Start ()
    {
        GuildService.Instance.OnGuildUpdate += UpdateUI; //更新公会信息
        GuildService.Instance.OnLeaveGuild += OnLeaveGuild;
        this.listMain.onItemSelected += this.OnGuildMemberSelected;
        this.UpdateUI();
    }

    void OnDestroy()
    {
        GuildService.Instance.OnGuildUpdate -= UpdateUI;
        GuildService.Instance.OnLeaveGuild -= OnLeaveGuild;
    }

    void UpdateUI ()
    {
        if (GuildManager.Instance.guildInfo != null && GuildManager.Instance.myMemberInfo != null)
        {
            this.uiInfo.Info = GuildManager.Instance.guildInfo;
            ClearList();
            InitItem();
            this.panelmember.SetActive(GuildManager.Instance.myMemberInfo.Title == GuildTitle.None);
            this.panelAdmin.SetActive(GuildManager.Instance.myMemberInfo.Title == GuildTitle.VicePresident);
            this.panelLeader.SetActive(GuildManager.Instance.myMemberInfo.Title == GuildTitle.President);
        }
    }

    void OnUpdateInfo()
    {
        this.Close(WindowResult.Yes);
        UIMain.Instance.OnClickGuild();
    }
    public void OnGuildMemberSelected(ListView.ListViewItem item)
    {
        this.selectedItem = item as UIGuildMemberItem;
    }

    /// <summary>
    /// 初始化公会成员列表
    /// </summary>
    public void InitItem()
    {
        foreach (var item in GuildManager.Instance.guildInfo.Members)
        {
            GameObject go = Instantiate(itemPrefab, this.listMain.transform);
            UIGuildMemberItem ui = go.GetComponent<UIGuildMemberItem>();
            ui.SetGuildMemberInfo(item);
            this.listMain.AddItem(ui);
        }
    }

    /// <summary>
    /// 清除成员列表
    /// </summary>
    public void ClearList()
    {
        this.listMain.RemoveAll();
    }

    /// <summary>
    /// 点击申请列表
    /// </summary>
    public void OnClickAppliesList()
    {
        UIManager.Instance.Show<UIGuildApplyList>();
    }

    /// <summary>
    /// 点击离开公会
    /// </summary>
    public void OnClickLeave()
    {
        MessageBox.Show(string.Format("确定需要离开[{0}]嘛？", this.uiInfo.guildName.text.ToString()), "离开公会", MessageBoxType.Confirm,"确定","取消").OnYes =
            () =>
            {
                GuildService.Instance.SendGuildLeaveRequest();
                this.Close();
            };
    }

    /// <summary>
    /// 离开公会回调
    /// </summary>
    /// <param name="result"></param>
    void OnLeaveGuild()
    {
        this.Close(WindowResult.Yes);
    }

    /// <summary>
    /// 点击私聊成员
    /// </summary>
    public void OnClickChat()
    {
        if (selectedItem == null)
            MessageBox.Show("请选择私聊的成员");
        else
        {
            SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Button2);
            ChatManager.Instance.StartPrivateChat(selectedItem.Info.Info.Id, selectedItem.Info.Info.Name);
        }
    }

    /// <summary>
    /// 点击踢出成员
    /// </summary>
    public void OnClickKickout()
    {
        if (selectedItem == null)
        {
            MessageBox.Show("请选择需要踢出的成员");
            return;
        }
        MessageBox.Show(string.Format("需要把[{0}]踢出公会嘛？",this.selectedItem.Info.Info.Name),"踢出公会",MessageBoxType.Confirm).OnYes =
            () =>
            {
                GuildService.Instance.SendAdminCommand(GuildAdminCommand.Kickout, this.selectedItem.Info.Info.Id);
            };
    }

    /// <summary>
    /// 点击晋升成员
    /// </summary>
    public void OnClickPromote()
    {
        if (selectedItem == null)
        {
            MessageBox.Show("请选择需要晋升的成员");
            return;
        }

        if (selectedItem.Info.Title != GuildTitle.None)
        {
            MessageBox.Show("对方职位已有");
            return;
        }
        MessageBox.Show(string.Format("需要晋升[{0}]为副公长嘛？", this.selectedItem.Info.Info.Name), "晋升", MessageBoxType.Confirm).OnYes =
            () =>
            {
                GuildService.Instance.SendAdminCommand(GuildAdminCommand.Promote, this.selectedItem.Info.Info.Id);
            };
    }

    /// <summary>
    /// 点击罢免成员
    /// </summary>
    public void OnClickDepose()
    {
        if (selectedItem == null)
        {
            MessageBox.Show("请选择需要罢免的成员");
            return;
        }

        if (selectedItem.Info.Title == GuildTitle.None)
        {
            MessageBox.Show("对方已经无职可免");
            return;
        }
        if (selectedItem.Info.Title == GuildTitle.President)
        {
            MessageBox.Show("抱歉,无法罢免会长");
            return;
        }
        MessageBox.Show(string.Format("需要罢免[{0}]公会职务嘛？", this.selectedItem.Info.Info.Name), "罢免", MessageBoxType.Confirm).OnYes =
            () =>
            {
                GuildService.Instance.SendAdminCommand(GuildAdminCommand.Depost, this.selectedItem.Info.Info.Id);
            };
    }

    /// <summary>
    /// 点击转让会长
    /// </summary>
    public void OnClickTransfer()
    {
        if (selectedItem == null)
        {
            MessageBox.Show("请选择需要转让的成员");
            return;
        }
        MessageBox.Show(string.Format("需要把会长转让给[{0}]嘛？", this.selectedItem.Info.Info.Name), "转让会长", MessageBoxType.Confirm).OnYes =
            () =>
            {
                GuildService.Instance.SendAdminCommand(GuildAdminCommand.Tansfer, this.selectedItem.Info.Info.Id);
            };
    }

    /// <summary>
    /// 点击设置公会介绍
    /// </summary>
    public void OnClickSetNotice()
    {
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Button1);
        var bo = InputBox.Show("请输入公会介绍", "修改介绍", "修改");
        if (bo != null)
        {
            this.uiInfo.notice.text = bo.input.text;
        }
    }
    

}
