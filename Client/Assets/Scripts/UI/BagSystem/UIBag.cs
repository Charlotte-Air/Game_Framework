using Models;
using Managers;
using UnityEngine;
using Common.Data;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 背包系统
/// </summary>
public class UIBag : UIWindow
{
    /// <summary>
    /// 背包金钱
    /// </summary>
    public Text money;

    /// <summary>
    /// 背包页数
    /// </summary>
    public Transform[] pages;

    /// <summary>
    /// 背包道具
    /// </summary>
    public GameObject bagItem;

    /// <summary>
    /// 槽
    /// </summary>
    List<Image> slots;

    /// <summary>
    /// 锁槽图标
    /// </summary>
    public Sprite SlotsSprite;

    void Start () 
    {
        if (slots == null)
        {
            slots = new List<Image>();
            for (int page = 0; page < this.pages.Length; page++)
            {
                slots.AddRange(this.pages[page].GetComponentsInChildren<Image>(true));
            }
            StartCoroutine(InitBags());
            this.money.text = User.Instance.CurrentCharacter.Gold.ToString();
        }
        BagManager.Instance.ItemBuyUpdate += this.UpdateItem;
    }

    /// <summary>
    /// 初始化背包
    /// </summary>
    IEnumerator InitBags()
    {
        for (int i = 0; i < BagManager.Instance.Items.Length; i++)
        {
            var item = BagManager.Instance.Items[i];
            if (item.ItemId > 0)
            {
                GameObject go = Instantiate(bagItem, slots[i].transform);   //实例化，副节点设置为槽
                var ui = go.GetComponent<UIBagItem>();
                ItemDefine def = ItemManager.Instance.Items[item.ItemId].Define; //从道具管理器拿取配置表数据
                ui.SetMainIcon(def.Icon, item.Count.ToString(), i ,def); //设置图标和数量
            }
        }
        for (int i = BagManager.Instance.Items.Length; i < slots.Count; i++) //判断
        {
            slots[i].gameObject.GetComponent<Image>().sprite = SlotsSprite;
        }
        yield return null;
    }

    void OnDestroy()
    {
        BagManager.Instance.ItemBuyUpdate -= this.UpdateItem;
    }

    /// <summary>
    /// 清空
    /// </summary>
    void Clear()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].transform.childCount > 0)
            {
                Destroy(slots[i].transform.GetChild(0).gameObject);
            }
        }
    }

    /// <summary>
    /// 点击整理背包
    /// </summary>
    public void OnReset()
    {
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_ChatSend);
        BagManager.Instance.Reset();
        this.Close();
        StartCoroutine(InitBags());
        UIMain.Instance.OnClickBag();
    }

    public void UpdateItem()
    {
        BagManager.Instance.Reset();
        this.Close();
        StartCoroutine(InitBags());
        UIMain.Instance.OnClickBag();
    }
}
