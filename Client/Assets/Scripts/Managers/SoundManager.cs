using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// 声音管理器
/// </summary>
public class SoundManager : MonoSingleton<SoundManager>
{
    /// <summary>
    /// 音乐
    /// </summary>
    public AudioSource musicAudioSource;

    /// <summary>
    /// 音效
    /// </summary>
    public AudioSource SoundAudioSource;

    /// <summary>
    /// 混音器
    /// </summary>
    public AudioMixer audioMixer;

    /// <summary>
    /// 音乐路程
    /// </summary>
    private const string MusicPath = "Music/";

    /// <summary>
    /// 音效路径
    /// </summary>
    private const string SoundPath = "Sound/";

    private bool musicOn;
    /// <summary>
    /// 音乐开关
    /// </summary>
    public bool MusicOn
    {
        get { return musicOn; }
        set
        {
            musicOn = value;
            this.MusicMute(!musicOn);
        }
    }

    private bool soundOn;
    /// <summary>
    /// 音效开关
    /// </summary>
    public bool SoundOn
    {
        get { return soundOn; }
        set
        {
            soundOn = value;
            this.SoundMute(!soundOn);
        }
    }

    /// <summary>
    /// 音乐值
    /// </summary>
    private int musicVolume;
    public int MusicVolume
    {
        get { return musicVolume; }
        set
        {
            musicVolume = value;
            if(MusicOn) this.SetVolume("MusicVolume", musicVolume);
        }
    }

    /// <summary>
    /// 音效值
    /// </summary>
    private int soundVolume;
    public int SoundVolume
    {
        get { return soundVolume; }
        set
        {
            soundVolume = value;
            if (soundOn) this.SetVolume("SoundVolume", soundVolume);
        }
    }

    protected override void OnStart()
    {
        this.MusicVolume = Config.MusicVolume;
        this.SoundVolume = Config.SoundVolume;
        this.MusicOn = Config.MusicOn;
        this.SoundOn = Config.SoundOn;
    }

    /// <summary>
    /// 音乐静音
    /// </summary>
    /// <param name="mute"></param>
    public void MusicMute(bool mute)
    {
        this.SetVolume("MusicVolume", mute ? 0 : musicVolume);
    }

    /// <summary>
    /// 音效静音
    /// </summary>
    /// <param name="mute"></param>
    public void SoundMute(bool mute)
    {
        this.SetVolume("SoundVolume", mute ? 0 : SoundVolume);
    }

    /// <summary>
    ///  设置值
    /// </summary>
    /// <param name="name"></param>
    /// <param name="vlaue"></param>
    private void SetVolume(string name, int vlaue)
    {
        float volme = vlaue * 0.5f - 50f;
        this.audioMixer.SetFloat(name, volme);
    }

    /// <summary>
    /// 播放音乐
    /// </summary>
    /// <param name="name">名称</param>
    public void PlayMusic(string name)
    {
        AudioClip clip = Resloader.Load<AudioClip>(MusicPath+name);
        if (clip == null)
        {
            Debug.LogWarningFormat("PlayMusic-> {0} Not Existed", name);
            return;
        }
        if (musicAudioSource.isPlaying)
        {
            musicAudioSource.Stop();
        }

        musicAudioSource.clip = clip;
        musicAudioSource.Play();
    }

    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="name">名称</param>
    public void PlaySound(string name)
    {
        AudioClip clip = Resloader.Load<AudioClip>(SoundPath+name);
        if (clip == null)
        {
            Debug.LogWarningFormat("PlaySound-> {0} Not Existed", name);
            return;
        }
        SoundAudioSource.PlayOneShot(clip);
    }

    protected void PlayClioOnAudioSource(AudioSource source, AudioClip clip, bool isLoop)
    {

    }
}
