using Models;
using Services;
using UnityEngine;
using Charlotte.Proto;
using UnityEngine.Events;

public class GlobeScene
{
    public static string nextSceneName;
}

/// <summary>
/// 场景管理器
/// </summary>
public class SceneManager : MonoSingleton<SceneManager>
{
    private byte SceneID = 0;
    private int UserID = 0;
    public bool IsLoad = false;
    private MapCharacterEnterResponse response;
    public UnityAction Load;

    public void LoadScene(string name , MapCharacterEnterResponse response)
    {
        if (this.SceneID != response.mapId[0] || this.UserID != User.Instance.CurrentCharacter.Id || this.IsLoad == true)
        {
            GlobeScene.nextSceneName = name;
            this.response = response;
            UnityEngine.SceneManagement.SceneManager.LoadScene("LoadScene");
            SoundManager.Instance.PlayMusic(SoundDefine.Music_LoadScene);
            this.SceneID = response.mapId[0];
            this.UserID = User.Instance.CurrentCharacter.Id;
            this.IsLoad = true;
            Debug.Log("LoadScene");
        }
        else
        {
            Debug.Log("No LoadScene");
            GlobeScene.nextSceneName = name;
            this.response = response;
            this.SceneID = response.mapId[0];
            this.UserID = User.Instance.CurrentCharacter.Id;
            foreach (var cha in response.Characters)
            {
                if (User.Instance.CurrentCharacter == null || (cha.Type == CharacterType.Player && User.Instance.CurrentCharacter.Id == cha.Id))
                {
                    User.Instance.CurrentCharacter = cha; //当前角色切换地图
                }
                CharacterManager.Instance.AddCharacter(cha); //进入地图的角色交给角色管理器
            }
        }
    }

    /// <summary>
    /// 加载完成
    /// </summary>
    public void LoadEnter()
    {
        foreach (var cha in this.response.Characters)
        {
            if (User.Instance.CurrentCharacter == null || (cha.Type == CharacterType.Player && User.Instance.CurrentCharacter.Id == cha.Id))
            {
                User.Instance.CurrentCharacter = cha; //当前角色切换地图
            }
            CharacterManager.Instance.AddCharacter(cha); //进入地图的角色交给角色管理器
        }
        if (Load != null)
            Load();
    }

    /// <summary>
    /// 场景退出
    /// </summary>
    public void SceneLeave()
    {
        this.SceneID = 0;
        this.UserID = 0;
        this.IsLoad = false;
        this.response = null;
    }

}
