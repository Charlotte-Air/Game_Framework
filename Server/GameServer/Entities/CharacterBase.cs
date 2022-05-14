using Common.Data;
using Charlotte.Proto;
using GameServer.Core;
using GameServer.Managers;

namespace GameServer.Entities
{
    class CharacterBase : Entity
    {
        public int Id { get; set; }
        public string Name { get => this.Info.Name; }
        /// <summary>
        /// 角色网络信息缓存
        /// </summary>
        public NCharacterInfo Info;
        /// <summary>
        /// 角色表缓存
        /// </summary>
        public CharacterDefine Define;

        public CharacterBase(Vector3Int pos, Vector3Int dir):base(pos,dir){ }

        public CharacterBase(CharacterType type, int configId, int level, Vector3Int pos, Vector3Int dir) : base(pos, dir)
        {
            this.Info = new NCharacterInfo();
            this.Info.Type = type;
            this.Info.Level = level;
            this.Info.ConfigId = configId;
            this.Info.Entity = this.EntityData;
            this.Info.EntityId = this.entityId;
            this.Define = DataManager.Instance.Characters[this.Info.ConfigId];
            this.Info.Name = this.Define.Name;
        }
    }
}
