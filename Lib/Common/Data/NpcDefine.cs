using Charlotte.Proto;

namespace Common.Data
{
    /// <summary>
    /// NPC类型
    /// </summary>
    public enum NpcType
    {
        None =0,
        Functional=1,
        Task,
    }

    /// <summary>
    /// NPC功能类型
    /// </summary>
    public enum NpcFunction
    {
        None = 0,
        InvokeShop = 1,
        InvokeInsrance = 2,
    }

    /// <summary>
    /// NPC表
    /// </summary>
    public class NpcDefine
    {
        /// <summary>
        /// NPC ID
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// NPC名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Descript { get; set;}

        /// <summary>
        /// 位置
        /// </summary>
        public NVector3 Position { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public NpcType Type { get; set; }

        /// <summary>
        /// 功能
        /// </summary>
        public NpcFunction Function { get; set; }

        /// <summary>
        /// 参数
        /// </summary>
        public int Param { get; set; }
    }
}
