using System;
using Services;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Charlotte.Proto;
using Sirenix.OdinInspector;
using Button = UnityEngine.UI.Button;
using Toggle = UnityEngine.UI.Toggle;


[Title("登入系统", Subtitle = "设置", TitleAlignment = TitleAlignments.Split, Bold = true, HorizontalLine = true)]
public class UILogin : SerializedMonoBehaviour
{
    [TabGroup("输入框")]
    public InputField username;
    [TabGroup("输入框")]
    public InputField password;

    [TabGroup("按钮")]
    public Button buttonLogin;
    [TabGroup("按钮")]
    public Button buttonRegister;

	[TabGroup("登入")]
    public GameObject ObjLoing;
    [TabGroup("登入")]
    public RectTransform Login;

	[TabGroup("注册")]
    public GameObject ObjRegister;
    [TabGroup("注册")]
    public RectTransform Register;

	[TabGroup("切换")]
    private bool isOpen = true;

    [TabGroup("开关")]
    public Toggle UserRemember;
    [TabGroup("开关")]
    public Toggle UserAgreement ;
    
    [TabGroup("Test")]
    public ScrollRect view;

    private bool rememberuser;
    public bool RememberUser
    {
        get { return this.rememberuser; }
        set
        {
            SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Button1);
            rememberuser = value;
        }
    }

    private bool agreementuser;
    public bool AgreementUser
    {
        get { return this.agreementuser; }
        set
        {
            SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Button1);
            agreementuser = value;
        }
    }

    void Start () 
	{
		UserService.Instance.OnLogin = OnLogin; //服务层接收登入成功消息
    }

    /// <summary>
	/// 点击登入
	/// </summary>
	public void OnClickLogin()
    {
        if (string.IsNullOrEmpty(this.username.text))
        {
			MessageBox.Show("请输入账号");
			return;
        }
		if(string.IsNullOrEmpty(this.password.text))
        {
			MessageBox.Show("请输入密码");
			return;
		}
        if (!this.AgreementUser)
        {
            MessageBox.Show("抱歉~请您先同意用户协议再进行登入");
            return;
        }
        UserService.Instance.SendLogin(this.username.text, this.password.text);
    }

    /// <summary>
    /// 登入回调
    /// </summary>
    /// <param name="result"></param>
    /// <param name="message"></param>
    void OnLogin(Result result, string message)
    {
        if (result == Result.Success)
        {
            SoundManager.Instance.PlayMusic(SoundDefine.Music_Select);
			UnityEngine.SceneManagement.SceneManager.LoadScene("CharSelect");
        }
        else
            MessageBox.Show(message, "错误", MessageBoxType.Error);
    }

    public void OnRegister()
    {
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Hide);
        if (isOpen)
        {
            isOpen = false;
            this.ObjRegister.SetActive(true);
            Login.DOAnchorPosX(100, 0.33f);
            Register.DOAnchorPosX(960, 0.33f);
            this.ObjLoing.gameObject.SetActive(false);

        }
        else
        {
            isOpen = true;
            this.ObjLoing.SetActive(true);
            Login.DOAnchorPosX(960, 0.33f);
            Register.DOAnchorPosX(320, 0.33f);
            this.ObjRegister.gameObject.SetActive(false);
        }
    }
}
