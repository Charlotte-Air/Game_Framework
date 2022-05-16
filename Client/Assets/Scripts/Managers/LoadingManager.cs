using Services;
using Managers;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// 加载管理器
/// </summary>
public class LoadingManager : MonoBehaviour
{
    [Header("LoingBar")]
    [Tooltip("提示")]
    public GameObject UITips;
    [Tooltip("Loading界面")]
    public GameObject UILoading;
    [Tooltip("用户登入界面")]
    public GameObject UILogin;
    [Tooltip("进度条A")]
    public Slider progressBarA;
    [Tooltip("进度条B")]
    public Slider progressBarB;
    [Tooltip("进度条字体")]
    public Text progressNumber;
    
    IEnumerator Start()
    {
        log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo("log4net.xml"));
        UnityLogger.Init();
        Common.Log.Init("Unity");
        Common.Log.Info("LoadingManager Start");
        UITips.SetActive(true);
        UILoading.SetActive(false);
        UILogin.SetActive(false);
        yield return new WaitForSeconds(2f);
        UILoading.SetActive(true);
        yield return new WaitForSeconds(1f);
        UITips.SetActive(false);
        yield return DataManager.Instance.LoadData();
        //初始化基本服务
        UserService.Instance.Init();
        MapService.Instance.Init();
        StateService.Instance.Init();
        FriendService.Instance.Init();
        TeamService.Instance.Init();
        GuildService.Instance.Init();
        ChatService.Instance.Init();
        ShopManager.Instance.Init();
        SoundManager.Instance.PlayMusic(SoundDefine.Music_Login);
        this.progressNumber.text = "0%";
        for (float i = 1; i < 100;)
        {
            i += Random.Range(0.1f, 1.5f);
            this.progressNumber.text = (int) i + "%";
            progressBarA.value = i;
            progressBarB.value = i;
            yield return new WaitForEndOfFrame();
        }
        UILoading.SetActive(false);
        UILogin.SetActive(true);
        yield return null;
    }
}
