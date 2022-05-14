using Network;
using Common;
using Common.Data;
using Charlotte.Proto;
using GameServer.Entities;
using GameServer.Services;
using GameServer.Managers;
using System.Collections.Generic;

namespace GameServer.Models
{
    class Map
    {
        internal class ICharacterMapInfo
        {
            public NetConnection<NetSession> connection;
            public Character character;
            public ICharacterMapInfo(NetConnection<NetSession> conn, Character cha)
            {
                this.connection = conn;
                this.character = cha;
            }
        }

        Dictionary<int, ICharacterMapInfo> MapCharacters; //Map-Character (CharacterID - Key)

        MapDefine Define;
        public byte GetDefineID => this.Define.ID;
        public string GetDefineName => this.Define.Name;

        SpawnManager spawnManager;
        public SpawnManager GetSpawnManagerr { get => spawnManager; }

        MonsterManager MonsterManager;
        public MonsterManager GetMonsterManager { get => MonsterManager; }

        internal Map(MapDefine define)
        {
            this.Define = define;
            this.spawnManager = SpawnManager.Instance;
            this.spawnManager.Init(this);
            this.MonsterManager = MonsterManager.Instance;
            this.MonsterManager.Init(this);
            MapCharacters = new Dictionary<int, ICharacterMapInfo>();
        }

        internal void Update()
        {
            spawnManager.Update();
        }

        /// <summary>
        /// 怪物进入地图
        /// </summary>
        /// <param name="monster">怪物</param>
        internal void MonsterEnter(Monster monster)
        {
            Log.InfoFormat("MonsterEnter-> MapID [{0}] Monster [{1}:{2}]",this.Define.ID,monster.Id,monster.Name);
            foreach (var kv in this.MapCharacters)
            {
                this.AddCharacterEnterMap(kv.Value.connection, monster.Info);
            }
        }

        /// <summary>
        /// 角色进入地图
        /// </summary>
        /// <param name="character">角色</param>
        internal void CharacterEnter(NetConnection<NetSession> conn, Character character)
        {
            Log.InfoFormat("CharacterEnter-> MapID [{0}] Character [{1}:{2}]", this.Define.ID, character.Id, character.Name);

            character.Info.mapId = new byte[] { this.GetDefineID };
            this.MapCharacters[character.Id] = new ICharacterMapInfo(conn, character);
            conn.Session.Response.mapCharacterEnter = new MapCharacterEnterResponse();
            conn.Session.Response.mapCharacterEnter.mapId = new byte[] { this.Define.ID };

            foreach (var notify in this.MapCharacters)
            {
                conn.Session.Response.mapCharacterEnter.Characters.Add(notify.Value.character.Info);
                if (notify.Value.character != character)
                    this.AddCharacterEnterMap(notify.Value.connection, character.Info);
            }
            foreach (var notify in this.MonsterManager.Monsters)
            {
                conn.Session.Response.mapCharacterEnter.Characters.Add(notify.Value.Info);
            }
            conn.SendResponse();
        }

        /// <summary>
        /// 角色离开地图
        /// </summary>
        /// <param name="cha">角色</param>
        internal void CharacterLeave(Character character)
        {
            Log.InfoFormat("CharacterLeave-> MapID [{0}] Character [{1}:{2}]", this.Define.ID, character.Id, character.Name);
            foreach (var notify in this.MapCharacters)
            {
                this.SendCharacterLeaveMap(notify.Value.connection, character);
            }
            this.MapCharacters.Remove(character.Id);
        }

        /// <summary>
        /// 添加角色进入地图消息
        /// </summary>
        /// <param name="conn">消息</param>
        /// <param name="character">NCharacterInfo</param>
        void AddCharacterEnterMap(NetConnection<NetSession> conn, NCharacterInfo character)
        {
            if (conn.Session.Response.mapCharacterEnter == null)
            {
                conn.Session.Response.mapCharacterEnter = new MapCharacterEnterResponse();
                conn.Session.Response.mapCharacterEnter.mapId = new byte[] { this.Define.ID };
            }
            conn.Session.Response.mapCharacterEnter.Characters.Add(character);
            conn.SendResponse();
        }

        /// <summary>
        /// 发送角色离开地图消息
        /// </summary>
        /// <param name="conn">消息</param>
        /// <param name="character">Character</param>
        private void SendCharacterLeaveMap(NetConnection<NetSession> conn, Character character)
        {
            Log.InfoFormat("SendCharacterLeaveMap-> Session [{0}:{1}] MapID [{2}] Character [{3}:{4}]", conn.Session.Character.Id,conn.Session.Character.Info.Name,this.Define.ID,character.Id,character.Info.Name);
            conn.Session.Response.mapCharacterLeave = new MapCharacterLeaveResponse();
            conn.Session.Response.mapCharacterLeave.EntityId = character.entityId;
            conn.SendResponse();
        }

        /// <summary>
        /// 更新实体同步
        /// </summary>
        /// <param name="entity"></param>
        internal void UpdateEntity(NEntitySync entity)
        {
            foreach (var notify in this.MapCharacters) //遍历每个角色
            {
                if (notify.Value.character.entityId == entity.Entity.Id) //判断角色是否是自己
                {
                    notify.Value.character.Position = entity.Entity.Position;
                    notify.Value.character.Direction = entity.Entity.Direction;
                    notify.Value.character.Speed = entity.Entity.Speed;
                }
                else
                {   
                    MapService.Instance.SendEntityUpdate(notify.Value.connection, entity); //给其他玩家发送同步消息
                }
            }
        }
    }
}
