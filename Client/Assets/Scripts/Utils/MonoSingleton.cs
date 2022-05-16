using UnityEngine;

/// <summary>
/// Mono单例模式
/// </summary>
public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    /// <summary>
    /// 全局变量
    /// </summary>
    public bool global = true;
    /// <summary>
    /// 单例对象
    /// </summary>
    static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance =(T)FindObjectOfType<T>();
            }
            return instance;
        }
    }

    void Awake()
    {
        Debug.LogFormat("{0} [{1}] Awake", typeof(T), this.GetInstanceID());
        if (global)
        {
            if (instance != null && instance != this.gameObject.GetComponent<T>()) //判断单例对象是否重复创建
            {
                    Destroy(this.gameObject);
                    return;
            }
            DontDestroyOnLoad(this.gameObject);                     //单例对象不会被销毁
            instance = this.gameObject.GetComponent<T>();    //单例对象设为单例
        }
        this.OnStart();
    }

    protected virtual void OnStart()
    {

    }
}