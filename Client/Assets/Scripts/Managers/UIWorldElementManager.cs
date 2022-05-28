using Entities;
using Models;
using Managers;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// ����Ԫ�ع�����
/// </summary>
public class UIWorldElementManager : MonoSingleton<UIWorldElementManager> 
{
    /// <summary>
    /// �������Ԫ��
    /// </summary>
    public GameObject nameBarPrefab;

    /// <summary>
    /// ��������Ԫ��
    /// </summary>
    public GameObject monsterBarPrefab;

    /// <summary>
    /// NPC״̬Ԫ��
    /// </summary>
    public GameObject npcStatusPrefab;

    /// <summary>
    /// �������Ԫ�ؼ���
    /// </summary>
    private Dictionary<Transform,GameObject> elementNames = new Dictionary<Transform,GameObject>();

    /// <summary>
    /// ��������Ԫ�ؼ���
    /// </summary>
    private Dictionary<int, GameObject> elementMonster = new Dictionary<int, GameObject>();

    /// <summary>
    /// NPC״̬Ԫ�ؼ���
    /// </summary>
    private Dictionary<Transform, GameObject> elementStatus = new Dictionary<Transform, GameObject>();

    /// <summary>
    /// ������Bar
    /// </summary>
    /// <param name="owner">����Transform</param>
    /// <param name="character">��ɫ</param>
    public void AddCharacterNameBar(Transform owner, Character character)
    {

        if (character.Info.Type == Charlotte.Proto.CharacterType.Monster) //����Bar
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
        else  //���Bar
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
    /// ������Bar
    /// </summary>
    /// <param name="owner">���Transform</param>
    public void RemoveCharacterNameBar(Transform owner)
    {
        if (this.elementNames.ContainsKey(owner)) //�ж��Ƿ����
        {
            GameObject tr = elementNames[owner];
            Destroy(tr);
            this.elementNames.Remove(owner);
        }
    }

    /// <summary>
    /// ���NPC״̬Bar
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
    /// ���NPC״̬Bar
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
