using System;
using Models;
using Network;
using Manager;
using UnityEngine;
using Common.Data;
using Charlotte.Proto;
using Charlotte.Extend;

namespace Services
{
    class MapService : Singleton<MapService>, IDisposable
    {
        /// <summary>
        /// 当前地图索引值
        /// </summary>
        public byte CurrentMapId = 0;
        public MapService()
        {
            MessageDistributer.Instance.Subscribe<MapCharacterEnterResponse>(this.OnMapCharacterEnter);    //角色进入地图消息响应
            MessageDistributer.Instance.Subscribe<MapCharacterLeaveResponse>(this.OnMapCharacterLeave);  //角色离开地图消息响应
            MessageDistributer.Instance.Subscribe<MapEntitySyncResponse>(this.OnMapEntitySync);                  //角色实体同步消息响应
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<MapCharacterEnterResponse>(this.OnMapCharacterEnter);
            MessageDistributer.Instance.Unsubscribe<MapCharacterLeaveResponse>(this.OnMapCharacterLeave);
        }

        public void Init()
        {

        }

        /// <summary>
        /// 角色进入地图回调
        /// </summary>
        private void OnMapCharacterEnter(object sender, MapCharacterEnterResponse response)
        {
            MapDefine map = DataManager.Instance.Maps[response.mapId[0]]; //取得地图ID
            Debug.LogFormat("MapCharacterEnter-> Map:{0} Count:{1}", response.mapId, response.Characters.Count);
            if (CurrentMapId != response.mapId[0] || DataManager.Instance.Maps.ContainsKey(response.mapId[0])) //判断当前地图跟加载地图是否一样
            {
                User.Instance.CurrentMapData = map;
                SceneManager.Instance.LoadScene(map.Resource, response); //加载地图资源
                this.CurrentMapId = response.mapId[0];
                SoundManager.Instance.PlayMusic(map.Music);
            }
            else
            {
                Debug.LogErrorFormat("EnterMap-> Map {0} Not Existed", response.mapId);
                return;
            }
        }

        /// <summary>
        /// 角色离开地图回调
        /// </summary>
        private void OnMapCharacterLeave(object sender, MapCharacterLeaveResponse response)
        {
            Debug.LogFormat("MapCharacterLeave-> CharacterID:{0}", response.EntityId);
            if (response.EntityId != User.Instance.CurrentCharacter.EntityId) //判断当时离开的玩家是否是自己
                CharacterManager.Instance.RemoveCharacter(response.EntityId);
            else
                CharacterManager.Instance.Clear();
        }

        /// <summary>
        /// 实体同步事件
        /// </summary>
        /// <param name="entityEvent">实体事件</param>
        /// <param name="entity">NEntity</param>
        public void SendMapEntitySync(EntityEvent entityEvent, NEntity entity)
        {
            Debug.LogFormat("MapEntityUpdateRequest-> ID:{0}:{1} POS:{2} DIR:{3} SPD:{4} ", entity.Id,User.Instance.CurrentCharacter.Name,entity.Position.String(), entity.Direction.String(), entity.Speed);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.mapEntitySync = new MapEntitySyncRequest();
            message.Request.mapEntitySync.entitySync = new NEntitySync()
            {
                Event = entityEvent, //当前事件
                Entity = entity //位置、方向、速度
            };
            message.Request.mapEntitySync.entitySync.Entity.Id = entity.Id; //角色ID
            NetClient.Instance.SendMessage(message);
        }

        /// <summary>
        /// 同步事件回调
        /// </summary>
        private void OnMapEntitySync(object sender, MapEntitySyncResponse response)
        {
            //System.Text.StringBuilder sb = new System.Text.StringBuilder();
            //sb.AppendFormat("MapEntityUpdateResponse: Entitys:{0}", response.entitySyncs.Count);
            //sb.AppendLine();
            foreach (var entity in response.entitySyncs)
            {
                EntityManager.Instance.OnEntitySync(entity);
                //sb.AppendFormat("    [{0}]evt:{1} entity:{2}", entity.Id, entity.Event, entity.Entity.String());
                //sb.AppendLine();
            }
            //Debug.Log(sb.ToString());
        }

        /// <summary>
        /// 发送地图传送
        /// </summary>
        /// <param name="teleporterID">地图ID</param>
        public void SendMapTeleport(int teleporterID)
        {
            Debug.LogFormat("MapteleportRequest-> TeleporterID:{0}", teleporterID);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.mapTeleport = new MapTeleportRequest();
            message.Request.mapTeleport.teleporterId = teleporterID;
            NetClient.Instance.SendMessage(message);
        }

    }
}