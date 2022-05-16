using Models;
using Managers;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 装备道具
/// </summary>
public class UIEquipItem : MonoBehaviour ,IPointerClickHandler
{
    /// <summary>
    /// 装备图标
    /// </summary>
    public Image icon;

    /// <summary>
    /// 装备名称
    /// </summary>
    public Text title;

    /// <summary>
    /// 装备等级
    /// </summary>
    public Text level;

    /// <summary>
    /// 所属职业
    /// </summary>
    public Text IimitClass;

    /// <summary>
    /// 所属种类
    /// </summary>
    public Text IimitCategory;

    /// <summary>
    /// 正常状态背景
    /// </summary>
    public Image background;

    /// <summary>
    /// 未选中精灵
    /// </summary>
    public Sprite normalBg;

    /// <summary>
    /// 选中精灵
    /// </summary>
    public Sprite selectedBg;

    private bool selected;
    public bool Selected
    {
        get { return selected; }
        set
        {
            selected = value;
            this.background.overrideSprite = selected ? selectedBg : normalBg;
        }
    }

    public int index { get; set; }
    private UICharEquip owmer;
    private Item item;
    private bool isEquiped = false;

    /// <summary>
    /// 装备详情设置
    /// </summary>
    /// <param name="idx">索引值</param>
    /// <param name="item">装备</param>
    /// <param name="owmer">实图对象</param>
    /// <param name="equiped">是否装备</param>
    public void SetEquipItem(int idx, Item item, UICharEquip owmer,bool equiped)
    {
        this.owmer = owmer;
        this.index = idx;
        this.item = item;
        this.isEquiped = equiped;
        if (this.title != null) this.title.text = this.item.Define.Name;
        if (this.level != null) this.level.text = this.item.Define.Level.ToString();
        if (this.IimitClass != null) this.IimitClass.text = this.item.Define.LimitClass.ToString();
        if (this.IimitCategory != null) this.IimitCategory.text = this.item.Define.Category;
        if (this.icon != null) this.icon.overrideSprite = Resloader.Load<Sprite>(this.item.Define.Icon);
    }

    /// <summary>
    /// 点击事件
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Button1);
        if (this.isEquiped)
        {
            UnEquip();
        }
        else
        {
            if (this.selected)
            {
                DoEquip();
                this.Selected = false;
            }
            else
                this.selected = true;
        }
    }

    /// <summary>
    /// 点击装备提示
    /// </summary>
    void DoEquip()
    {
        var msg = MessageBox.Show(string.Format("需要装备[{0}]嘛?", this.item.Define.Name), "确认", MessageBoxType.Confirm);
        msg.OnYes = () =>
         {
            var oldEquip = EquipManager.Instance.GetEquip(item.EquipInfo.Slot);
            if (oldEquip != null)
            {
                var newmsg = MessageBox.Show(string.Format("要替换装备中的[{0}]嘛?", oldEquip.Define.Name), "确认", MessageBoxType.Confirm);
                newmsg.OnYes = () =>
                {
                    this.owmer.DoEquip(this.item);
                };
            }
            else
                this.owmer.DoEquip(this.item);
         };
    }

    /// <summary>
    /// 点击取下提示
    /// </summary>
    void UnEquip()
    {
        var msg = MessageBox.Show(string.Format("要取下装备[{0}]嘛?", this.item.Define.Name), "确认", MessageBoxType.Confirm);
        msg.OnYes = () =>
         {
            this.owmer.UnEquip(this.item);
         };
    }
}
