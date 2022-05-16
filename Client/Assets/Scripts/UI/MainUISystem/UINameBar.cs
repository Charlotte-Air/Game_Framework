using Entities;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ����UI�����
/// </summary>
public class UINameBar : MonoBehaviour
{
    /// <summary>
    /// �������
    /// </summary>
    public Text avaverName;

    /// <summary>
    /// ��Ҷ���
    /// </summary>
    public Character character;

    /// <summary>
    /// ְҵͼ��
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
