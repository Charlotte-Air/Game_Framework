using Common;
using Common.Data;
using GameServer.Models;

namespace GameServer.Managers
{
    class Spawner
    {
        /// <summary>
        /// 刷怪表缓存
        /// </summary>
        private SpawnRuleDefine Define { get; }

        /// <summary>
        /// 当前刷怪点缓存
        /// </summary>
        private SpawnPointDefine spawnPoint;

        /// <summary>
        /// 地图缓存
        /// </summary>
        private Map map;

        /// <summary>
        /// 刷新时间
        /// </summary>
        //private float spawnTime;

        /// <summary>
        /// 消失时间
        /// </summary>
        private float unspawnTime = 0;

        private bool spawned = false;

        public Spawner(SpawnRuleDefine define, Map map)
        {
            this.map = map;
            this.Define = define;
            if (DataManager.Instance.SpawnPoints.ContainsKey(this.map.GetDefineID)) //判断地图是否存在刷怪点
            {
                if (DataManager.Instance.SpawnPoints[this.map.GetDefineID].ContainsKey(this.Define.SpawnPoint))
                    spawnPoint = DataManager.Instance.SpawnPoints[this.map.GetDefineID][this.Define.SpawnPoint];
                else
                    Log.InfoFormat("SpawnRule [{0}] SpawnPoint [{1}] Not Existed",this.Define.ID,this.Define.SpawnPoint);
            }
        }

        public void Update()
        {
            if(this.CanSpawn()) //判断每帧是否能刷怪
                this.Spawn();
        }

        bool CanSpawn()
        {
            if (this.spawned) //判断是否刷新过
                return false;
            if (this.unspawnTime + this.Define.SpawnPeriod > Time.time)
                return false;
            return true;
        }

        public void Spawn()
        {
            this.spawned = true;
            this.map.GetMonsterManager.Create(this.Define.SpawnMonID,this.Define.SpawnLevel,this.spawnPoint.Position, this.spawnPoint.Direction);
            Log.InfoFormat("Spawn-> Map[{0}] Spawn[{1}] Mon[{2}] Lv[{3}] AtPoint[{4}]", this.Define.MapID, this.Define.ID, this.Define.SpawnMonID, this.Define.SpawnLevel, this.Define.SpawnPoint);
        }

    }
}
