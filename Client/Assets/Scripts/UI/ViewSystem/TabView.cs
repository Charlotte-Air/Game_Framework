using UnityEngine;
using System.Collections;
using UnityEngine.Events;

/// <summary>
/// 切换视图
/// </summary>
public class TabView : MonoBehaviour 
{
    /// <summary>
    /// 切换按钮
    /// </summary>
    public TabButton[ ] tabButtons;
    /// <summary>
    /// 视图对象
    /// </summary>
    public GameObject[] tabPages;
    public UnityAction<int> OnTabSelect;
    /// <summary>
    /// 视图索引值
    /// </summary>
    public int index = -1;

    IEnumerator Start()
    {
        for (int i = 0; i < tabButtons.Length; i++)
        {
            tabButtons[i].tabView = this;
            tabButtons[i].tabIndex = i;
        }
        yield return new WaitForEndOfFrame(); //等一帧
        SelectTab(0); //默认选择第一页
    }

    /// <summary>
    ///  切换
    /// </summary>
    /// <param name="index">视图索引值</param>
    public void SelectTab(int index)
    {
        if (this.index != index) //判断当前背包页数
        {
            for (int i = 0; i < tabButtons.Length; i++)
            {
                tabButtons[i].Select(i == index);
                if (i < tabPages.Length)
                {
                    tabPages[i].SetActive(i == index);
                }
            }

            if (OnTabSelect != null)
                OnTabSelect(index);
        }
    }

}
