using Charlotte.Proto;
using System.Collections.Generic;

namespace Common.Data
{
    /// <summary>
    /// 道具类型
    /// </summary>
    public enum ItemFunction
    {
        RecoverHP,
        RecoverMP,
        AddBuff,
        AddExp,
        AddMoney,
        Addltem,
        AddSkillPoint,
    }

    /// <summary>
    /// 道具表
    /// </summary>
    public class ItemDefine
    {
        /// <summary>
        /// 道具ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 道具名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// 道具类型
        /// </summary>
        public ItemType Type { get; set; }

        /// <summary>
        /// 类别
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// 道具等级
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// 限制职业
        /// </summary>
        public CharacterClass LimitClass { get; set; }

        /// <summary>
        /// 道具使用
        /// </summary>
        public bool CanUse { get; set; }

        /// <summary>
        /// 使用CD
        /// </summary>
        public float UseCD { get; set; }

        /// <summary>
        /// 购买价格
        /// </summary>
        public int Price { get; set; }

        /// <summary>
        /// 出售价格
        /// </summary>
        public int SellPrice { get; set; }

        /// <summary>
        /// 堆叠限制
        /// </summary>
        public int StackLimit { get; set; }

        /// <summary>
        /// 道具资源
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 道具功能
        /// </summary>
        public ItemFunction Function { get; set; }

        /// <summary>
        /// 功能参数
        /// </summary>
        public int Param { get; set; }

        /// <summary>
        /// 参数列表
        /// </summary>

        public List<int>Params { get; set; }
    }
}
