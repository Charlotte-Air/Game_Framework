using UnityEngine;
class MessageBox
{
    static Object cacheObject = null;

    public static UIMessageBox Show(string message, string title="", MessageBoxType type = MessageBoxType.Information, string btnOK = "", string btnCancel = "")
    {
        if(cacheObject==null)
        {
            cacheObject = Resloader.Load<Object>("UI/UIMessageBox");
        }

        GameObject go = (GameObject)GameObject.Instantiate(cacheObject);
        UIMessageBox msgbox = go.GetComponent<UIMessageBox>();
        msgbox.Init(title, message, type, btnOK, btnCancel);
        return msgbox;
    }
}

public enum MessageBoxType
{
    /// <summary>
    /// 带有OK按钮的信息对话框
    /// </summary>
    Information = 1,

    /// <summary>
    /// 用OK和Cancel按钮确认对话框
    /// </summary>
    Confirm = 2,

    /// <summary>
    /// 带有OK按钮的错误对话框
    /// </summary>
    Error = 3
}