using Common;
using System.IO;
using Common.Data;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace GameServer.Managers
{
    public class DataManager : Singleton<DataManager>
    {
        /// <summary>
        /// 路径
        /// </summary>
        internal string DataPath;
        /// <summary>
        /// 地图表集合
        /// </summary>
        internal Dictionary<int, MapDefine> Maps = null;
        /// <summary>
        /// 角色表集合
        /// </summary>
        internal Dictionary<int, CharacterDefine> Characters = null;
        /// <summary>
        /// 传送表集合
        /// </summary>
        internal Dictionary<int, TeleporterDefine> Teleporters = null;
        /// <summary>
        /// 刷怪重生点集合
        /// </summary>
        public Dictionary<int, Dictionary<int, SpawnPointDefine>> SpawnPoints = null;
        /// <summary>
        /// 怪物表集合
        /// </summary>
        public Dictionary<int, Dictionary<int, SpawnRuleDefine>> SpawnRules = null;
        /// <summary>
        /// NPC表集合
        /// </summary>
        public Dictionary<int, NpcDefine> Npcs = null;
        /// <summary>
        /// 道具表集合
        /// </summary>
        public Dictionary<int, ItemDefine> Items = null;
        /// <summary>
        /// 商店表集合
        /// </summary>
        public Dictionary<int, ShopDefine> Shops = null;
        /// <summary>
        /// 商店道具表集合
        /// </summary>
        public Dictionary<int, Dictionary<int, ShopItemDefine>> ShopItems = null;
        /// <summary>
        /// 装备表集合
        /// </summary>
        public Dictionary<int, EquipDefine> Equips = null;
        /// <summary>
        /// 任务表集合
        /// </summary>
        public Dictionary<int, QuestDefine> Quests = null;

        public DataManager()
        {
            this.DataPath = "Data/";
            Log.Info("DataManager -> DataManager()");
        }

        /// <summary>
        /// 加载
        /// </summary>
        internal void Load()
        {
            string json = File.ReadAllText(this.DataPath + "MapDefine.txt");
            this.Maps = JsonConvert.DeserializeObject<Dictionary<int, MapDefine>>(json);

            json = File.ReadAllText(this.DataPath + "CharacterDefine.txt");
            this.Characters = JsonConvert.DeserializeObject<Dictionary<int, CharacterDefine>>(json);

            json = File.ReadAllText(this.DataPath + "TeleporterDefine.txt");
            this.Teleporters = JsonConvert.DeserializeObject<Dictionary<int, TeleporterDefine>>(json);

            json = File.ReadAllText(this.DataPath + "NpcDefine.txt");
            this.Npcs = JsonConvert.DeserializeObject<Dictionary<int, NpcDefine>>(json);

            json = File.ReadAllText(this.DataPath + "NpcDefine.txt");
            this.Npcs = JsonConvert.DeserializeObject<Dictionary<int, NpcDefine>>(json);

            json = File.ReadAllText(this.DataPath + "ItemDefine.txt");
            this.Items = JsonConvert.DeserializeObject<Dictionary<int, ItemDefine>>(json);

            json = File.ReadAllText(this.DataPath + "ShopDefine.txt");
            this.Shops = JsonConvert.DeserializeObject<Dictionary<int, ShopDefine>>(json);

            json = File.ReadAllText(this.DataPath + "ShopItemDefine.txt");
            this.ShopItems = JsonConvert.DeserializeObject<Dictionary<int,Dictionary<int,ShopItemDefine>>>(json);

            json = File.ReadAllText(this.DataPath + "EquipDefine.txt");
            this.Equips = JsonConvert.DeserializeObject<Dictionary<int, EquipDefine>>(json);

            json = File.ReadAllText(this.DataPath + "QuestDefine.txt");
            this.Quests = JsonConvert.DeserializeObject<Dictionary<int, QuestDefine>>(json);

            json = File.ReadAllText(this.DataPath + "SpawnPointDefine.txt");
            this.SpawnPoints = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int,SpawnPointDefine>>>(json);

            json = File.ReadAllText(this.DataPath + "SpawnRuleDefine.txt");
            this.SpawnRules = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int,SpawnRuleDefine>>>(json);

        }
    }
}