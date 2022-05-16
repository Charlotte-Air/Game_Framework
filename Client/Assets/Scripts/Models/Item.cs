using Common.Data;
using Charlotte.Proto;

namespace Models
{
    public class Item
    {
        /// <summary>
        /// 道具ID
        /// </summary>
        public int ID;

        /// <summary>
        /// 道具数量
        /// </summary>
        public int Count;

        /// <summary>
        /// 道具表缓存
        /// </summary>
        public ItemDefine Define;

        /// <summary>
        /// 装备表缓存
        /// </summary>
        public EquipDefine EquipInfo;

        public Item(NItemInfo item) : this (item.Id, item.Count) { } //重载

        public Item(int id ,int count)
        {
            this.ID = id;
            this.Count = count;
            DataManager.Instance.Items.TryGetValue(this.ID, out this.Define); //加载道具信息
            DataManager.Instance.Equips.TryGetValue(this.ID, out this.EquipInfo); //加载装备信息
        }

        public override string ToString()
        {
            return string.Format("-> ID: {0},Count: {1}", this.ID, this.Count);
        }
    }
}
