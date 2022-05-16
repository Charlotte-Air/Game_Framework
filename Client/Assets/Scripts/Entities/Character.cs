using UnityEngine;
using Charlotte.Proto;

namespace Entities
{
    public class Character : Entity
    {
        /// <summary>
        /// NCharacter信息
        /// </summary>
        public NCharacterInfo Info;

        /// <summary>
        /// 本地角色文件
        /// </summary>
        public Common.Data.CharacterDefine Define;

        public int Id { get { return this.Info.Id; } }
        public string Name
        {
            get
            {
                if (this.Info.Type == CharacterType.Player)
                    return this.Info.Name;
                else
                    return this.Define.Name;
            }
        }

        /// <summary>
        /// 判断是否玩家
        /// </summary>
        public bool IsPlayer
        {
            get
            {
                return this.Info.Type == CharacterType.Player;
            }
        }

        /// <summary>
        /// 判断是否玩家自己
        /// </summary>
        public bool IsCurrentPlayer
        {
            get
            {
                if (!IsPlayer) return false;
                return this.Info.Id == Models.User.Instance.CurrentCharacter.Id;
            }
        }

        public Character(NCharacterInfo info) : base(info.Entity)
        {
            this.Info = info;
            this.Define = DataManager.Instance.Characters[info.ConfigId];
        }

        /// <summary>
        /// 向前
        /// </summary>
        public void MoveForward()
        {
            this.speed = this.Define.Speed;
        }

        /// <summary>
        /// 向后
        /// </summary>
        public void MoveBack()
        {
            this.speed = -this.Define.Speed;
        }

        /// <summary>
        /// 奔跑
        /// </summary>
        public void MoveRush()
        {
            this.speed = (100+this.Define.Speed);
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            this.speed = 0;
        }

        /// <summary>
        /// 设置方向
        /// </summary>
        public void SetDirection(Vector3Int direction)
        {
            this.direction = direction;
        }

        /// <summary>
        /// 设置位置
        /// </summary>
        public void SetPosition(Vector3Int position)
        {
            this.position = position;
        }
    }
}
