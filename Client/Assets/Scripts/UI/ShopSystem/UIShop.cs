using Models;
using Managers;
using UnityEngine;
using Common.Data;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// 商店系统
/// </summary>
public class UIShop : UIWindow
{
    /// <summary>
    /// 标题
    /// </summary>
    public Text title;

    /// <summary>
    /// 金币
    /// </summary>
    public Text money;

    /// <summary>
    /// 道具对象
    /// </summary>
    public GameObject shopItem;

    /// <summary>
    /// 商店表缓存
    /// </summary>
    ShopDefine shop;

    /// <summary>
    /// 列表节点
    /// </summary>
    public Transform[] itemRoot;

	void Start ()
    {
        StartCoroutine((InitItems()));
    }

    void Update()
    {
        if (this.money.text != User.Instance.CurrentCharacter.Gold.ToString()) this.money.text = User.Instance.CurrentCharacter.Gold.ToString();
    }

    /// <summary>
    /// 初始化商店
    /// </summary>
    IEnumerator InitItems()
    {
        int count = 0;
        int page = 0;
        foreach (var kv in DataManager.Instance.ShopItems[shop.ID])
        {
            if (kv.Value.Status > 0)  //判断道具状态是否销售
            {
                GameObject go = Instantiate(shopItem, itemRoot[page]);
                UIShopItem ui = go.GetComponent<UIShopItem>();
                ui.SetShopItem(kv.Key, kv.Value, this);
                count++;
                if (count >= 20) //每个道具超过20放入下一页
                {
                    count = 0;
                    page++;
                    itemRoot[page].gameObject.SetActive(true);
                }
            }
        }
        yield return null;
    }

    /// <summary>
    /// 设置商店
    /// </summary>
    /// <param name="shop">商店表</param>
    public void SetShop(ShopDefine shop)
    {
        this.shop = shop;
        this.title.text = shop.Name;
        this.money.text = User.Instance.CurrentCharacter.Gold.ToString(); 
    }

    private UIShopItem selectedItem;
    /// <summary>
    /// 选中道具
    /// </summary>
    /// <param name="item"></param>
    public void SelectShopItem(UIShopItem item)
    {
        if (selectedItem != null)
        {
            selectedItem.Selected = false; //选中状态清除
        }
        selectedItem = item; //当前选中
    }

    /// <summary>
    /// 购买提示框
    /// </summary>
    public void OnClickBuy()
    {
        if (this.selectedItem == null)
        {
            MessageBox.Show("请选择要购买的道具", "购买提示");
            return;
        }
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Button2);
        ShopManager.Instance.BuyItem(this.shop.ID, this.selectedItem.ShopItemID);
    }

}
