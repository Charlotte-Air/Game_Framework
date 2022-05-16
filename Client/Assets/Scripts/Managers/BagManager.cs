using Models;
using Charlotte.Proto;
using UnityEngine.Events;
using System.Collections.Generic;

namespace Managers
{   
    /// <summary>
    /// 背包管理器
    /// </summary>
    class BagManager : Singleton<BagManager>
    {
        /// <summary>
        /// 道具购买更新
        /// </summary>
        public UnityAction ItemBuyUpdate;
        /// <summary>
        /// 背包道具集合 Key(格子) Value (道具)
        /// </summary>
        public Dictionary<int, BagItem> BagItems = new Dictionary<int, BagItem>();
        /// <summary>
        /// 格子
        /// </summary>
        public int Unlocked;
        /// <summary>
        /// 道具
        /// </summary>
        public BagItem[] Items;
        /// <summary>
        /// 背包信息缓存
        /// </summary>
        NBagInfo Info;

        unsafe public void Init(NBagInfo info)
        {
            this.Info = info;
            this.Unlocked = info.Unlocked;
            Items = new BagItem[this.Unlocked];
            if (info.Items != null && info.Items.Length >= this.Unlocked)
            {
                Analyze(info.Items); //解析
            }
            else
            {
                Info.Items = new byte[sizeof(BagItem) * this.Unlocked];  //重新创建数组
                Reset();
            }
            for (int i = 0; i < this.Unlocked; i++)
            {
                BagItems[i] = Items[i];
            }
        }

        /// <summary>
        /// 整理背包
        /// </summary>
        public void Reset()
        {
            int i = 0;
            foreach (var kv in ItemManager.Instance.Items) //遍历道具管理器
            {
                if (kv.Value.Count <= kv.Value.Define.StackLimit) //判断用户背包里第一个道具数量 ＜ 堆叠数量直接添入
                {
                    this.Items[i].ItemId = (ushort) kv.Key;
                    this.Items[i].Count = (ushort) kv.Value.Count;
                }
                else  //用户背包里第一个道具数量 > 堆叠数量 (进行拆分)
                {
                    int count = kv.Value.Count;  //记录当前道具数量
                    while (count > kv.Value.Define.StackLimit)  //判断道具数量 > 道具堆叠Max值
                    {
                        this.Items[i].ItemId = (ushort) kv.Key;                                //放进新格子
                        this.Items[i].Count = (ushort) kv.Value.Define.StackLimit; //放入当前道具规定Max值
                        i++; //下一个格子
                        count -= kv.Value.Define.StackLimit;
                    }
                    this.Items[i].ItemId = (ushort) kv.Key;
                    this.Items[i].Count = (ushort) count;
                }
                i++; //继续下个道具
            }
        }

        /// <summary>
        /// 解析 从字节数组流 - 解析成 -结构体数组
        /// 从字节数组通过指针方式赋值给结构体数组
        /// </summary>
        /// <param name="data">数据流</param>
        unsafe void Analyze(byte[] data)
        {   //fixed执行过程中地址不会发生改变
            fixed (byte* pt = data) //指向data的指针
            {
                for (int i = 0; i < this.Unlocked; i++)
                {
                    BagItem* item = (BagItem*) (pt + i * sizeof(BagItem)); //  data最开始的指针  + i(第几个格子的大小) * 1个格子字节数（通过地址访问数据）
                    Items[i] = *item;  //赋值
                }
            }
        }

        /// <summary>
        /// 封装 从结构体数组 -封装成 - 字节数组流
        /// </summary>
        /// <returns></returns>
        unsafe public NBagInfo GetBagInfo()
        {
            fixed (byte* pt = Info.Items)
            {
                for (int i = 0; i < this.Unlocked; i++)
                {
                    BagItem* item = (BagItem*) (pt + i * sizeof(BagItem));
                    *item = Items[i];
                }
            }
            return this.Info; //发送网络消息
        }

        /// <summary>
        /// 添加道具
        /// </summary>
        /// <param name="itemId">道具ID</param>
        /// <param name="count">数量</param>
        public void AddItem(int itemId, int count)
        {
            ushort addCount = (ushort) count;
            for (int i = 0; i < Items.Length; i++)
            {
                if (this.Items[i].ItemId == itemId)
                {
                    ushort canAdd = (ushort) (DataManager.Instance.Items[itemId].StackLimit - this.Items[i].Count);
                    if (canAdd >= addCount)
                    {
                        this.Items[i].Count += addCount;
                        addCount = 0;
                        break;
                    }
                    else
                    {
                        this.Items[i].Count += canAdd;
                        addCount -= canAdd;
                    }
                }
            }
            if (addCount > 0)
            {
                for (int i = 0; i < Items.Length; i++) 
                {
                    if (this.Items[i].ItemId == 0) //判断格子是否空 ，则添加道具
                    {
                        this.Items[i].ItemId = (ushort) itemId;
                        this.Items[i].Count = addCount;
                        break;
                    }
                }
            }
            if (this.ItemBuyUpdate != null)
                this.ItemBuyUpdate();
        }

        /// <summary>
        /// 删除道具
        /// </summary>
        /// <param name="itemId">道具ID</param>
        /// <param name="count">数量</param>
        public void RemoveItem(int itemId, int count)
        {
            
        }

        public bool SwapItem(int itemid1, int itemid2)
        {
            var item1 = BagItems[itemid1];
            var item2 = BagItems[itemid2];
            if (item1 != null && item2 != null && item1.ItemId > 0 && item2.ItemId > 0)
            {
                BagItem bg = Items[itemid1];
                Items[itemid1] = Items[itemid2];
                Items[itemid2] = bg;
                this.BagItems[itemid1] = item1;
                this.BagItems[itemid2] = item2;
                return true;
            }
            return false;
        }

    }
}
