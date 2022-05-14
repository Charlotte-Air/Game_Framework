using Network;
using Common;
using Charlotte.Proto;
using GameServer.Entities;

namespace GameServer.Services
{
    class BagService : Singleton<BagService>
    {
        public BagService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<BagSaveResponst>(this.OnBagSave); //背包处理
        }

        public void Init()
        {

        }

        /// <summary>
        ///  背包保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        void OnBagSave(NetConnection<NetSession> sender, BagSaveResponst request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("BagSaveRequest-> Character [{0}:{1}] Unlocked[{1}] ",character.Id, character.Name,request.BagInfo.Unlocked);
            if (request.BagInfo != null)
            {
                character.Data.Bag.Items = request.BagInfo.Items;
                DBService.Instance.Save();
            }
        }
    }
}
