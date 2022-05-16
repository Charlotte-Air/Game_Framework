using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 切换按钮
/// </summary>
public class TabButton : MonoBehaviour
{
    public Sprite activeImage;
    private Sprite normalImage;
    /// <summary>
    /// 切换视图
    /// </summary>
    public TabView tabView;
    /// <summary>
    /// 切换页数
    /// </summary>
    public int tabIndex = 0;
    /// <summary>
    /// 选中
    /// </summary>
    public bool selected = false;
    /// <summary>
    /// 选中图标
    /// </summary>
    private Image tabImage;

    void Start ()
    {
        tabImage = this.GetComponent<Image>();
        normalImage = tabImage.sprite;
        this.GetComponent<Button>().onClick.AddListener(OnClick); //给按钮添加事件
    }

    public void Select(bool select)
    {
        tabImage.overrideSprite = select ? activeImage : normalImage;
    }

    /// <summary>
    /// 点击切换
    /// </summary>
    void OnClick()
    {
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Button2);
        this.tabView.SelectTab(this.tabIndex); //切换标签
    }
}
