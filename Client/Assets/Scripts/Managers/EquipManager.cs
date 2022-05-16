using Models;
using Services;
using Charlotte.Proto;

namespace Managers
{  
    /// <summary>
    /// 装备管理器
    /// </summary>
    class EquipManager : Singleton<EquipManager>
    {
        public delegate void OnEquipChangeHandler();
        /// <summary>
        /// 装备更新事件
        /// </summary>
        public event OnEquipChangeHandler OnEquipChanged;
        /// <summary>
        /// 槽
        /// </summary>
        public Item[] Equips = new Item[(int) EquipSlot.SlotMax];
        /// <summary>
        /// 装备槽数据流
        /// </summary>
        byte[] Data; //int Liem

        unsafe public void Init(byte[] data)
        {
            this.Data = data;
            this.ParseEquipData(data);
        }

        /// <summary>
        /// 效验是否装备
        /// </summary>
        /// <param name="equipId">装备ID</param>
        /// <returns></returns>
        public bool Contains(int equipId)
        {
            for (int i = 0; i < this.Equips.Length; i++)
            {
                if (Equips[i] != null && Equips[i].ID == equipId)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 检查槽中装备
        /// </summary>
        /// <param name="slot">槽类型</param>
        /// <returns></returns>
        public Item GetEquip(EquipSlot slot)
        {
            return Equips[(int) slot];
        }

        /// <summary>
        /// 解析装备槽数据
        /// </summary>
        /// <param name="data">数据流</param>
        unsafe void ParseEquipData(byte[] data)
        {
            fixed (byte* pt = this.Data)
            {
                for (int i = 0; i < this.Equips.Length; i++)
                {
                    int itemId = *(int*) (pt + i * sizeof(int)); //解析ID
                    if (itemId > 0)
                        Equips[i] = ItemManager.Instance.Items[itemId];
                    else
                        Equips[i] = null;
                }
            }
        }

        /// <summary>
        /// 封装装备数据流
        /// </summary>
        unsafe public byte[] GetEquipData()
        {
            fixed (byte* pt = Data)
            {
                for (int i = 0; i < (int) EquipSlot.SlotMax; i++)
                {
                    int* itemId = (int*) (pt + i * sizeof(int));
                    if (Equips[i] == null)
                        *itemId = 0;
                    else
                        *itemId = Equips[i].ID;
                }
            }
            return this.Data;
        }

        /// <summary>
        /// 装备
        /// </summary>
        /// <param name="equip">装备</param>
        public void EquipItem(Item equip)
        {
            ItemService.Instance.SendEquipIte(equip, true);
        }

        /// <summary>
        /// 脱装备
        /// </summary>
        /// <param name="equip">装备</param>
        public void UnEquipItem(Item equip)
        {
            ItemService.Instance.SendEquipIte(equip, false);
        }

        /// <summary>
        /// 装备事件回调
        /// </summary>
        /// <param name="equip">装备</param>
        public void OnEquipItem(Item equip)
        {
            if (this.Equips[(int)equip.EquipInfo.Slot] != null && this.Equips[(int)equip.EquipInfo.Slot].ID == equip.ID)
                return;
            this.Equips[(int)equip.EquipInfo.Slot] = ItemManager.Instance.Items[equip.ID];
            if (OnEquipChanged != null)
                OnEquipChanged();
        }

        /// <summary>
        /// 脱装备事件回调 
        /// </summary>
        /// <param name="slot">槽类型</param>
        public void OnUnEquipItem(EquipSlot slot)
        {
            if (this.Equips[(int)slot] != null)
            {
                this.Equips[(int) slot] = null;
                if (OnEquipChanged != null)
                    OnEquipChanged();
            }
        }
    }
}
