using Charlotte.Proto;

namespace Common.Data
{
    /// <summary>
    /// 刷怪点表
    /// </summary>
    public class SpawnPointDefine
    {
        /// <summary>
        /// 刷怪点 ID
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 地图  ID
        /// </summary>
        public int MapID { get; set;}
        /// <summary>
        /// 位置
        /// </summary>
        public NVector3 Position { get; set; }
        /// <summary>
        /// 方向
        /// </summary>
        public NVector3 Direction { get; set; }
    }
}
