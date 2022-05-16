using Entities;
using Charlotte.Proto;
using System.Collections.Generic;

namespace Manager
{
    /// <summary>
    /// 实体通知接口
    /// </summary>
    interface IEntityNotify
    {
        void OnEntityRemoved();
        void OnEntityChanged(Entity entity);
        void OnEntityEvent(EntityEvent @entity);
    }

    /// <summary>
    /// 实体管理器
    /// </summary>
    class EntityManager :Singleton<EntityManager>
    {
        /// <summary>
        /// 实体集合
        /// </summary>
        Dictionary<int, Entity> entities = new Dictionary<int, Entity>();
        /// <summary>
        /// 实体通知集合
        /// </summary>
        Dictionary<int, IEntityNotify> notifiers = new Dictionary<int, IEntityNotify>();

        /// <summary>
        /// 实体通知注册
        /// </summary>
        public void RegisterEntityChangeNotify(int entityId, IEntityNotify notify)
        {
            this.notifiers[entityId] = notify;
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">实体对象</param>
        public void AddEntity(Entity entity)
        {
            entities[entity.entityId] = entity;
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity">实体对象</param>
        public void RemoveEntity(NEntity entity)
        {
            this.entities.Remove(entity.Id);
            if (notifiers.ContainsKey(entity.Id))
            {
                notifiers[entity.Id].OnEntityRemoved();
                notifiers.Remove(entity.Id);
            }
        }

        /// <summary>
        /// 实体同步
        /// </summary>
        /// <param name="data">数据</param>
        internal void OnEntitySync(NEntitySync data)
        {
            if (data == null) return;
            Entity entity = null;
            entities.TryGetValue(data.Entity.Id, out entity);
            if (entity != null)
            {
                if (data.Entity != null)
                    entity.EntityData = data.Entity;
                if (notifiers.ContainsKey(data.Entity.Id))
                {
                    notifiers[entity.entityId].OnEntityChanged(entity);    //通知数据变化
                    notifiers[entity.entityId].OnEntityEvent(data.Event);  //通知事件变化
                }
            }
        }
    }
}
