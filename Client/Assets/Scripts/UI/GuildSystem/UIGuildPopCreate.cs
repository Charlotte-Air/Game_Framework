using Services;
using UnityEngine.UI;

/// <summary>
/// 创建公会系统
/// </summary>
public class UIGuildPopCreate : UIWindow
{
    /// <summary>
    /// 公会名称
    /// </summary>
    public InputField inputName;
    /// <summary>
    /// 公会介绍
    /// </summary>
    public InputField inputNotice;

    private void Start()
    {
        GuildService.Instance.OnGuildCreateResult = OnGuildCreate; //监听创建公会事件
    }

    private void OnDestroy()
    {
        GuildService.Instance.OnGuildCreateResult = null;
    }

    /// <summary>
    /// 点击创建公会
    /// </summary>
    public override void OnYesClick()
    {
        if (string.IsNullOrEmpty(inputName.text))
        {
            MessageBox.Show("请输入公会名称", "错误", MessageBoxType.Error);
            return;
        }
        if (inputName.text.Length < 2 || inputName.text.Length > 10)
        {
            MessageBox.Show("请输入2-10个字符的公会名称", "错误", MessageBoxType.Error);
            return;
        }

        if (string.IsNullOrEmpty(inputNotice.text))
        {
            MessageBox.Show("请输入公会介绍", "错误", MessageBoxType.Error);
            return;
        }
        if (inputNotice.text.Length < 3 || inputNotice.text.Length > 50)
        {
            MessageBox.Show("请输入3-50个字符的公会介绍", "错误", MessageBoxType.Error);
            return;
        }

        GuildService.Instance.SendGuildCreate(inputName.text, inputNotice.text);
    }

    /// <summary>
    /// 回调创建公会
    /// </summary>
    /// <param name="result"></param>
    void OnGuildCreate(bool result)
    {
        if(result)
            this.Close(WindowResult.Yes);
    }
}
