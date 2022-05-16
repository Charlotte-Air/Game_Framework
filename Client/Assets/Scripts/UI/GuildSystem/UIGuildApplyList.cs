using Services;
using Managers;
using UnityEngine;

/// <summary>
/// 公会申请列表系统
/// </summary>
public class UIGuildApplyList : UIWindow
{
    /// <summary>
    /// 公会申请系统预设体
    /// </summary>
    public GameObject itemPrefab;

    /// <summary>
    /// 视图对象
    /// </summary>
    public ListView listMain;

    /// <summary>
    /// 视图子节点
    /// </summary>
    public Transform itemRoot;

	void Start ()
    {
        GuildService.Instance.OnGuildUpdate += UpdateUI;
		GuildService.Instance.SendGuildListRequest();
        this.UpdateUI();
    }

    void OnDestroy()
    {
        GuildService.Instance.OnGuildUpdate -= UpdateUI;
    }

    /// <summary>
    /// 更新UI
    /// </summary>
    void UpdateUI ()
    {
        ClearList();
        InitItems();
    }

    /// <summary>
    /// 初始化申请列表
    /// </summary>
    public void InitItems()
    {
        foreach (var item in GuildManager.Instance.guildInfo.Applies)
        {
            GameObject go = Instantiate(itemPrefab, this.listMain.transform);
            UIGuildApplyItem ui = go.GetComponent<UIGuildApplyItem>();
            ui.SetGuildInfo(item);
            this.listMain.AddItem(ui);
        }
    }

    /// <summary>
    /// 清空申请列表
    /// </summary>
    public void ClearList()
    {
        this.listMain.RemoveAll();
    }
}
