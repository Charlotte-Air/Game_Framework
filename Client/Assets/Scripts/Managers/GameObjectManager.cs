using Entities;
using Models;
using Services;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 游戏对象管理器
/// </summary>
public class GameObjectManager : MonoSingleton<GameObjectManager>
{
    /// <summary>
    /// 角色对象集合
    /// </summary>
    Dictionary<int, GameObject> Characters = new Dictionary<int, GameObject>();

    protected override void OnStart()
    {
        StartCoroutine(InitGameObjects()); //启用协程
        CharacterManager.Instance.OnCharacterEnter += OnCharacterEnter; //角色进入事件
        CharacterManager.Instance.OnCharacterLeave += OnCharacterLeave; //角色离开事件
    }

    private void OnDestroy()
    {
        CharacterManager.Instance.OnCharacterEnter -= OnCharacterEnter;
        CharacterManager.Instance.OnCharacterLeave -= OnCharacterLeave;
    }

    /// <summary>
    /// 角色进入
    /// </summary>
    /// <param name="cha"></param>
    void OnCharacterEnter(Character cha)
    {
        CreateCharacterObject(cha);
    }

    void OnCharacterLeave(Character character)
    {
        if (!Characters.ContainsKey(character.entityId))
        {
            return;
        }

        if (Characters[character.entityId] != null)
        {
            GameObject go = Characters[character.entityId];
            Destroy(go);
            this.Characters.Remove(character.entityId);
        }
    }

    /// <summary>
    /// 初始化游戏对象
    /// </summary>
    IEnumerator InitGameObjects()
    {
        foreach (var cha in CharacterManager.Instance.Characters.Values) //查找当前场景中所有角色分别创建对象
        {
            CreateCharacterObject(cha);
            yield return null;
        }
    }

    /// <summary>
    /// 创建角色对象
    /// </summary>
    /// <param name="character">角色</param>
    private void CreateCharacterObject(Character character)
    {
        if (!Characters.ContainsKey(character.entityId) || Characters[character.entityId] == null) //判断角色的ID是否重复过
        {
            Object obj = Resloader.Load<Object>(character.Define.Resource); //资源加载
            if(obj == null) //判断对象是否存在
            {
                Debug.LogErrorFormat("Character[{0}] Resource[{1}] not existed.",character.Info.ConfigId, character.Define.Resource);
                return;
            }
            GameObject go = (GameObject)Instantiate(obj,this.transform);                 //实例化角色对象
            go.name = "Character_" + character.Id + "_" + character.Name;               //角色名称赋值
            Characters[character.entityId] = go;
            UIWorldElementManager.Instance.AddCharacterNameBar(go.transform, character); //添加UI玩家条
        }
        this.InitGameObject(Characters[character.entityId], character);
    }

    /// <summary>
    /// 初始化游戏对象
    /// </summary>
    /// <param name="go">角色对象</param>
    /// <param name="character">角色</param>
    private void InitGameObject(GameObject go, Character character)
    {
        go.transform.position = GameObjectTool.LogicToWorld(character.position);  //服务端坐标转换成客户端世界坐标
        go.transform.forward = GameObjectTool.LogicToWorld(character.direction);
        EntityController ec = go.GetComponent<EntityController>(); //获取玩家实体控制器组件
        if (ec != null)
        {
            ec.entity = character;
            ec.isPlayer = character.IsCurrentPlayer;
        }
        PlayerInputController pc = go.GetComponent<PlayerInputController>(); //获取玩家输入控制器组件
        if (pc != null)
        {
            if (character.IsCurrentPlayer) //玩家初始化
            {
                User.Instance.CurrentCharacterObject = pc;
                MainPlayerCamera.Instance.player = go;
                pc.enabled = true;
                pc.character = character;
                pc.entityController = ec;
            }
            else
            {
                pc.enabled = false;
            }
        }
    }
}

