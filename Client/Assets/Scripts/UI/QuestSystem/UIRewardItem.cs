using UnityEngine;
using UnityEngine.UI;
public class UIRewardItem : MonoBehaviour
{
    /// <summary>
    /// 道具图标
    /// </summary>
    public Image mainImage;

    /// <summary>
    /// 道具内容
    /// </summary>
    public Text mainText;

    /// <summary>
    /// 道具ID
    /// </summary>
    public int ItemID;

    /// <summary>
    /// 设置内容
    /// </summary>
    /// <param name="iconName">图标名称</param>
    /// <param name="text">内容</param>
    public void SetRewardItem(string iconName, string text,int itemID)
    {
        this.mainImage.overrideSprite = Resloader.Load<Sprite>(iconName); //加载图标
        this.mainText.text = text; //加载文字
        this.ItemID = itemID;
    }

}
