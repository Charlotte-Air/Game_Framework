using Services;
using Common.Data;

namespace Managers
{
    /// <summary>
    /// 商店管理器
    /// </summary>
    public class ShopManager : Singleton<ShopManager>
    {
        public void Init()
        {
            NPCManager.Instance.RegisterNpcEvent(NpcFunction.InvokeShop, OnOpenShop); 
        }

        /// <summary>
        /// 点击打开商店
        /// </summary>
        /// <param name="npc">NpcDefine</param>
        /// <returns></returns>
        private bool OnOpenShop(NpcDefine npc)
        {
            this.ShowShop(npc.Param);
            return true;
        }

        /// <summary>
        /// 显示商店
        /// </summary>
        /// <param name="shopId">商店ID</param>
        public void ShowShop(int shopId)
        {
            ShopDefine shop;
            if (DataManager.Instance.Shops.TryGetValue(shopId,out shop)) //查找表的商店ID
            {
                UIShop uiShop = UIManager.Instance.Show<UIShop>(); //打开UI
                if (uiShop != null) //判断
                {
                    uiShop.SetShop(shop);
                }
            }
        }

        /// <summary>
        /// 点击购买
        /// </summary>
        /// <param name="shopId">商店ID</param>
        /// <param name="shopItemId">道具ID</param>
        /// <returns></returns>
        public bool BuyItem(int shopId, int shopItemId)
        {
            ItemService.Instance.SendBuyItem(shopId,shopItemId);
            return true;
        }
    }

}