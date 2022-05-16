using UnityEngine;
using Common.Data;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 商店道具
/// </summary>
public class UIShopItem : MonoBehaviour ,ISelectHandler,IPointerClickHandler
{
    /// <summary>
    /// 道具图标
    /// </summary>
    public Image icon;

    /// <summary>
    /// 道具名称
    /// </summary>
    public Text title;

    /// <summary>
    /// 道具价格
    /// </summary>
    public Text price;

    /// <summary>
    /// 道具数量
    /// </summary>
    public Text count;

    /// <summary>
    /// 道具职业
    /// </summary>
    public Text IimitClass;

    /// <summary>
    /// 道具选中状态
    /// </summary>
    public Image background;

    /// <summary>
    /// 正常状态
    /// </summary>
    public Sprite normalBg;

    /// <summary>
    /// 选中状态
    /// </summary>
    public GameObject selectedBg;

    private bool selected;
    /// <summary>
    /// 道具选中
    /// </summary>
    public bool Selected
    {
        get { return selected; }
        set
        {
            selected = value;
            if(selected)
                selectedBg.SetActive(true);
            else
                selectedBg.SetActive(false);
        }
    }


    /// <summary>
    /// 商店道具ID
    /// </summary>
    public int ShopItemID { get; set; }
    private UIShop shop;
    private ItemDefine item;
    private ShopItemDefine ShopItem;
    private bool ClickCount;
    private float ClickTime;

    /// <summary>
    /// 设置商店道具
    /// </summary>
    /// <param name="id">道具ID</param>
    /// <param name="shopItem">商店道具表</param>
    /// <param name="owner">商店视图</param>
    public void SetShopItem(int id, ShopItemDefine shopItem, UIShop owner)
    {
        this.shop = owner;
        this.ShopItemID = id;
        this.ShopItem = shopItem;
        this.item = DataManager.Instance.Items[this.ShopItem.ItemID];
        this.title.text = this.item.Name;
        this.count.text = "X" + ShopItem.Count.ToString();
        this.price.text = ShopItem.Price.ToString();
        if (this.item.LimitClass == Charlotte.Proto.CharacterClass.None)
            this.IimitClass.gameObject.SetActive(false);
        else
            this.IimitClass.text = this.item.LimitClass.ToString();
        this.icon.overrideSprite = Resloader.Load<Sprite>(item.Icon);
    }

    /// <summary>
    /// 点击道具事件
    /// </summary>
    /// <param name="eventData"></param>
    public void OnSelect(BaseEventData eventData)
    {
        this.Selected = true;                           //被选择时
        this.shop.SelectShopItem(this); //告诉商店选择了自己
    }

    /// <summary>
    /// 多次点击事件
    /// </summary>
    /// <param name="pointerEventData"></param>
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Button2);
        if (this.ClickCount)
        {
            this.ClickCount = true;
            this.ClickTime = Time.time;
            if (this.ClickTime < 0.5 ||this.ClickCount ==true)
            {
                this.shop.SelectShopItem(this); //告诉商店选择了自己
            }
        }
        else
        {
            this.ClickCount = false;
            this.ClickTime = 0;
        }
    }
}
