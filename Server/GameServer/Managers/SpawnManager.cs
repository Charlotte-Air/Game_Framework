using Common;
using GameServer.Models;
using System.Collections.Generic;

namespace GameServer.Managers
{   
    /// <summary>
    /// 刷怪规则管理器
    /// </summary>
    class SpawnManager : Singleton<SpawnManager>
    {
        /// <summary>
        /// 怪物列表
        /// </summary>
        private List<Spawner> Rules;
        /// <summary>
        /// 地图缓存
        /// </summary>
        private Map map;

        public void Init(Map map)
        {
            this.map = map;
            if (DataManager.Instance.SpawnRules.ContainsKey(map.GetDefineID))
            {
                Rules = new List<Spawner>(DataManager.Instance.SpawnRules[map.GetDefineID].Count);
                foreach (var define in DataManager.Instance.SpawnRules[map.GetDefineID].Values) //读取刷怪规则表
                {
                    this.Rules.Add(new Spawner(define, this.map)); //创建刷怪器
                }
            }
        }

        public void Update()
        {
            if (Rules == null || Rules.Count == 0) { return; }
            for (int i = 0; i < this.Rules.Count; i++)
            {
                this.Rules[i].Update();
            }
        }
    }
}
