using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine.EventSystems;


[System.Serializable]
public class ItemSelectEvent : UnityEvent<ListView.ListViewItem>
{
}

/// <summary>
/// 列表视图
/// </summary>
public class ListView : MonoBehaviour
{ 
    public UnityAction<ListViewItem> onItemSelected;
    /// <summary>
    /// 列表视图道具
    /// </summary>
    public class ListViewItem : MonoBehaviour, IPointerClickHandler
    {
        /// <summary>
        /// 视图对象
        /// </summary>
        public ListView owner;
        private bool selected;
        /// <summary>
        /// 选中属性
        /// </summary>
        public bool Selected
        {
            get { return selected; }
            set
            {
                selected = value;
                onSelected(selected);
            }
        }

        public virtual  void onSelected(bool selected)
        {
        }

        private bool isSelected = true;
        /// <summary>
        /// 点击事件
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Button2);
            if (!this.selected)
                this.Selected = true;
            if (owner != null)
                owner.SelectedItem = this;
        }
    }

    /// <summary>
    /// 道具集合
    /// </summary>
    List<ListViewItem> items = new List<ListViewItem>();
    private ListViewItem selectedItem = null;
    /// <summary>
    /// 选中道具属性
    /// </summary>
    public ListViewItem SelectedItem
    {
        get { return selectedItem; }
        set
        {
            if (selectedItem != null && selectedItem != value)
                selectedItem.Selected = false;
            selectedItem = value;
            if (onItemSelected != null)
                onItemSelected.Invoke((ListViewItem) value);
        }
    }

    /// <summary>
    /// 添加道具
    /// </summary>
    public void AddItem(ListViewItem item)
    {
        item.owner = this;
        this.items.Add(item);
    }

    /// <summary>
    /// 清除全部道具
    /// </summary>
    public void RemoveAll()
    {
        if (items != null)
        {
            foreach (var it in items)
            {
                if (it != null)
                    Destroy(it.gameObject);
            }
            items.Clear();
        }
    }

}
