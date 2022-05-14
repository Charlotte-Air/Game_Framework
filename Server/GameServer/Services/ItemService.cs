using Network;
using Common;
using Charlotte.Proto;
using GameServer.Entities;
using GameServer.Managers;

namespace GameServer.Services
{
    class ItemService : Singleton<ItemService>
    {
        public ItemService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<ItemBuyRequest>(this.OnItemBuy);        //购买道具
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<ItemEquipRequest>(this.OnItemEquip);  //购买装备
        }

        public void Init()
        {

        }

        /// <summary>
        /// 服务端处理用户道具购买请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        private void OnItemBuy(NetConnection<NetSession> sender, ItemBuyRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("ItemBuy-> Character [{0}:{1}] ShopID [{2}]  ShopItemID [{3}] ",character.Id,character.Name,request.shopId,request.shopItemId);
            var result = ShopManager.Instance.BuyItem(sender, request.shopId, request.shopItemId);
            sender.Session.Response.itemBuy = new ItemBuyResponse();
            sender.Session.Response.itemBuy.Result = result;
            sender.SendResponse();
        }

        /// <summary>
        /// 服务端处理用户装备请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        private void OnItemEquip(NetConnection<NetSession> sender, ItemEquipRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("EquipItem-> Character [{0}:{1}] SlopID [{2}] ItemID [{3}] Equip [{4}] ", character.Id,character.Name,request.Slot, request.itemId,request.isEquip);
            var result = EquipManager.Instance.EquipItem(sender, request.Slot, request.itemId, request.isEquip);
            sender.Session.Response.itemEquip = new ItemEquipResponse();
            sender.Session.Response.itemEquip.Result = result;
            sender.SendResponse();
        }
    }
}
