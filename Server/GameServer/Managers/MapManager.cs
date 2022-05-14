using Common;
using GameServer.Models;
using System.Collections.Generic;

namespace GameServer.Managers
{  
    /// <summary>
    /// 地图管理器
    /// </summary>
    class MapManager : Singleton<MapManager>
    {
        Dictionary<byte, Map> Maps; //地图集合

        public Map this[byte key] => this.Maps[key];

        public void Init()
        {
            Maps = new Dictionary<byte, Map>();
            foreach (var mapdefine in DataManager.Instance.Maps.Values) //角色加载资源
            {
                Map map = new Map(mapdefine);
                this.Maps[mapdefine.ID] = map;
                Log.InfoFormat("MapManager->Init Map [{0}:{1}]", map.GetDefineID, map.GetDefineName);
            }
        }


        public void Update()
        {
            foreach(var map in this.Maps.Values)
            {
                map.Update();
            }
        }
    }
}
