using UnityEngine;
using Charlotte.Proto;
using Charlotte.Extend;

namespace Entities
{
    public class Entity
    {
        /// <summary>
        /// 实体ID
        /// </summary>
        public int entityId;

        /// <summary>
        /// 实体位置
        /// </summary>
        public Vector3Int position;

        /// <summary>
        /// 实体方向
        /// </summary>
        public Vector3Int direction;

        /// <summary>
        /// 实体速度
        /// </summary>
        public int speed;

        private NEntity entityData;
        /// <summary>
        ///  网络NEntity数据 </summary>
        /// </summary>
        public NEntity EntityData  //用与服务端同步储存
        {
            get
            {
                UpdateEntityData();  //实现数据更新
                return entityData;
            }
            set {
                entityData = value;
                this.SetEntityData(value);
            }
        }

        public Entity(NEntity entity)
        {
            this.entityId = entity.Id;
            this.entityData = entity;
            this.SetEntityData(entity);
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="delta"></param>
        public virtual void OnUpdate(float delta)
        {
            if (this.speed != 0)
            {
                Vector3 dir = this.direction;
                this.position += Vector3Int.RoundToInt(dir * speed * delta / 100f); //实体操作
            }
            entityData.Position.FromVector3Int(this.position);
            entityData.Direction.FromVector3Int(this.direction);
            entityData.Speed = this.speed;
        }

        /// <summary>
        /// NEntity数据转换
        /// </summary>
        /// <param name="entity">NEntity数据</param>
        public void SetEntityData(NEntity entity)
        {
            this.position = this.position.FromNVector3(entity.Position);
            this.direction = this.direction.FromNVector3(entity.Direction);
            this.speed = entity.Speed;
        }

        /// <summary>
        /// 更新实体数据
        /// </summary>
        private void UpdateEntityData()
        {
            entityData.Position.FromVector3Int(this.position);
            entityData.Direction.FromVector3Int(this.direction);
            entityData.Speed = this.speed;
        }
    }
}
