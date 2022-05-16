using Managers;
using UnityEngine;
using UnityEngine.UI;
public class UISetting : UIWindow
{
    public Image musicOff;
    public Image soundOff;
    public Toggle toggleMusic;
    public Toggle toggleSonud;
    public Slider sliderMusic;
    public Slider sliderSond;
    private float lastPlay = 0;

    void Start()
    {
        this.toggleMusic.isOn = Config.MusicOn;
        this.toggleSonud.isOn = Config.SoundOn;
        this.sliderMusic.value = Config.MusicVolume;
        this.sliderSond.value = Config.SoundVolume;
    }

    public override void OnYesClick()
    {
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Click);
        PlayerPrefs.Save();
        base.OnYesClick();
    }

    /// <summary>
    /// 是否开启音乐
    /// </summary>
    public void MusicToogle(bool no)
    {
        musicOff.enabled = !no;
        Config.MusicOn = no;
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Click);
    }

    /// <summary>
    /// 是否开启音效
    /// </summary>
    public void SoundToogle(bool no)
    {
        soundOff.enabled = !no;
        Config.SoundOn = no;
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Click);
    }

    // <summary>
    // 音乐调节
    // </summary>
    public void MusicVolume(float vol)
    {
        Config.MusicVolume = (int) vol;
        PlaySound();
    }

    /// <summary>
    /// 音效调节
    /// </summary>
    public void SoundVolume(float vol)
    {
        Config.SoundVolume = (int)vol;
        PlaySound();
    }

    /// <summary>
    /// 播放音效
    /// </summary>
    private void PlaySound()
    {
        if (Time.realtimeSinceStartup - lastPlay > 0.1)
        {
            lastPlay = Time.realtimeSinceStartup;
            SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Click);
        }
    }

    /// <summary>
    /// 切换角色
    /// </summary>
    public void ExitToCharSelect()
    {
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Win_Exit);
        SoundManager.Instance.PlayMusic(SoundDefine.Music_Select);
        UnityEngine.SceneManagement.SceneManager.LoadScene("CharSelect");
        Services.UserService.Instance.SendGameLeave();
    } 

    /// <summary>
    /// 退出游戏
    /// </summary>
    public void ExitGame()
    {
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Win_Exit);
        Services.UserService.Instance.SendGameLeave(true);
    }

    public void OnClick()
    { 
        Destroy(this.gameObject);
        InputManager.Instance.isOpenUI = false;
        InputManager.Instance.OpenUI--;
    }
}
