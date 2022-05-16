using System.IO;
using UnityEngine;
using Common.Data;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;

public class DataManager : Singleton<DataManager>
{
    /// <summary>
    /// 路径
    /// </summary>
    public readonly string DataPath = "Assets/AssetBundle/Data/";

    /// <summary>
    /// 地图表集合
    /// </summary>
    public Dictionary<int, MapDefine> Maps = null;

    /// <summary>
    /// 角色表集合
    /// </summary>
    public Dictionary<int, CharacterDefine> Characters = null;

    /// <summary>
    /// 传送表集合
    /// </summary>
    public Dictionary<int, TeleporterDefine> Teleporters = null;

    /// <summary>
    /// 刷怪表集合
    /// </summary>
    public Dictionary<int, Dictionary<int, SpawnPointDefine>> SpawnPoints = null;

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
        Debug.LogFormat("DataManager -> Init.....");
    }

    /// <summary>
    /// 加载
    /// </summary>
    public void Load()
    {
        string json = File.ReadAllText(this.DataPath + "MapDefine.txt");
        this.Maps = JsonConvert.DeserializeObject<Dictionary<int, MapDefine>>(json);

        json = File.ReadAllText(this.DataPath + "CharacterDefine.txt");
        this.Characters = JsonConvert.DeserializeObject<Dictionary<int, CharacterDefine>>(json);

        json = File.ReadAllText(this.DataPath + "TeleporterDefine.txt");
        this.Teleporters = JsonConvert.DeserializeObject<Dictionary<int, TeleporterDefine>>(json);

        json = File.ReadAllText(this.DataPath + "NpcDefine.txt");
        this.Npcs = JsonConvert.DeserializeObject<Dictionary<int, NpcDefine>>(json);

        json = File.ReadAllText(this.DataPath + "ItemDefine.txt");
        this.Items = JsonConvert.DeserializeObject<Dictionary<int, ItemDefine>>(json);

        json = File.ReadAllText(this.DataPath + "ShopDefine.txt");
        this.Shops = JsonConvert.DeserializeObject<Dictionary<int, ShopDefine>>(json);

        json = File.ReadAllText(this.DataPath + "ShopItemDefine.txt");
        this.ShopItems = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, ShopItemDefine>>>(json);

        json = File.ReadAllText(this.DataPath + "EquipDefine.txt");
        this.Equips = JsonConvert.DeserializeObject<Dictionary<int, EquipDefine>>(json);

        json = File.ReadAllText(this.DataPath + "QuestDefine.txt");
        this.Quests = JsonConvert.DeserializeObject<Dictionary<int, QuestDefine>>(json);

        json = File.ReadAllText(this.DataPath + "SpawnPointDefine.txt");
        this.SpawnPoints = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, SpawnPointDefine>>>(json);
    }

    /// <summary>
    /// 加载数据
    /// </summary>
    public IEnumerator LoadData()
    {
        string json = File.ReadAllText(this.DataPath + "MapDefine.txt");
        this.Maps = JsonConvert.DeserializeObject<Dictionary<int, MapDefine>>(json);

        yield return null;

        json = File.ReadAllText(this.DataPath + "CharacterDefine.txt");
        this.Characters = JsonConvert.DeserializeObject<Dictionary<int, CharacterDefine>>(json);

        yield return null;

        json = File.ReadAllText(this.DataPath + "TeleporterDefine.txt");
        this.Teleporters = JsonConvert.DeserializeObject<Dictionary<int, TeleporterDefine>>(json);

        yield return null;

        json = File.ReadAllText(this.DataPath + "NpcDefine.txt");
        this.Npcs = JsonConvert.DeserializeObject<Dictionary<int, NpcDefine>>(json);

        yield return null;

        json = File.ReadAllText(this.DataPath + "ItemDefine.txt");
        this.Items = JsonConvert.DeserializeObject<Dictionary<int, ItemDefine>>(json);

        yield return null;

        json = File.ReadAllText(this.DataPath + "ShopDefine.txt");
        this.Shops = JsonConvert.DeserializeObject<Dictionary<int, ShopDefine>>(json);

        yield return null;

        json = File.ReadAllText(this.DataPath + "ShopItemDefine.txt");
        this.ShopItems = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, ShopItemDefine>>>(json);

        yield return null;

        json = File.ReadAllText(this.DataPath + "EquipDefine.txt");
        this.Equips = JsonConvert.DeserializeObject<Dictionary<int, EquipDefine>>(json);

        yield return null;

        json = File.ReadAllText(this.DataPath + "QuestDefine.txt");
        this.Quests = JsonConvert.DeserializeObject<Dictionary<int, QuestDefine>>(json);

        yield return null;

        json = File.ReadAllText(this.DataPath + "SpawnPointDefine.txt");
        this.SpawnPoints = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, SpawnPointDefine>>>(json);

        yield return null;
    }

#if UNITY_EDITOR
    public void SaveTeleporters()
    {
        string json = JsonConvert.SerializeObject(this.Teleporters, Formatting.Indented);
        File.WriteAllText(this.DataPath + "TeleporterDefine.txt", json);
    }

    public void SaveSpawnPoints()
    {
        string json = JsonConvert.SerializeObject(this.SpawnPoints, Formatting.Indented);
        File.WriteAllText(this.DataPath + "SpawnPointDefine.txt", json);
    }

#endif
}
