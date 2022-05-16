using Models;
using Charlotte.Proto;

namespace Assets.Scripts.Managers
{
    /// <summary>
    /// 组队管理器
    /// </summary>
    class TeamManager : Singleton<TeamManager>
    {
        public void Init()
        {

        }

        /// <summary>
        /// 更新组队信息
        /// </summary>
        /// <param name="team">NTeamInfo</param>
        public void UpdateTeamInfo(NTeamInfo team)
        {
            User.Instance.TeamInfo = team;
            ShowTeamUI(team != null);
        }

        /// <summary>
        /// 显示组队信息
        /// </summary>
        /// <param name="show">是否显示</param>
        public void ShowTeamUI(bool show)
        {
            if (UIMain.Instance != null)
                UIMain.Instance.ShowTeamUI(show);
        }
    }
}
