using Charlotte.Proto;

namespace Common.Data
{
    /// <summary>
    /// 传送点表
    /// </summary>
    public class TeleporterDefine
    {
        /// <summary>
        /// 传送点ID
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 传送名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 地图ID
        /// </summary>
        public byte MapID { get; set;}
        /// <summary>
        /// 链接
        /// </summary>
        public int LinkTo { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Descript { get; set; }
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
