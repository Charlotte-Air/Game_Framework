using Models;
using Services;
using Managers;
using UnityEngine;

/// <summary>
/// 好友系统
/// </summary>
public class UIFriends : UIWindow
{
    /// <summary>
    /// 好友列表预设体
    /// </summary>
    public GameObject itemPrefab;

    /// <summary>
    /// 视图列表
    /// </summary>
    public ListView IistMain;

    /// <summary>
    /// 视图列表子节点
    /// </summary>
    public Transform itemRoot;

    /// <summary>
    /// 好友列表对象
    /// </summary>
    public UIFriendItem selectedItem;

	void Start ()
    {
        FriendService.Instance.OnFriendUpdate += RefreshUI;
        this.IistMain.onItemSelected += this.OnFriendSelected;
        RefreshUI();
	}
    void OnDestroy()
    {
        FriendService.Instance.OnFriendUpdate -= RefreshUI;
    }
    public void OnFriendSelected(ListView.ListViewItem item)
    {
        this.selectedItem = item as UIFriendItem;
    }

    /// <summary>
    /// 点击添加好友
    /// </summary>
    public void OnClickFriendAdd()
    {
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Button2);
        InputBox.Show("输入需要添加的好友名称或ID", "添加好友").OnSubmit += OnFriendAddSubmit;
    }

    /// <summary>
    /// 添加好友
    /// </summary>
    /// <param name="input">输入内容</param>
    /// <param name="tips">提示信息</param>
    /// <returns></returns>
    private bool OnFriendAddSubmit(string input, out string tips)
    {
        tips = "";
        int friendId = 0;
        string friendName = "";
        if (!int.TryParse(input, out friendId)) //解析输入是ID还是Name
            friendName = input;
        if (friendId == User.Instance.CurrentCharacter.Id || friendName == User.Instance.CurrentCharacter.Name)
        {
            tips = "抱歉！无法添加自己";
            return false;
        }
        FriendService.Instance.SendFriendAddRequest(friendId, friendName);
        return true;
    }

    /// <summary>
    /// 点击私聊
    /// </summary>
    public void OnClickFriendChar()
    {
        if (selectedItem == null)
            MessageBox.Show("请选择私聊的成员");
        else
        {
            SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Button2);
            ChatManager.Instance.StartPrivateChat(selectedItem.Info.friendInfo.Id, selectedItem.Info.friendInfo.Name);
        }
    }

    /// <summary>
    /// 点击邀请组队
    /// </summary>
    public void OnClickFriendTeamInvite()
    {
        if(selectedItem ==null)
        {
            MessageBox.Show("请选择需要邀请的好友");
            return;
        }

        if (selectedItem.Info.Status == 0)
        {
            MessageBox.Show("请选择在线的好友");
            return;
        }
        MessageBox.Show(string.Format("确定需要邀请好友[{0}]加入队伍嘛？", selectedItem.Info.friendInfo.Name), "邀请好友组队", MessageBoxType.Confirm, "邀请", "取消").OnYes =
            () =>
            {
                TeamService.Instance.SendTeamInviteRequest(this.selectedItem.Info.friendInfo.Id,this.selectedItem.Info.friendInfo.Name);
            };
    }

    /// <summary>
    /// 点击删除好友
    /// </summary>
    public void OnClickFriendRemove()
    {
        if (selectedItem == null)
        {
            MessageBox.Show("请选择需要删除的好友");
            return;
        }
        MessageBox.Show(string.Format("确定需要删除好友[{0}]嘛?",selectedItem.Info.friendInfo.Name),"删除好友",MessageBoxType.Confirm,"删除","取消").OnYes = () => 
        {
            FriendService.Instance.SendFriendRemoveRequest(this.selectedItem.Info.Id,this.selectedItem.Info.friendInfo.Id);
        };
    }

    /// <summary>
    /// 刷新UI
    /// </summary>
    void RefreshUI()
    {
        if (IistMain != null)
        {
            ClearFriendList();
            InitFriendItems();
        }
    }

    /// <summary>
    /// 初始化好友列表
    /// </summary>
    void InitFriendItems()
    {
        foreach (var item in FriendManager.Instance.allFriends)
        {
            GameObject go = Instantiate(itemPrefab, this.IistMain.transform);
            UIFriendItem ui = go.GetComponent<UIFriendItem>();
            ui.SetFriendInfo(item);
            this.IistMain.AddItem(ui);
        }
    }

    /// <summary>
    /// 清空好友列表
    /// </summary>
    void ClearFriendList()
    {
        this.IistMain.RemoveAll();
    }
}
