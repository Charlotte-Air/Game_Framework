using Common;
using Charlotte.Proto;
using GameServer.Entities;
using GameServer.Models;
using System.Collections.Generic;

namespace GameServer.Managers
{   
    /// <summary>
    /// 怪物管理器
    /// </summary>
    class MonsterManager : Singleton<MonsterManager>
    {
        /// <summary>
        /// 地图缓存
        /// </summary>
        private Map Map;
        /// <summary>
        /// 怪物集合
        /// </summary>
        public Dictionary<int, Monster> Monsters = new Dictionary<int, Monster>();

        public void Init(Map map) => this.Map = map;

        /// <summary>
        /// 创建怪物
        /// </summary>
        /// <param name="spawnMonID">怪物ID</param>
        /// <param name="spawnLevel">怪物等级</param>
        /// <param name="position">位置</param>
        /// <param name="direction">方向</param>
        /// <returns></returns>
        internal Monster Create(int spawnMonID, int spawnLevel, NVector3 position, NVector3 direction)
        {
            Monster monster = new Monster(spawnMonID, spawnLevel, position, direction);
            EntityManager.Instance.AddEntity(this.Map.GetDefineID,monster);
            monster.Id = monster.entityId;
            monster.Info.EntityId = monster.entityId;
            monster.Info.mapId = new byte[] { this.Map.GetDefineID };
            Monsters[monster.Id] = monster;
            this.Map.MonsterEnter(monster);
            return monster;
        }

    }
}
