namespace Common.Data
{   
    /// <summary>
    /// 商店表
    /// </summary>
    public class ShopDefine
    {
        /// <summary>
        /// 商店ID
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 商店名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }
    }
}
