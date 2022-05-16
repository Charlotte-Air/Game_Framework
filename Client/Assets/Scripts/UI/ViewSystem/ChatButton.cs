using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 聊天按钮
/// </summary>
public class ChatButton : MonoBehaviour
{
    /// <summary>
    /// 聊天视图
    /// </summary>
    public ChatView Chatview;
    public GameObject root;
    /// <summary>
    /// 按钮索引值
    /// </summary>
    public int tabIndex = 0;
    public bool selected = false;

    void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(OnClick); //给按钮添加事件
    }

    public void Select(bool select)
    {

    }

    /// <summary>
    /// 点击按钮
    /// </summary>
    void OnClick()
    {
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Button1);
        this.Chatview.SelectTab(this.tabIndex); //切换标签
    }
}

