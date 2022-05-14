namespace Common.Data
{
    /// <summary>
    /// 怪物表
    /// </summary>
    public class SpawnRuleDefine
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 地图 ID
        /// </summary>
        public int MapID { get; set; }
        /// <summary>
        /// 怪物ID
        /// </summary>
        public int SpawnMonID { get; set; }
        /// <summary>
        /// 怪物等级
        /// </summary>
        public int SpawnLevel{ get;set; }
        /// <summary>
        /// 怪物类型
        /// </summary>
        public SPAWN_TYPE SpawnType { get; set; }
        /// <summary>
        /// 刷怪点
        /// </summary>
        public int SpawnPoint { get; set; }
        public int SpawnPoints { get; set; }
        /// <summary>
        /// 刷怪周期
        /// </summary>
        public float SpawnPeriod { get; set; }
    }
}
