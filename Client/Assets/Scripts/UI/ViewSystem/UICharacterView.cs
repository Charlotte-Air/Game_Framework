using UnityEngine;

/// <summary>
/// 职业视图
/// </summary>
public class UICharacterView : MonoBehaviour 
{
    /// <summary>
    /// 职业对象
    /// </summary>
    public GameObject[] characters;
    private int currentCharacter = 0;
    /// <summary>
    /// 职业索引值
    /// </summary>
    public int CurrectCharacter
    {
        get { return currentCharacter; }
        set
        {
            currentCharacter = value;
            this.UpdateCharacter();
        }
    }

    /// <summary>
    /// 职业索引
    /// </summary>
    void UpdateCharacter()
    {
        for(int i=0;i<3;i++)
        {
            characters[i].SetActive(i == this.currentCharacter); //根据索引角色激活相应的角色显示
        }
    }
}
