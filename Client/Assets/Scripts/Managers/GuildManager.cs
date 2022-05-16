using Models;
using Charlotte.Proto;

namespace Managers
{   
    /// <summary>
    /// 公会管理器
    /// </summary>
    public class GuildManager : Singleton<GuildManager>
    {
        /// <summary>
        /// 公会信息缓存
        /// </summary>
        public NGuildInfo guildInfo;
        /// <summary>
        /// 成员信息缓存
        /// </summary>
        public NGuildMemberInfo myMemberInfo;

        public bool HasGuild
        {
            get { return this.guildInfo != null; }
        }

        public void Init(NGuildInfo guild)
        {
            this.guildInfo = guild;
            if (guild == null)
            {
                guildInfo = null;
                myMemberInfo = null;
                return;
            }
            foreach (var kv in guild.Members)
            {
                if (kv.characterId == User.Instance.CurrentCharacter.Id)
                {
                    myMemberInfo = kv;
                    break;
                }
            }
        }

        /// <summary>
        /// 显示公会
        /// </summary>
        public void ShowGuild()
        {
            if (this.HasGuild)
            {
                UIManager.Instance.Show<UIGuild>();
            }
            else
            {
                var win = UIManager.Instance.Show<UIGuildPopNoGuild>();
                win.OnClose += PopNoGuild_OnClose;
            }
        }

        /// <summary>
        /// 无公会功能界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="result"></param>
        public void PopNoGuild_OnClose(UIWindow sender, UIWindow.WindowResult result)
        {
            if (result == UIWindow.WindowResult.Yes)    //创建公会
                UIManager.Instance.Show<UIGuildPopCreate>();
            else if (result == UIWindow.WindowResult.NO)    //加入公会
                UIManager.Instance.Show<UIGuildList>();
        }
    }
}
