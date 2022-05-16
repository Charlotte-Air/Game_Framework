using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

/// <summary>
/// 聊天频道视图
/// </summary>
public class ChatView : MonoBehaviour
{
    /// <summary>
    /// 频道按钮
    /// </summary>
    public ChatButton[] ChatButtons;
    /// <summary>
    /// 频道视图
    /// </summary>
    public GameObject[] tabPages;
    /// <summary>
    /// 频道内容
    /// </summary>
    public Text[] text;
    public UnityAction<int> OnTabSelect;
    /// <summary>
    /// 频道索引值
    /// </summary>
    public int index = -1;

    IEnumerator Start()
    {
        for (int i = 0; i < ChatButtons.Length; i++)
        {
            ChatButtons[i].Chatview = this;
            ChatButtons[i].tabIndex = i;
        }
        yield return new WaitForEndOfFrame(); //等一帧
        SelectTab(0); //默认选择第一页
    }

    /// <summary>
    /// 切换频道
    /// </summary>
    /// <param name="index">频道索引值</param>
    public void SelectTab(int index)
    {
        if (this.index != index)
        {
            for (int i = 0; i < ChatButtons.Length; i++)
            {
                ChatButtons[i].Select(i == index);
                if (i < tabPages.Length)
                {
                    tabPages[i].SetActive(i == index);
                    if (i == index)
                    {
                        text[i].color = new Color(42 / 255f, 143 / 255f, 255f / 255f, 255f / 255f);
                    }
                    else
                    {
                        text[i].color = Color.gray;
                    }
                }
            }
            if (OnTabSelect != null)
                OnTabSelect(index);
        }
    }
}

