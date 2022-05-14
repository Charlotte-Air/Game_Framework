namespace Common.Data
{
    /// <summary>
    /// 地图表
    /// </summary>
    public class MapDefine
    {
        /// <summary>
        /// 地图ID
        /// </summary>
        public byte ID { get; set; }

        /// <summary>
        /// 地图名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 地图资源
        /// </summary>
        public string Resource { get; set; }

        /// <summary>
        /// 小地图
        /// </summary>
        public string MiniMap { get; set; }

        /// <summary>
        /// 音乐资源
        /// </summary>
        public string Music { get; set; }
    }
}
