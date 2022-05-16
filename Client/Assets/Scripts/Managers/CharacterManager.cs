using System;
using Entities;
using Manager;
using System.Linq;
using UnityEngine;
using Charlotte.Proto;
using Charlotte.Extend;
using UnityEngine.Events;
using System.Collections.Generic;

namespace Services
{   
    /// <summary>
    /// 角色管理器
    /// </summary>
    class CharacterManager : Singleton<CharacterManager>, IDisposable
    {
        /// <summary>
        /// 角色集合
        /// </summary>
        public Dictionary<int, Character> Characters = new Dictionary<int, Character>();
        /// <summary>
        /// 角色进入事件
        /// </summary>
        public UnityAction<Character> OnCharacterEnter;
        /// <summary>
        /// 角色离开事件
        /// </summary>
        public UnityAction<Character> OnCharacterLeave;

        public CharacterManager()
        {
        }

        public void Dispose()
        {
        }

        public void Init()
        {
        }

        /// <summary>
        /// 清空角色
        /// </summary>
        public void Clear()
        {
            int[] keys = this.Characters.Keys.ToArray();
            foreach (var key in keys)
            {
                this.RemoveCharacter(key);
            }
            if (this.Characters.Count != 0)
            {
                this.Characters.Clear();
            }
        }

        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="cha">NCharacterInfo</param>
        public void AddCharacter(NCharacterInfo cha)
        {
            if (this.Characters.ContainsKey(cha.EntityId))
                return;
            Debug.LogFormat("AddCharacter-> [{0}:{1}] Map:{2} Entity:{3}", cha.Id, cha.Name, cha.mapId, cha.Entity.String());
            Character character = new Character(cha);
            this.Characters[cha.EntityId] = character;
            EntityManager.Instance.AddEntity(character);
            SceneManager.Instance.IsLoad = false;
            if (OnCharacterEnter != null)
                OnCharacterEnter(character);
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="entityId">实体ID</param>
        public void RemoveCharacter(int entityId)
        {
            Debug.LogFormat("RemoveCharacter-> {0}", entityId);
            if (this.Characters.ContainsKey(entityId))
            {
                EntityManager.Instance.RemoveEntity(this.Characters[entityId].Info.Entity);
                if (OnCharacterLeave != null)
                    OnCharacterLeave(this.Characters[entityId]);

                this.Characters.Remove(entityId);
            }
        }

        /// <summary>
        /// 获取角色
        /// </summary>
        /// <param name="id">实体ID</param>
        /// <returns></returns>
        public Character GetCharacter(int entityId)
        {
            Character character = null;
            this.Characters.TryGetValue(entityId, out character);
            return character;
        }

    }
}
