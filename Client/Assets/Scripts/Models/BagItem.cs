namespace Models
{
    [System.Runtime.InteropServices.StructLayout (System.Runtime.InteropServices.LayoutKind.Sequential,Pack =1) ]
    struct BagItem
    {
        /// <summary>
        /// 道具ID
        /// </summary>
        public ushort ItemId;
        /// <summary>
        /// 道具数量
        /// </summary>
        public ushort Count;
        /// <summary>
        /// 默认值
        /// </summary>
        public static BagItem zero = new BagItem {ItemId = 0, Count = 0};
        public BagItem(int itemid,int count)
        {
            this.ItemId = (ushort)itemid;
            this.Count = (ushort)count;
        }

        //重载
        public static bool operator==(BagItem lhs, BagItem rhs)
        {
            return lhs.ItemId == rhs.ItemId && lhs.Count == rhs.Count;
        }

        //重载
        public static bool operator !=(BagItem lhs, BagItem rhs)
        {
            return !(lhs == rhs);
        }

        public override bool Equals(object other)
        {
            if (other is BagItem)
            {
                return Equals((BagItem) other);
            }
            return false;
        }

        public bool Equals(BagItem other)
        {
            return this == other;
        }

        /// <summary>
        /// 获取哈希码
        /// </summary>
        /// <returns>返回道具哈希码</returns>
        public override int GetHashCode()
        {
            return ItemId.GetHashCode() ^ (Count.GetHashCode() << 2);
        }
    }
}
