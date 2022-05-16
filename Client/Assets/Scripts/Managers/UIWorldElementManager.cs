using Entities;
using Models;
using Managers;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 世界元素管理器
/// </summary>
public class UIWorldElementManager : MonoSingleton<UIWorldElementManager> 
{
    /// <summary>
    /// 玩家名称元素
    /// </summary>
    public GameObject nameBarPrefab;

    /// <summary>
    /// 怪物名称元素
    /// </summary>
    public GameObject monsterBarPrefab;

    /// <summary>
    /// NPC状态元素
    /// </summary>
    public GameObject npcStatusPrefab;

    /// <summary>
    /// 玩家名称元素集合
    /// </summary>
    private Dictionary<Transform,GameObject> elementNames = new Dictionary<Transform,GameObject>();

    /// <summary>
    /// 怪物名称元素集合
    /// </summary>
    private Dictionary<int, GameObject> elementMonster = new Dictionary<int, GameObject>();

    /// <summary>
    /// NPC状态元素集合
    /// </summary>
    private Dictionary<Transform, GameObject> elementStatus = new Dictionary<Transform, GameObject>();

    /// <summary>
    /// 添加玩家Bar
    /// </summary>
    /// <param name="owner">对象Transform</param>
    /// <param name="character">角色</param>
    public void AddCharacterNameBar(Transform owner, Character character)
    {

        if (character.Info.Type == Charlotte.Proto.CharacterType.Monster) //怪物Bar
        {
            if (!this.elementMonster.ContainsKey(character.entityId))
            {
                GameObject monsterBar = Instantiate(monsterBarPrefab, this.transform);
                monsterBar.name = "MonsterBar" + character.entityId;
                monsterBar.GetComponent<UIWorldElement>().owner = owner;
                monsterBar.GetComponent<UIWorldElement>().height = character.Define.Height;
                monsterBar.GetComponent<UINameBar>().character = character;
                monsterBar.GetComponent<UINameBar>().icon.overrideSprite = Resloader.Load<Sprite>(character.Define.Icon);
                monsterBar.SetActive(true);
                this.elementMonster[character.entityId] = monsterBar;
            }

        }
        else  //玩家Bar
        {
            if (character.Id != User.Instance.CurrentCharacter.Id)
            {
                GameObject goNameBar = Instantiate(nameBarPrefab, this.transform);
                goNameBar.name = "NameBar" + character.entityId;
                goNameBar.GetComponent<UIWorldElement>().owner = owner;
                goNameBar.GetComponent<UIWorldElement>().height = character.Define.Height;
                goNameBar.GetComponent<UINameBar>().character = character;
                goNameBar.GetComponent<UINameBar>().icon.overrideSprite = Resloader.Load<Sprite>(character.Define.Icon);
                goNameBar.SetActive(true);
                this.elementNames[owner] = goNameBar;
            }
        }
    }

    /// <summary>
    /// 清除玩家Bar
    /// </summary>
    /// <param name="owner">玩家Transform</param>
    public void RemoveCharacterNameBar(Transform owner)
    {
        if (this.elementNames.ContainsKey(owner)) //判断是否存在
        {
            GameObject tr = elementNames[owner];
            Destroy(tr);
            this.elementNames.Remove(owner);
        }
    }

    /// <summary>
    /// 添加NPC状态Bar
    /// </summary>
    /// <param name="owner">NPCTransform</param>
    /// <param name="status">NpcQuestStatus</param>
    public void AddNpcQuestStatus(Transform owner, NpcQuestStatus status)
    {
        if (this.elementStatus.ContainsKey(owner))
        {
            elementStatus[owner].GetComponent<UIQuestStatus>().SetQuestStatus(status);
        }
        else
        {
            GameObject go = Instantiate(npcStatusPrefab, this.transform);
            go.name = "NpcQuestStatus" + owner.name;
            go.GetComponent<UIWorldElement>().owner = owner;
            go.GetComponent<UIQuestStatus>().SetQuestStatus(status);
            go.SetActive(true);
            this.elementStatus[owner] = go;
        }
    }

    /// <summary>
    /// 清除NPC状态Bar
    /// </summary>
    /// <param name="owner">NPCTransform</param>
    public void RemoveNpcQuestStatus(Transform owner)
    {
        if (this.elementStatus.ContainsKey(owner))
        {
            Destroy(this.elementStatus[owner]);
            this.elementStatus.Remove(owner);
        }
    }
}
