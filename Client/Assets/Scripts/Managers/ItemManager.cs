using Models;
using Services;
using UnityEngine;
using Common.Data;
using Charlotte.Proto;
using System.Collections.Generic;

namespace Managers
{   /// <summary>
    /// 道具管理器
    /// </summary>
    public class ItemManager : Singleton<ItemManager>
    { 
        /// <summary>
        /// 道具集合
        /// </summary>
        public Dictionary<int, Item> Items = new Dictionary<int, Item>();

        internal void Init(List<NItemInfo> items) //来源于网络数据
        {
            this.Items.Clear();
            foreach (var info in items)
            {
                Item item = new Item(info);
                this.Items.Add(item.ID,item);
                Debug.LogFormat("ItemMagager-> Init[{0}]", item);
            }
            StateService.Instance.RegisterStatusNofity(StatusType.Item,OnItemNoTify); //注册状态通知事件
        }

        /// <summary>
        /// 获取道具
        /// </summary>
        /// <param name="itemID">道具ID</param>
        /// <returns></returns>
        public ItemDefine GetItem(int itemID)
        {
            return null;
        }

        /// <summary>
        /// 状态通知回调事件
        /// </summary>
        /// <param name="status">NStatus</param>
        /// <returns></returns>
        private bool OnItemNoTify(NStatus status)
        {
            if (status.Action == StatusAction.Add)
                this.AddItem(status.Id, status.Value);
            if (status.Action == StatusAction.Delete)
                this.RemoveItem(status.Id, status.Value);
            return true;
        }

        /// <summary>
        /// 添加道具
        /// </summary>
        /// <param name="itemId">道具ID</param>
        /// <param name="count">数量</param>
        private void AddItem(int itemId, int count)
        {
            Item item = null;
            if (this.Items.TryGetValue(itemId, out item)) //判断道具是否存在
            {
                item.Count += count; //堆加
            }
            else
            {
                item = new Item(itemId,count);
                this.Items.Add(itemId,item); //添加道具列表
            }
            BagManager.Instance.AddItem(itemId,count); //背包更新
        }

        /// <summary>
        /// 删除道具
        /// </summary>
        /// <param name="itemId">道具ID</param>
        /// <param name="count">数量</param>
        private void RemoveItem(int itemId, int count)
        {
            if (!this.Items.ContainsKey(itemId)) //判断道具是否存在
                return;
            Item item = this.Items[itemId];
            if (item.Count < count)
                return;
            item.Count -= count;
            BagManager.Instance.RemoveItem(itemId, count);
        }

        /// <summary>
        /// 使用道具
        /// </summary>
        /// <param name="itemID">道具ID</param>
        public bool UseItem(int itemID)
        {
            return false;
        }

        /// <summary>
        /// 使用道具
        /// </summary>
        /// <param name="itemID">道具ID</param>
        public bool UseItem(ItemDefine item)
        {
            return false;
        }
    }
}
