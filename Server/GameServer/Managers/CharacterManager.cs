using Common;
using Charlotte.Proto;
using GameServer.Entities;
using System.Collections.Generic;

namespace GameServer.Managers
{   /// <summary>
    /// 角色管理器
    /// </summary>
    class CharacterManager : Singleton<CharacterManager>
    {
        /// <summary>
        /// 角色缓存集合
        /// </summary>
        public Dictionary<int, Character> Characters = new Dictionary<int, Character>();

        public CharacterManager()
        {
        }

        public void Dispose()
        {
        }

        public void Init()
        {

        }

        public void Clear()
        {
            this.Characters.Clear();
        }

        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="cha">TCharacter</param>
        /// <returns></returns>
        public Character AddCharacter(TCharacter cha)
        {
            Character character = new Character(CharacterType.Player, cha);
            EntityManager.Instance.AddEntity(cha.MapID, character);
            character.Info.EntityId = character.entityId;
            this.Characters[character.Id] = character;
            return character;
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="characterId">角色ID</param>
        public void RemoveCharacter(int characterId)
        {
            if (this.Characters.ContainsKey(characterId))
            {
                var cha = this.Characters[characterId];
                EntityManager.Instance.RemoveEntity(cha.Data.MapID, cha);
                this.Characters.Remove(characterId);
            }
        }
        
        /// <summary>
        /// 获取角色
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns></returns>
        public Character GetCharacter(int characterId)
        {
            Character character = null;
            this.Characters.TryGetValue(characterId, out character);
            return character;
        }
    }
}
