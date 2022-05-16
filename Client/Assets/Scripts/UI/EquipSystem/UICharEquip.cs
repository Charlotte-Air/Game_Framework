using Models;
using Managers;
using UnityEngine;
using UnityEngine.UI;
using Charlotte.Proto;
using System.Collections.Generic;

/// <summary>
/// 装备系统
/// </summary>
public class UICharEquip : UIWindow
{
    /// <summary>
    /// 标题
    /// </summary>
    public Text title;

    /// <summary>
    /// 金钱
    /// </summary>
    public Text money;

    /// <summary>
    /// 装备名称
    /// </summary>
    public Text Name;

    /// <summary>
    /// 装备等级
    /// </summary>
    public Text Level;

    /// <summary>
    /// 装备预制体
    /// </summary>
    public GameObject itemPrefab;

    /// <summary>
    /// 已装备中预制体
    /// </summary>
    public GameObject itemEquipendPrefab;

    /// <summary>
    /// 未装备列表节点
    /// </summary>
    public Transform itemListRoot;

    /// <summary>
    /// 装备槽
    /// </summary>
    public List<Transform> slots;

	void Start ()
    {
        RefreshUI();
        EquipManager.Instance.OnEquipChanged += RefreshUI; //注册装备变动事件
    }

    private void OnDestroy()
    {
        EquipManager.Instance.OnEquipChanged -= RefreshUI;
    }

    /// <summary>
    /// 更新UI
    /// </summary>
    void RefreshUI()
    {
        ClearAllEquipList();
        InitAllEquipItems();
        ClearEquipedList();
        InitEquipedItems();
        this.money.text = User.Instance.CurrentCharacter.Gold.ToString();
        this.Level.text = "Lv:" + User.Instance.CurrentCharacter.Level;
        this.Name.text = User.Instance.CurrentCharacter.Name;
    }

    /// <summary>
    /// 初始化装备列表
    /// </summary>
    void InitAllEquipItems()
    {
        foreach (var kv in ItemManager.Instance.Items) //遍历道具
        {
            if (kv.Value.Define.Type == ItemType.Equip)  //筛选为装备
            {
                if (EquipManager.Instance.Contains(kv.Key)) //效验是否已经装备
                    continue;
                GameObject go = Instantiate(itemPrefab, itemListRoot); //初始化
                UIEquipItem ui = go.GetComponent<UIEquipItem>();
                ui.SetEquipItem(kv.Key, kv.Value, this, false);
            }
        }
    }

    /// <summary>
    /// 清空装备列表
    /// </summary>
    void ClearAllEquipList()
    {
        foreach (var item in itemListRoot.GetComponentsInChildren<UIEquipItem>())
        {
            Destroy(item.gameObject);
        }
    }

    /// <summary>
    /// 清空装备槽
    /// </summary>
    void ClearEquipedList()
    {
        foreach (var item in slots)
        {
            if (item.childCount > 0)
                Destroy(item.GetChild(0).gameObject);
        }
    }

    /// <summary>
    /// 初始化已装备列表
    /// </summary>
    void InitEquipedItems()
    {
        for (int i = 0; i < (int) EquipSlot.SlotMax; i++)
        {
            var item = EquipManager.Instance.Equips[i]; //检查每个槽是否存在装备
            {
                if (item != null)
                {
                    GameObject go = Instantiate(itemEquipendPrefab, slots[i]);
                    UIEquipItem ui = go.GetComponent<UIEquipItem>();
                    ui.SetEquipItem(i, item, this, true);
                }
            }
        }
    }

    /// <summary>
    /// 装备
    /// </summary>
    /// <param name="item">装备</param>
    public void DoEquip(Item item)
    {
        EquipManager.Instance.EquipItem(item); 
    }

    /// <summary>
    /// 取下
    /// </summary>
    /// <param name="item">装备</param>
    public void UnEquip(Item item)
    {
        EquipManager.Instance.UnEquipItem(item);
    }
}
