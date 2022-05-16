using Models;
using Managers;
using UnityEngine.UI;

public class UIMain : MonoSingleton<UIMain>
{
    /// <summary>
    /// 姓名
    /// </summary>
    public Text avatarName;

    /// <summary>
    /// 等级
    /// </summary>
    public Text avatarLevel;

    /// <summary>
    /// 组队
    /// </summary>
    public UITeam TeamWindow;

    private bool isMain = true;
    public bool IsMain
    {
        get { return isMain; }
        set { isMain = value; }
    }

    protected override void OnStart () 
    {
        this.UpdateCharacterInfo();
    }

    /// <summary>
    /// 更新角色信息
    /// </summary>
    void UpdateCharacterInfo()
    {
        this.avatarName.text = string.Format("{0}[{1}]", User.Instance.CurrentCharacter.Name, User.Instance.CurrentCharacter.Id); 
        this.avatarLevel.text = User.Instance.CurrentCharacter.Level.ToString();
    }

    public void OnClickBag()
    {
        UIManager.Instance.Show<UIBag>();
    }

    public void OnClickCharEquip()
    {
        UIManager.Instance.Show<UICharEquip>();
    }

    public void OnClickQuest()
    {
        UIManager.Instance.Show<UIQuestSystem>();
    }

    public void OnClickFriend()
    {
        UIManager.Instance.Show<UIFriends>();
    }

    public void ShowTeamUI(bool show)
    {
        TeamWindow.ShowTeam(show);
    }

    public void OnClickGuild()
    {
        GuildManager.Instance.ShowGuild();
    }

    public void OnClickSetting()
    {
        UIManager.Instance.Show<UISetting>();
    }

    public void OnHide()
    {
        this.gameObject.SetActive(false);
        InputManager.Instance.ISMain = false;
    }

    public void OnShow()
    {
        this.gameObject.SetActive(true);
        InputManager.Instance.ISMain = true;
    }
}
