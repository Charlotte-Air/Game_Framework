using UnityEngine;
using UnityEngine.UI;
using Charlotte.Proto;

/// <summary>
/// 角色信息
/// </summary>
public class UICharInfo : MonoBehaviour 
{
    /// <summary>
    /// 角色信息缓存
    /// </summary>
    public NCharacterInfo info;
    /// <summary>
    /// 角色职业
    /// </summary>
    public Text charClass;
    /// <summary>
    /// 角色名称
    /// </summary>
    public Text charName;
    /// <summary>
    /// 选中状态
    /// </summary>
    public Image highlight;
    public bool Selected
    {
        get { return highlight.IsActive(); }
        set { highlight.gameObject.SetActive(value); }
    }

    void Start () 
    {
		if(info!=null)
        {
            this.charClass.text = this.info.Class.ToString();
            this.charName.text = this.info.Name;
        }
	}
}
