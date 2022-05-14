using Network;
using Common;
using Common.Data;
using Charlotte.Proto;
using Charlotte.Extend;
using GameServer.Entities;
using GameServer.Managers;

namespace GameServer.Services
{
    class MapService : Singleton<MapService>
    {
        public MapService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<MapEntitySyncRequest>(this.OnMapEntitySync);   //实体同步
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<MapTeleportRequest>(this.OnMapTeleport);         //地图传送
        }

        public void Init() => MapManager.Instance.Init();

        /// <summary>
        /// 服务端处理实体同步事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        private void OnMapEntitySync(NetConnection<NetSession> sender, MapEntitySyncRequest request)
        {
            Character character = sender.Session.Character;
            MapManager.Instance[character.Info.mapId[0]].UpdateEntity(request.entitySync);
            Log.InfoFormat("MapEntitySync-> Character [{0}:{1}] EntityID [{2}] Evt [{3}] Entity [{4}]", character.Id, character.Info.Name, request.entitySync.Entity.Id, request.entitySync.Event, request.entitySync.Entity.String());
        }

        /// <summary>
        /// 发送实体同步更新
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="entity"></param>
        public void SendEntityUpdate(NetConnection<NetSession> conn, NEntitySync entity)
        {
            conn.Session.Response.mapEntitySync = new MapEntitySyncResponse();
            conn.Session.Response.mapEntitySync.entitySyncs.Add(entity);
            conn.SendResponse();
        }

        /// <summary>
        /// 服务端处理地图传送请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        void OnMapTeleport(NetConnection<NetSession> sender, MapTeleportRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("MapTeleport-> Character[{0}:{1}] TeleporterID[{2}]", character.Id,character.Name,request.teleporterId);
            if (DataManager.Instance.Teleporters.ContainsKey(request.teleporterId)) //效验传送点是否存在
            {
                TeleporterDefine source = DataManager.Instance.Teleporters[request.teleporterId];//读取数据
                if (source.LinkTo != 0 || DataManager.Instance.Teleporters.ContainsKey(source.LinkTo))  //效验链接是否正确
                {
                    MapManager.Instance[source.MapID].CharacterLeave(character); //当前角色离开地图
                    TeleporterDefine target = DataManager.Instance.Teleporters[source.LinkTo]; //拉取传送目标
                    character.Position = target.Position;
                    character.Direction = target.Direction;
                    MapManager.Instance[target.MapID].CharacterEnter(sender, character); //进入新地图的传送点位置
                }
                else
                    Log.WarningFormat("Source->TeleporterID:[{0}] LinkToID:[{1}] Not Existed", request.teleporterId, source.LinkTo);
            }
            Log.WarningFormat("Source->TeleporterID:[{0}] Not Existed", request.teleporterId);
            return;
        }
    }
}
