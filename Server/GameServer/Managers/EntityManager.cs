using Common;
using GameServer.Entities;
using System.Collections.Generic;

namespace GameServer.Managers
{   /// <summary>
    /// 实体管理器
    /// </summary>
    class EntityManager :Singleton<EntityManager>
    {
        /// <summary>
        /// 实体起始值
        /// </summary>
        private int idx = 0;
        /// <summary>
        /// 实体集合
        /// </summary>
        public List<Entity> AllEntities = new List<Entity>();
        /// <summary>
        /// 地图实体集合
        /// </summary>
        public Dictionary<int, List<Entity>> MapEntities = new Dictionary<int, List<Entity>>();

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="mapId">地图ID</param>
        /// <param name="entity">实体</param>
        public void AddEntity(int mapId, Entity entity)
        {
            AllEntities.Add(entity);
            entity.EntityData.Id = ++this.idx; //加入管理器生成唯一的ID

            List<Entity> entities = null;      //判断当前地图
            if (!MapEntities.TryGetValue(mapId, out entities))
            {
                entities = new List<Entity>(); 
                MapEntities[mapId] = entities;
            }
            entities.Add(entity);
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="mapId">地图ID</param>
        /// <param name="entity">实体</param>
        public void RemoveEntity(int mapId, Entity entity)
        {
            this.AllEntities.Remove(entity);
            this.MapEntities[mapId].Remove(entity);
        }
    }
}
