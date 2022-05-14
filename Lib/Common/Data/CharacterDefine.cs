using Charlotte.Proto;

namespace Common.Data
{
    /// <summary>
    /// 角色表
    /// </summary>
    public class CharacterDefine
    {
        /// <summary>
        /// DB(ID)
        /// </summary>
        public int TID { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// 职业
        /// </summary>
        public CharacterClass Class { get; set; }
        /// <summary>
        /// 资源
        /// </summary>
        public string Resource { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 速度
        /// </summary>
        public int Speed { get; set; }
        /// <summary>
        /// 身高
        /// </summary>
        public float Height { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }
    }
}
