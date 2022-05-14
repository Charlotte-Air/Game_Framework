namespace GameServer.Models
{
    class Item
    {
        /// <summary>
        /// 角色道具缓存
        /// </summary>
        TCharacterItem dbItem;

        /// <summary>
        /// 道具ID
        /// </summary>
        public int ItemID;

        /// <summary>
        /// 道具数量
        /// </summary>
        public int Count;

        public Item(TCharacterItem item) //来源于DB数据
        {
            this.dbItem = item;
            this.ItemID = (short)item.ItemID;
            this.Count = (short)item.ItemCount;
        }

        /// <summary>
        ///  添加
        /// </summary>
        /// <param name="count">数量</param>
        public void Add(int count)
        {
            this.Count += count;
            dbItem.ItemCount = this.Count;
        }

        /// <summary>
        /// 删除 
        /// </summary>
        /// <param name="count">数量</param>
        public void Remove(int count)
        {
            this.Count -= count;
            dbItem.ItemCount = this.Count;
        }

        /// <summary>
        /// 使用
        /// </summary>
        /// <param name="count">数量</param>
        /// <returns>是否成功</returns>
        public bool Use(int count = 1)
        {
            if (dbItem.ItemCount >= 1)
            {
                this.Count -= count;
                dbItem.ItemCount = this.Count;
                return true;
            }
            else
                return false;
        }

       
        public override string ToString()
        {
            return string.Format("ItemID:{0},Count:{1}", this.ItemID, this.Count);
        }
    }
}
