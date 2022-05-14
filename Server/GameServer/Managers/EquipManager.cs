using Network;
using Common;
using Charlotte.Proto;
using GameServer.Entities;
using GameServer.Services;

namespace GameServer.Managers
{   
    /// <summary>
    /// 装备管理器
    /// </summary>
    class EquipManager : Singleton<EquipManager>
    {
        /// <summary>
        /// 装备
        /// </summary>
        /// <param name="serder"></param>
        /// <param name="slot">槽</param>
        /// <param name="itemId">装备ID</param>
        /// <param name="isEquip">是否装备</param>
        /// <returns></returns>
        public Result EquipItem(NetConnection<NetSession> serder, int slot, int itemId, bool isEquip)
        {
            Character character = serder.Session.Character;
            if (character.ItemManager.characterItems.ContainsKey(itemId))
            {
                //byte[] newData = UpdateEquip(character.Data.Equips, slot, itemId, isEquip,out newData);
                character.Data.Equips = UpdateEquip(character.Data.Equips, slot, itemId, isEquip);
                DBService.Instance.Save();
                return Result.Success;
            }
            return Result.Failed;
        }

        /// <summary>
        /// 更新装备
        /// </summary>
        /// <param name="equuipData">数据字节流</param>
        /// <param name="slot">槽</param>
        /// <param name="itemId">装备ID</param>
        /// <param name="isEquip">是否装备</param>
        unsafe byte[] UpdateEquip(byte[] equuipData, int slot, int itemId, bool isEquip)
        {
            fixed (byte* pt = equuipData)
            {
                int* slotid = (int*) (pt + slot * sizeof(int));
                if (isEquip)
                    *slotid = itemId;
                else
                    *slotid = 0;
                return equuipData;
            }
        }

    }
}
