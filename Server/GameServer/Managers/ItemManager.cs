using Common;
using Charlotte.Proto;
using GameServer.Entities;
using GameServer.Models;
using GameServer.Services;
using System.Collections.Generic;

namespace GameServer.Managers
{
    class ItemManager
    {
        //角色缓存
        Character Owner;
        //本地item缓存
        public Dictionary<int, Item> characterItems = new Dictionary<int, Item>();

        public ItemManager(Character owner)
        {
            this.Owner = owner;
            foreach (var item in owner.Data.Items) //DB
            {
                this.characterItems.Add(item.ItemID, new Item(item));
            }
        }

        /// <summary>
        /// 使用道具
        /// </summary>
        /// <param name="itemID">道具ID</param>
        /// <param name="count">数量</param>
        /// <returns></returns>
        public bool UseItem(int itemID, int count = 1)
        {
            Log.InfoFormat("[{0}] UseItem [{1} Count:{2}]", this.Owner.Data.ID, itemID, count);
            Item item = null;
            if (this.characterItems.TryGetValue(itemID, out item)) //判断道具是否存在
            {
                if (item.Count < count) //判断当前角色使用道具数量是否够不够
                {
                    return false;
                }
                item.Remove(count);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 查询道具是否存在
        /// </summary>
        /// <param name="itemID">道具ID</param>
        /// <returns></returns>
        public bool HasItem(int itemID)
        {
            if(characterItems.ContainsKey(itemID))
            {
                return characterItems[itemID].Count > 0;
            }
            else
                return false;
        }

        /// <summary>
        ///  获取道具
        /// </summary>
        /// <param name="itemID">道具ID</param>
        /// <returns></returns>
        public Item GetItem(int itemID)
        {
            if (characterItems.ContainsKey(itemID))
            {
                Log.InfoFormat("[{0}] GetItem [{1} Count:{2}]", this.Owner.Data.ID, characterItems[itemID].Count);
                return characterItems[itemID];
            }
            else
                return null;
        }

        /// <summary>
        /// 添加道具
        /// </summary>
        /// <param name="itemID">道具ID</param>
        /// <param name="count">数量</param>
        /// <returns></returns>
        public bool AddItem(int itemID, int count)
        {
            if (this.characterItems.ContainsKey(itemID))
            {
                this.characterItems[itemID].Add(count);
            }
            else
            {
                TCharacterItem newItem = new TCharacterItem()
                {
                    Owner = Owner.Data,
                    TCharacterID = Owner.Data.ID,
                    ItemID = itemID,
                    ItemCount = count,
                };
                Owner.Data.Items.Add(newItem);
                this.characterItems.Add(itemID, new Item(newItem));
            }
            this.Owner.StateManager.StateNotice(StatusType.Item, itemID, count, StatusAction.Add);
            Log.InfoFormat("[{0}] AddItemID [{1}] Count:[{2}]", this.Owner.Data.ID, itemID, count);
            DBService.Instance.Save();
            return true;
        }

        /// <summary>
        /// 删除道具
        /// </summary>
        /// <param name="itemID">道具ID</param>
        /// <param name="count">数量</param>
        /// <returns></returns>
        public bool RemoveItem(int itemID, int count)
        {
            if (this.characterItems.ContainsKey(itemID) && this.characterItems[itemID].Count >= count)
            {
                this.characterItems[itemID].Remove(count);
                this.Owner.StateManager.StateNotice(StatusType.Item, itemID, count, StatusAction.Delete);
                Log.InfoFormat("[{0}] RemoveItem [{1} Count:{2}]", this.Owner.Data.ID, itemID, count);
                DBService.Instance.Save();  //DB保存
                return true;
            }
            return false;
        }

        public TCharacterItem AddItem(TCharacter character , int itemID ,int itemCount)
        {
            TCharacterItem item = new TCharacterItem
            {
                Owner = character,
                ItemID = itemID,
                ItemCount = itemCount,
            };
            return item;
        }

        /// <summary>
        /// 获取道具信息
        /// </summary>
        /// <param name="list">NItemInfo</param>
        public void GetItemInfos(List<NItemInfo> list)
        {
            foreach (var item in this.characterItems) //内存数据转网络数据
            {
                list.Add(new NItemInfo()
                {
                    Id = item.Value.ItemID,
                    Count = item.Value.Count,
                });
            }
        }

    }
}
