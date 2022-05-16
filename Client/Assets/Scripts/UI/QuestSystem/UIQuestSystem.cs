using Managers;
using UnityEngine;
using Common.Data;

/// <summary>
/// 任务系统
/// </summary>
public class UIQuestSystem : UIWindow
{
    /// <summary>
    /// 列表预设体
    /// </summary>
    public GameObject itemPrefab;

    /// <summary>
    /// 任务视图
    /// </summary>
    public TabView Tabs;

    /// <summary>
    /// 主线列表
    /// </summary>
    public ListView listMain;

    /// <summary>
    /// 支线列表
    /// </summary>
    public ListView listBranch;

    /// <summary>
    /// 任务信息
    /// </summary>
    public UIQuestInfo questInfo;

    /// <summary>
    /// 是否显示可接任务
    /// </summary>
    private bool showAvailableList = false;

	void Start ()
    {
        this.listMain.onItemSelected += this.OnQuestSelected;
        this.listBranch.onItemSelected += this.OnQuestSelected;
        this.Tabs.OnTabSelect += OnSelectTab;
        RefreshUI();
        //QuestManager.Instance.OnQuestChanged += RefreshUI;
    }

    void OnDestroy()
    {
        //QuestManager.Instance.OnQuestChanged -= RefreshUI;
    }

    /// <summary>
    /// 切换任务类型视图
    /// </summary>
    /// <param name="idx">索引值</param>
    void OnSelectTab(int idx)
    {
        showAvailableList = idx == 1;
        RefreshUI();
    }

    /// <summary>
    /// 刷新
    /// </summary>
    void RefreshUI()
    {
        ClearAllQuestList();
        InitAllQuestItems();
    }

    /// <summary>
    /// 初始化任务列表
    /// </summary>
    void InitAllQuestItems()
    {
        foreach (var kv in QuestManager.Instance.allQuests) //拉取全部任务
        {
            if (showAvailableList) //判断是否是可接任务
            {
                if (kv.Value.Info != null)  //已经接受了跳过
                    continue;
            }
            else
            {
                if (kv.Value.Info == null)
                    continue;
            }
            GameObject go = Instantiate(itemPrefab, kv.Value.Define.Type == QuestType.Main ? this.listMain.transform : this.listBranch.transform);
            UIQuestItem ui = go.GetComponent<UIQuestItem>();
            ui.SetQuestItemInfo(kv.Value);
            if (kv.Value.Define.Type == QuestType.Main)
                this.listMain.AddItem(ui);
            else
                this.listBranch.AddItem(ui);
        }
    }

    /// <summary>
    /// 清空任务列表
    /// </summary>
    void ClearAllQuestList()
    {
        this.listMain.RemoveAll();
        this.listBranch.RemoveAll();
    }

    /// <summary>
    /// 选中任务列表
    /// </summary>
    public void OnQuestSelected(ListView.ListViewItem item)
    {
        if (listMain.SelectedItem != item.owner.SelectedItem)
        {
            if(listMain.SelectedItem!=null)
                listMain.SelectedItem.Selected = false;
        }
        else if (listBranch.SelectedItem != item.owner.SelectedItem)
        {
            if(listBranch.SelectedItem !=null)
                listBranch.SelectedItem.Selected = false;
        }
        UIQuestItem questItem =item as UIQuestItem;
        this.questInfo.SetQuestInfo(questItem.quest);
    }

    private UIQuestItem selectedItem;
    public void SelectQuestItem(UIQuestItem item)
    {
        if (selectedItem != null)
        {
            selectedItem.Selected = false; //选中状态清除
        }
        selectedItem = item; //当前选中
    }
}
