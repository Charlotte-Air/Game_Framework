using UnityEngine;
using Common.Data;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIPopItemInfo : UIWindow,IDeselectHandler
{
    public int itemid;
    public Text itemName;
    public Text itemInfo;
    public Text itemSellPrice;
    public Text itemStackLimit;
    public Image itemlcon;
    private ItemDefine item;
    
    public void OnDeselect(BaseEventData eventData)
    {
        var ed = eventData as PointerEventData;
        if (ed.hovered.Contains(this.gameObject))//判断节点里面是否包含当前界面
            return;
        else
            this.Close(WindowResult.None);
    }

    public void OnEnable()
    {
        this.GetComponent<Selectable>().Select();
        this.Root.transform.position = Input.mousePosition + new Vector3(-80, 0, 0);
    }

    public void SetItemInfo(ItemDefine item,string itemCount)
    {
        this.item = item;
        this.itemid = this.item.ID;
        this.itemName.text = this.item.Name.ToString();
        this.itemInfo.text = this.item.Description.ToString();
        this.itemlcon.sprite = Resloader.Load<Sprite>(this.item.Icon);
        this.itemSellPrice.text = string.Format("出售价格:{0}", this.item.SellPrice.ToString());
        this.itemStackLimit.text = string.Format("堆叠数量({0}/{1})", this.item.StackLimit.ToString(), itemCount);
    }

}
