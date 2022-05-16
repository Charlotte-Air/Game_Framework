using Common.Data;
using Charlotte.Proto;

namespace Models
{
    class User : Singleton<User>
    {
        /// <summary>
        /// 网络用户信息缓存
        /// </summary>
        private NUserInfo userInfo;

        /// <summary>
        /// 网络用户信息缓存
        /// </summary>
        public NUserInfo Info { get { return userInfo; } }

        /// <summary>
        /// 地图表缓存
        /// </summary>
        public MapDefine CurrentMapData { get; set; }

        /// <summary>
        /// 网络角色信息缓存
        /// </summary>
        public NCharacterInfo CurrentCharacter { get; set; }

        /// <summary>
        /// 用户输入控制器缓存
        /// </summary>
        public PlayerInputController CurrentCharacterObject { get; set; }

        /// <summary>
        /// 网络组队信息缓存
        /// </summary>
        public NTeamInfo TeamInfo { get; set; }

        /// <summary>
        /// 设置网络信息缓存
        /// </summary>
        public void SetupUserInfo(NUserInfo info)
        {
            this.userInfo = info;
        }

        /// <summary>
        /// 添加用户金币
        /// </summary>
        /// <param name="gold">金币数</param>
        public void AddGold(int gold)
        {
            this.CurrentCharacter.Gold += gold;
        }
    }
}
