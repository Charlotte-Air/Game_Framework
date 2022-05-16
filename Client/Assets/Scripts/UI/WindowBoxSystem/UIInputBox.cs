using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIInputBox : MonoBehaviour 
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
    /// 提示内容
    /// </summary>
    public Text tips;
    /// <summary>
    /// 输入提示
    /// </summary>
    public string emptyTips;
    /// <summary>
    /// 输入框
    /// </summary>
    public InputField input;
    /// <summary>
    /// 按钮功能
    /// </summary>
    public Button buttonYes;
    public Button buttonNo;
    /// <summary>
    /// 按钮标题
    /// </summary>
    public Text buttonYesTitle;
    public Text buttonNoTitle;
    public delegate bool SubmitHandler(string inputText, out string tips);
    public event SubmitHandler OnSubmit;
    public UnityAction OnCancel;
    
    public void Init(string title, string message, string btnOK="",string btnCancel ="",string emptyTips="")
    {
        if (!string.IsNullOrEmpty(title)) this.title.text = title;
        this.message.text = message;
        this.tips.text = null;
        this.OnSubmit = null;
        this.emptyTips = emptyTips;
        if (!string.IsNullOrEmpty(btnOK)) this.buttonYesTitle.text = title;
        if (!string.IsNullOrEmpty(btnCancel)) this.buttonNoTitle.text = title;
        this.buttonYes.onClick.AddListener(OnClickYes);
        this.buttonNo.onClick.AddListener(OnClickNo);
    }

    void OnClickYes()
    {
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Win_Yes);
        this.tips.text = "";
        if (string.IsNullOrEmpty(input.text))
        {
            this.tips.text = this.emptyTips;
            return;
        }
        if (OnSubmit != null)
        {
            string tips;
            if (!OnSubmit(this.input.text, out tips))
            {
                this.tips.text = tips;
                return;
            }
        }
        Destroy(this.gameObject);
    }

    void OnClickNo()
    {
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Win_No);
        Destroy(this.gameObject);
        if (this.OnCancel != null)
            this.OnCancel();
    }
}
