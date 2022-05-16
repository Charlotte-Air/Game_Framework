using Services;
using Managers;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class UIPopCharMenu : UIWindow,IDeselectHandler
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public int targetId;

    /// <summary>
    /// 用户姓名
    /// </summary>
    public string targetName;
    
    public void OnDeselect(BaseEventData eventData)
    {
        var ed = eventData as PointerEventData;
        if (ed.hovered.Contains(this.gameObject))//判断节点里面是否包含当前界面
            return;
        else
            this.Close(WindowResult.None);
    }

    public void OnEnable()
    {
        this.GetComponent<Selectable>().Select();
        this.Root.transform.position = Input.mousePosition + new Vector3(80, 0, 0);
    }

    /// <summary>
    /// 点击私聊
    /// </summary>
    public void OnChat()
    {
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Button2);
        ChatManager.Instance.StartPrivateChat(targetId, targetName);
        this.Close();
    }

    /// <summary>
    /// 点击添加好友
    /// </summary>
    public void OnAddFriend()
    {
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Button1);
        FriendService.Instance.SendFriendAddRequest(targetId, targetName);
        this.Close();
    }

    /// <summary>
    /// 点击邀请组队
    /// </summary>
    public void OnInviteTeam()
    {
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Button2);
        TeamService.Instance.SendTeamInviteRequest(targetId,targetName);
        this.Close();
    }
}
