using Charlotte.Proto;
using GameServer.Core;

namespace GameServer.Entities
{
    class Entity
    {
        public int entityId { get => this.entityData.Id; }
    
        private Vector3Int position;
        /// <summary>
        /// 实体位置
        /// </summary>
        public Vector3Int Position
        {
            get { return position; }
            set 
            {
                position = value;
                this.entityData.Position = position;
            }
        }

        private Vector3Int direction;
        /// <summary>
        /// 实体方向
        /// </summary>
        public Vector3Int Direction
        {
            get { return direction; }
            set
            {
                direction = value;
                this.entityData.Direction = direction;
            }
        }

        private int speed;
        /// <summary>
        /// 实体速度
        /// </summary>
        public int Speed
        {
            get { return speed; }
            set
            {
                speed = value;
                this.entityData.Speed = speed;
            }
        }

        private NEntity entityData;
        /// <summary>
        /// 网络实体Data
        /// </summary>
        public NEntity EntityData
        {
            get { return entityData; }
            set
            {
                entityData = value;
                this.SetEntityData(value);
            }
        }

        public Entity(Vector3Int pos,Vector3Int dir)
        {
            this.entityData = new NEntity();
            this.entityData.Position = pos;
            this.entityData.Direction = dir;
            this.SetEntityData(this.entityData);
        }

        public Entity(NEntity entity)
        {
            this.entityData = entity;
        }

        /// <summary>
        /// 设置实体数据
        /// </summary>
        /// <param name="entity">NEntity</param>
        public void SetEntityData(NEntity entity)
        {
            this.Position = entity.Position;
            this.Direction = entity.Direction;
            this.speed = entity.Speed;
        }
    }
}
