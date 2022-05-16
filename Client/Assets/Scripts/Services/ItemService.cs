using System;
using Models;
using Network;
using Managers;
using UnityEngine;
using Charlotte.Proto;

namespace Services
{
    class ItemService : Singleton<ItemService>, IDisposable
    {
        public ItemService()
        {
            MessageDistributer.Instance.Subscribe<ItemBuyResponse>(this.OnItemBuy);         //角色购买道具响应
            MessageDistributer.Instance.Subscribe<ItemEquipResponse>(this.OnItemEquip);  //角色购买装备响应
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<ItemBuyResponse>(this.OnItemBuy);
            MessageDistributer.Instance.Unsubscribe<ItemEquipResponse>(this.OnItemEquip);
        }

        /// <summary>
        /// 发送购买道具请求
        /// </summary>
        public void SendBuyItem(int shopId, int shopItemId)
        {
            Debug.Log("->SendBuyItem");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.itemBuy = new ItemBuyRequest();
            message.Request.itemBuy.shopId = shopId;
            message.Request.itemBuy.shopItemId = shopItemId;
            NetClient.Instance.SendMessage(message);
        }

        /// <summary>
        /// 购买道具回调
        /// </summary>
        private void OnItemBuy(object sender, ItemBuyResponse message)
        {
            if (message.Result == Result.Success)
                MessageBox.Show(string.Format("购买成功"), "确认", MessageBoxType.Information);
            else
                MessageBox.Show(string.Format("购买失败"), "确认", MessageBoxType.Error);
        }

        /// <summary>
        /// 记录装备信息
        /// </summary>
        Item pendingEquip = null;
        /// <summary>
        /// 记录是穿或脱
        /// </summary>
        private bool isEquip;
        /// <summary>
        /// 发送装备请求
        /// </summary>
        /// <param name="equip">装备</param>
        /// <param name="isEquip">是否装备</param>
        /// <returns></returns>
        public bool SendEquipIte(Item equip, bool isEquip)
        {
            if (pendingEquip != null)
                return false;
            Debug.Log("->SendEquipItem");
            pendingEquip = equip;
            this.isEquip = isEquip;
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.itemEquip = new ItemEquipRequest();
            message.Request.itemEquip.Slot = (int) equip.EquipInfo.Slot;
            message.Request.itemEquip.itemId = equip.ID;
            message.Request.itemEquip.isEquip = isEquip;
            NetClient.Instance.SendMessage(message);
            return true;
        }

        /// <summary>
        /// 装备请求回调
        /// </summary>
        private void OnItemEquip(object sender, ItemEquipResponse message)
        {
            if (message.Result == Result.Success)
            {
                if (pendingEquip != null)
                {
                    if (this.isEquip)
                        EquipManager.Instance.OnEquipItem(pendingEquip); //穿装备
                    else
                        EquipManager.Instance.OnUnEquipItem(pendingEquip.EquipInfo.Slot); //穿脱装备
                    pendingEquip = null;
                }
            }
        }

    }
}
