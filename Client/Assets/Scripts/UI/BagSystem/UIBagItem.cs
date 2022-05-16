using UnityEngine;
using Common.Data;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIBagItem : MonoBehaviour, IPointerClickHandler
{
    /// <summary>
    /// 道具图标
    /// </summary>
    public Image mainImage;

    /// <summary>
    /// 道具内容
    /// </summary>
    public Text mainText;

    private ItemDefine item;

    private int slotID;

    /// <summary>
    /// 设置内容
    /// </summary>
    /// <param name="iconName">图标名称</param>
    /// <param name="text">内容</param>
    public void SetMainIcon(string iconName, string text, int seloid, ItemDefine item)
    {
        this.mainImage.overrideSprite = Resloader.Load<Sprite>(iconName); //加载图标
        this.mainText.text = text; //加载文字
        this.slotID = seloid;
        this.item = item;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        UIPopItemInfo popinfo = UIManager.Instance.Show<UIPopItemInfo>();
        popinfo.SetItemInfo(this.item, this.mainText.text);
    }


}
