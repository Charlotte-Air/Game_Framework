using Entities;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 世界UI玩家条
/// </summary>
public class UINameBar : MonoBehaviour
{
    /// <summary>
    /// 玩家名称
    /// </summary>
    public Text avaverName;

    /// <summary>
    /// 玩家对象
    /// </summary>
    public Character character;

    /// <summary>
    /// 职业图标
    /// </summary>
    public Image icon;

	void Update () 
    {
        this.UpdateInfo();
    }
    void UpdateInfo()
    {
        if (this.character != null)
        {
            string name = this.character.Name + " Lv." + this.character.Info.Level;
            if(name != this.avaverName.text)
            {
                this.avaverName.text = name;
            }
        }
    }
}
