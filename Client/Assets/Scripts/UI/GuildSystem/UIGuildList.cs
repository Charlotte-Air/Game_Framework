using Services;
using UnityEngine;
using Charlotte.Proto;
using System.Collections.Generic;

public class UIGuildList : UIWindow
{
    /// <summary>
    /// 公会列表预设体
    /// </summary>
    public GameObject itemPrefab;

    /// <summary>
    /// 视图对象
    /// </summary>
    public ListView listMain;

    /// <summary>
    /// 视图节点
    /// </summary>
    public Transform itemRoot;

    /// <summary>
    /// 公会信息
    /// </summary>
    public UIGuildInfo uiInfo;

    /// <summary>
    /// 公会列表
    /// </summary>
    public UIGuildItem selectedItem;

	void Start ()
    {
        this.listMain.onItemSelected += this.OnGuildSelected;                //监听选中事件
        this.uiInfo.Info = null;
        GuildService.Instance.OnGuildListResult += UpdateGuildList;    //监听列表刷新
        GuildService.Instance.OnJoinGuild += OnJoinGuild;
        GuildService.Instance.SendGuildListRequest();                            //列表刷新请求
    }

    void OnDestroy()
    {
        GuildService.Instance.OnGuildListResult -= UpdateGuildList;
        GuildService.Instance.OnJoinGuild -= OnJoinGuild;
    }

    /// <summary>
    /// 更新公会列表
    /// </summary>
    /// <param name="guilds">NGuildInfo 公会列表</param>
    void UpdateGuildList(List<NGuildInfo> guilds)
    {
        ClearList();
        InitItems(guilds);
    }

    /// <summary>
    ///  公会选中
    /// </summary>
    /// <param name="item"></param>
    public void OnGuildSelected(ListView.ListViewItem item)
    {
        this.selectedItem = item as UIGuildItem;
        this.uiInfo.Info = this.selectedItem.Info;
    }

    /// <summary>
    /// 初始化公会列表
    /// </summary>
    /// <param name="guilds">NGuildInfo公会列表</param>
    void InitItems(List<NGuildInfo> guilds)
    {
        foreach (var item in guilds)
        {
            GameObject go = Instantiate(itemPrefab, this.listMain.transform);
            UIGuildItem ui = go.GetComponent<UIGuildItem>();
            ui.SetGuildInfo(item);
            this.listMain.AddItem(ui);
        }
    }

    /// <summary>
    /// 清空公会列表
    /// </summary>
    void ClearList()
    {
        this.listMain.RemoveAll();
    }

    /// <summary>
    /// 点击加入公会
    /// </summary>
    public void OnClickJoin()
    {
        if (selectedItem == null)
        {
            MessageBox.Show("请选择需要加入的公会");
            return;
        }
        MessageBox.Show(string.Format("确定需要加入[{0}]公会嘛？",selectedItem.Info.GuildName), "申请加入公会", MessageBoxType.Confirm,"确认","取消").OnYes 
            = () =>
            {
                GuildService.Instance.SendGuildJoinRequest(this.selectedItem.Info.Id);
            };
    }

    /// <summary>
    /// 加入公会回调
    /// </summary>
    void OnJoinGuild()
    {
        this.Close(WindowResult.Yes);
    }
}
