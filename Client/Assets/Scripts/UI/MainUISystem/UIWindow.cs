using Managers;
using UnityEngine;

/// <summary>
/// UI窗口
/// </summary>
public abstract class UIWindow : MonoBehaviour
{
    public delegate void CloseHandler(UIWindow sender, WindowResult result); //委托
    public event CloseHandler OnClose; //事件
    public virtual System.Type type
    {
        get { return this.GetType(); }
    }
    /// <summary>
    /// 根节点
    /// </summary>
    public GameObject Root;
    /// <summary>
    /// 结果类型
    /// </summary>
    public enum WindowResult
    {
        None = 0,
        Yes,
        NO,
    }

    /// <summary>
    /// 关闭窗口事件
    /// </summary>
    public void Close(WindowResult result = WindowResult.None)
    {
        UIManager.Instance.Close(this.type);
        if (this.OnClose != null)
        {
            this.OnClose(this, result);
        }
        this.OnClose = null;
    }

    /// <summary>
    /// 点击关闭
    /// </summary>
    public virtual void OncloseClick()
    {
        this.Close();
        InputManager.Instance.isNpcInteraction = false;
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Click);
    }

    /// <summary>
    /// 点击Yes
    /// </summary>
    public virtual void OnYesClick()
    {
        this.Close(WindowResult.Yes);
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Win_Yes);
    }

    /// <summary>
    /// 点击No
    /// </summary>
    public virtual void OnOnClick()
    {
        this.Close(WindowResult.NO);
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Win_No);
    }

}
