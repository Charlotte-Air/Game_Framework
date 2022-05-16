using Services;
using UnityEngine.UI;
using Charlotte.Proto;
using Sirenix.OdinInspector;

[Title("注册系统", Subtitle = "设置", TitleAlignment = TitleAlignments.Split, Bold = true, HorizontalLine = true)]
public class UIRegister : SerializedMonoBehaviour
{

    [TabGroup("输入框")]
    public InputField username;
    [TabGroup("输入框")]
    public InputField password;
    [TabGroup("输入框")]
    public InputField passwordConfirm;

    [TabGroup("按钮")]
    public Button buttonRegister;

    [TabGroup("GameObject")]
    public UILogin uiLogin;

    void Start () 
    {
        UserService.Instance.OnRegister = OnRegister;
    }

    /// <summary>
    /// 点击注册
    /// </summary>
    public void OnClickRegister()
    {
        if(string.IsNullOrEmpty(this.username.text)) //检查数据
        {
            MessageBox.Show("请输入账号");
            return;
        }
        if (string.IsNullOrEmpty(this.password.text))
        {
            MessageBox.Show("请输入密码");
            return;
        }
        if (string.IsNullOrEmpty(this.passwordConfirm.text))
        {
            MessageBox.Show("请输入确认密码");
            return;
        }
        if (this.password.text != this.passwordConfirm.text)
        {
            MessageBox.Show("两次输入的密码不一致");
            return;
        }
        UserService.Instance.SendRegister(this.username.text, this.password.text);
    }

    /// <summary>
    /// 注册回调
    /// </summary>
    /// <param name="result"></param>
    /// <param name="message"></param>
    void OnRegister(Result result,string message)
    {
        if(result == Result.Success)
            MessageBox.Show("注册成功、请重新登入", "提示", MessageBoxType.Information).OnYes = this.CloseRegister; //登录成功，进入角色选择
        else
            MessageBox.Show(message, "错误", MessageBoxType.Error);
    }

    void CloseRegister()
    {
        uiLogin.OnRegister();
    }
}
