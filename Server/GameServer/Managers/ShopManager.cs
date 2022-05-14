using Network;
using Common;
using Common.Data;
using Charlotte.Proto;
using GameServer.Services;

namespace GameServer.Managers
{   
    /// <summary>
    /// 商店管理器
    /// </summary>
    class ShopManager : Singleton<ShopManager>
    {
        /// <summary>
        /// 购买道具
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="shopId">商店ID</param>
        /// <param name="shopItemId">道具ID</param>
        /// <returns></returns>
        public Result BuyItem(NetConnection<NetSession> sender, int shopId, int shopItemId)
        {
            if (DataManager.Instance.Shops.ContainsKey(shopId)) //验证商店ID是否存在
            {
                ShopItemDefine shopItem;
                if (DataManager.Instance.ShopItems[shopId].TryGetValue(shopItemId, out shopItem))
                {
                    if (sender.Session.Character.Gold >= shopItem.Price)
                    {
                        Log.InfoFormat("BuyItem-> Character [{0}:{1}]  Item [{2} Count:{3} Price:{4}]", sender.Session.Character.Id, sender.Session.Character.Name, shopItem.ItemID, shopItem.Count, shopItem.Price);
                        sender.Session.Character.ItemManager.AddItem(shopItem.ItemID, shopItem.Count);
                        sender.Session.Character.Gold -= shopItem.Price;
                        return Result.Success;
                    }
                }
                else
                    return Result.Failed;
            }
            return Result.Failed;
        }

    }
}
