using Charlotte.Proto;

namespace Common.Data
{   
    /// <summary>
    /// 装备表
    /// </summary>
    public class EquipDefine
    {
        /// <summary>
        /// 装备ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 装备槽
        /// </summary>
        public EquipSlot Slot { get; set; }

        /// <summary>
        /// 装备职业
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// 力量
        /// </summary>
        public float STR { get; set; }

        /// <summary>
        /// 智力
        /// </summary>
        public float INT { get; set; }

        /// <summary>
        /// 敏捷
        /// </summary>
        public float DEX { get; set; }

        /// <summary>
        /// 生命
        /// </summary>
        public float HP { get; set; }

        /// <summary>
        /// 法力
        /// </summary>
        public float MP { get; set; }

        /// <summary>
        /// 物理攻击
        /// </summary>
        public float AD { get; set; }

        /// <summary>
        /// 法术攻击
        /// </summary>
        public float AP { get; set; }

        /// <summary>
        /// 物理防御
        /// </summary>
        public float DEF { get; set; }

        /// <summary>
        /// 法术防御
        /// </summary>
        public float MDEF { get; set; }

        /// <summary>
        /// 攻击速度
        /// </summary>
        public float SPD { get; set; }

        /// <summary>
        /// 暴击概率
        /// </summary>
        public float CRI { get; set; }

    }
}
