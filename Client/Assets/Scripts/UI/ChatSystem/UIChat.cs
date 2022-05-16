using Managers;
using UnityEngine;
using DG.Tweening;
using Candlelight.UI;
using Charlotte.Proto;
using UnityEngine.UI;

/// <summary>
/// 聊天系统
/// </summary>
public class UIChat : MonoBehaviour
{
    /// <summary>
    /// 聊天框文本
    /// </summary>
	public HyperText textArea;

    /// <summary>
    /// 私聊者
    /// </summary>
    public Text chatTarget;

    /// <summary>
    /// 频道
    /// </summary>
    public Dropdown channelSelect;

    /// <summary>
    /// 聊天输入框
    /// </summary>
    public InputField chatText;

    /// <summary>
    /// 频道视图
    /// </summary>
    public ChatView ChatTab;
    
    [Header("收缩聊天框")]
    public RectTransform Content;
    [Tooltip("图片")]
    public Image image;
    [Tooltip("启动")]
    private bool isOpen = false;

    void Start ()
    {
        this.ChatTab.OnTabSelect += OnDisplayChannelSelected;
        ChatManager.Instance.OnChat += RefreshUI;
    }

    void OnDestroy()
    {
        ChatManager.Instance.OnChat -= RefreshUI;
    }

	void Update ()
    {
        InputManager.Instance.IsInputMode = chatText.isFocused; //检测聊天输入控件是否拥有焦点
    }

    void OnDisplayChannelSelected(int idx)
    {
        ChatManager.Instance.displayChannel = (ChatManager.LocalChannel) idx;
        RefreshUI();
    }

    /// <summary>
    /// 更新UI
    /// </summary>
    public void RefreshUI()
    {
        this.textArea.text = ChatManager.Instance.GetCurrentMessages();
        this.channelSelect.value = (int)ChatManager.Instance.sendChannel - 1;
        if (ChatManager.Instance.SendChannel == ChatChannel.Private)
        {
            this.chatTarget.gameObject.SetActive(true);
            if (ChatManager.Instance.PrivateID != 0)
                this.chatTarget.text = ChatManager.Instance.PrivateName + ":";
            else
                this.chatTarget.text = "<无>:";
        }
        else
            this.chatTarget.gameObject.SetActive(false);
    }

    /// <summary>
    /// 聊天组件事件
    /// </summary>
    /// <param name="link"> 链接规则: "T":ID:Name" class"player>Name) </param>
    public void OnClickChatLink(HyperText text, HyperText.LinkInfo link)
    {
        if(string.IsNullOrEmpty(link.Name))
            return;
        if (link.Name.StartsWith("c:"))
        {
            string[] strs = link.Name.Split(":".ToCharArray());
            UIPopCharMenu menu = UIManager.Instance.Show<UIPopCharMenu>();
            menu.targetId = int.Parse(strs[1]);
            menu.targetName = strs[2];
        }
    }

    /// <summary>
    /// 点击发送
    /// </summary>
    public void OnClickSend()
    {
        OnEndInput(this.chatText.text);
    }

    /// <summary>
    /// 点击回车发送
    /// </summary>
    /// <param name="text"></param>
    public void OnEndInput(string text)
    {
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_ChatSend);
        if (!string.IsNullOrEmpty(text.Trim())) //判断是否输入为空
            this.SendChat(text);
        this.chatText.text = "";
    }

    /// <summary>
    /// 发送聊天
    /// </summary>
    /// <param name="content"></param>
    void SendChat(string content)
    {
        ChatManager.Instance.SendChat(content, ChatManager.Instance.PrivateID, ChatManager.Instance.PrivateName);
    }

    /// <summary>
    /// 聊天频道切换
    /// </summary>
    /// <param name="idx">频道索引值</param>
    public void OnSendChannelChanged(int idx)
    {
        if (ChatManager.Instance.sendChannel == (ChatManager.LocalChannel) (idx + 1))
            return;
        if (!ChatManager.Instance.SetSendChannel((ChatManager.LocalChannel) idx + 1))
            this.channelSelect.value = (int) ChatManager.Instance.sendChannel - 1;
        else
            this.RefreshUI();
    }
    public void Click()
    {
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Hide);
        if (isOpen)
        {
            isOpen = false;
            Content.DOAnchorPosX(-455, 0.33f);
        }
        else
        {
            isOpen = true;
            Content.DOAnchorPosX(0, 0.33f);
        }
    }
}
