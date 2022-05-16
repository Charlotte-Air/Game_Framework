using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
public class UIMessageBox : MonoBehaviour
{
    /// <summary>
    /// 标题内容
    /// </summary>
    public Text title;
    /// <summary>
    /// 消息内容
    /// </summary>
    public Text message;
    /// <summary>
    /// 提示图标
    /// </summary>
    public Image[] icons;
    /// <summary>
    /// 按钮功能
    /// </summary>
    public Button buttonYes;
    public Button buttonNo;
    public Button buttonClose;
    /// <summary>
    /// 按钮标题
    /// </summary>
    public Text buttonYesTitle;
    public Text buttonNoTitle;
    public UnityAction OnYes;
    public UnityAction OnNo;

    public void Init(string title, string message, MessageBoxType type = MessageBoxType.Information, string btnOK = "", string btnCancel = "")
    {
        if (!string.IsNullOrEmpty(title)) this.title.text = title;
        this.message.text = message;
        this.icons[0].enabled = type == MessageBoxType.Information;
        this.icons[1].enabled = type == MessageBoxType.Confirm;
        this.icons[2].enabled = type == MessageBoxType.Error;
        if (!string.IsNullOrEmpty(btnOK)) this.buttonYesTitle.text = btnOK;
        if (!string.IsNullOrEmpty(btnCancel)) this.buttonNoTitle.text = btnCancel;
        this.buttonYes.onClick.AddListener(OnClickYes);
        this.buttonNo.onClick.AddListener(OnClickNo);
        this.buttonNo.gameObject.SetActive(type == MessageBoxType.Confirm);
        if (type == MessageBoxType.Error)
            SoundManager.Instance.PlaySound(SoundDefine.SFX_Message_Error);
        else
            SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Confirm);
    }

    void OnClickYes()
    {
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Win_Yes);
        Destroy(this.gameObject);
        if (this.OnYes != null)
            this.OnYes();
    }

    void OnClickNo()
    {
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Win_No);
        Destroy(this.gameObject);
        if (this.OnNo != null)
            this.OnNo();
    }
}
