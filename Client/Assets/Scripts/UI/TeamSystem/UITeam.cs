using Models;
using Services;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UITeam : MonoBehaviour
{
    /// <summary>
    /// 组队标题
    /// </summary>
    public Text teamTitle;

    /// <summary>
    /// 成员列表
    /// </summary>
    public UITeamItem[] Members;

    /// <summary>
    /// 视图列表
    /// </summary>
    public ListView Iist;

    [Header("收缩组队框")]
    public RectTransform Content;
    [Tooltip("图片")]
    public Image image;
    [Tooltip("启动")]
    private bool isOpen = false;

    void Start () 
    {
        if (User.Instance.TeamInfo == null) //判断当前界面是否有成员
        {
            this.gameObject.SetActive(false); 
            return;
        }
        foreach (var item in Members)
        {
            this.Iist.AddItem(item);
        }
    }

    void OnEnable()
    {
        UpdateTeamUI();
    }

    /// <summary>
    /// 显示组队
    /// </summary>
    public void ShowTeam(bool show)
    {
        this.gameObject.SetActive(show);
        if (show)
        {
            UpdateTeamUI();
        }
    }

    /// <summary>
    /// 更新组队
    /// </summary>
    public void UpdateTeamUI()
    {
        if(User.Instance.TeamInfo==null) return;
        this.teamTitle.text = string.Format("当前队伍({0}/5)", User.Instance.TeamInfo.Members.Count);  //更新队伍信息
        for (int i = 0; i < 5; i++)
        {
            if (i < User.Instance.TeamInfo.Members.Count)
            {
                this.Members[i].SetMemberInfo(i , User.Instance.TeamInfo.Members[i] , User.Instance.TeamInfo.Members[i].Id==User.Instance.TeamInfo.Leader);
                this.Members[i].gameObject.SetActive(true);
                Click();
            }
            else
                this.Members[i].gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 离开组队
    /// </summary>
    public void OnClickLeave()
    {
        MessageBox.Show("确定需要离开队伍嘛？", "退出队伍", MessageBoxType.Confirm, "离开", "取消").OnYes = () =>
         {
            if (User.Instance.TeamInfo != null)
                TeamService.Instance.SendTeamLevaeRequest(User.Instance.TeamInfo.Id);
            else
                MessageBox.Show("抱歉~您现在没有队伍", "错误", MessageBoxType.Error);
         };
    }

    public void Click()
    {
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Hide);
        if (isOpen)
        {
            isOpen = false;
            Content.DOAnchorPosX(-78, 0.33f);
        }
        else
        {
            isOpen = true;
            Content.DOAnchorPosX(143, 0.33f);
        }
    }
}
