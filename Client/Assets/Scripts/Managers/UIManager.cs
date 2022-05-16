using System;
using Managers;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 主UI管理器
/// </summary>
public class UIManager : Singleton<UIManager>
{
    /// <summary>
    /// 元素节点
    /// </summary>
	class UIElement
    {
        /// <summary>
        /// 资源路径
        /// </summary>
		public string Resources;
        /// <summary>
        /// 隐藏开关
        /// </summary>
		public bool Cache;
        /// <summary>
        /// 实例对象
        /// </summary>
		public GameObject Instance;
    }
    /// <summary>
    /// 管理器集合
    /// </summary>
    private Dictionary<Type, UIElement> UIResources = new Dictionary<Type, UIElement>();

    public UIManager()
    {
        this.UIResources.Add(typeof(UISetting), new UIElement() { Resources = "UI/UISetSystem", Cache = true });
        this.UIResources.Add(typeof(UIBag), new UIElement() { Resources = "UI/UIBagSystem", Cache = false });
        this.UIResources.Add(typeof(UIShop), new UIElement() { Resources = "UI/UIShopSystem", Cache = false });
        this.UIResources.Add(typeof(UICharEquip), new UIElement() {Resources = "UI/UIEquipSystem", Cache = false});
        this.UIResources.Add(typeof(UIQuestSystem), new UIElement() { Resources = "UI/UIQuestSystem", Cache = false });
        this.UIResources.Add(typeof(UIQuestDialog), new UIElement() { Resources = "UI/UIQuestDialog", Cache = false });
        this.UIResources.Add(typeof(UIFriends), new UIElement() { Resources = "UI/UIFriendSystem", Cache = false });
        this.UIResources.Add(typeof(UIGuild), new UIElement() { Resources = "UI/Guild/UIGuildSystem", Cache = false });
        this.UIResources.Add(typeof(UIGuildList), new UIElement() { Resources = "UI/Guild/UIGuildList", Cache = false });
        this.UIResources.Add(typeof(UIGuildPopNoGuild), new UIElement() { Resources = "UI/Guild/UIGuildPopNoGuild", Cache = false });
        this.UIResources.Add(typeof(UIGuildPopCreate), new UIElement() { Resources = "UI/Guild/UIGuildPopCreate", Cache = false });
        this.UIResources.Add(typeof(UIGuildApplyList), new UIElement() { Resources = "UI/Guild/UIGuildApplyList", Cache = false });
        this.UIResources.Add(typeof(UIPopCharMenu), new UIElement() { Resources = "UI/UIPopChatMenu", Cache = false });
        this.UIResources.Add(typeof(UIPopItemInfo), new UIElement() { Resources = "UI/UIPopItemInfo", Cache = false });
    }

    ~UIManager()
    {

    }

    /// <summary>
    /// 显示
    /// </summary>
    public T Show<T>()
    {
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Win_Open);  //声音开启
        InputManager.Instance.isOpenUI = true;
        InputManager.Instance.OpenUI++;
        Type type = typeof(T); //定义类型
        if (this.UIResources.ContainsKey(type)) //判断集合中是否存在
        {
            UIElement info = this.UIResources[type];
            if (info.Instance != null) //判断实例中是否存在
            {
                info.Instance.SetActive(true);
            }
            else
            {
                UnityEngine.Object prefab = Resources.Load(info.Resources); //加载Prefab
                if (prefab == null)
                {
                    return default(T);
                }
                info.Instance = (GameObject) GameObject.Instantiate(prefab); //实例化
            }
            return info.Instance.GetComponent<T>();
        }
        return default(T);
    }

    /// <summary>
    /// 关闭
    /// </summary>
    public void Close(Type type)
    {
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Win_Close);  //声音关闭
        InputManager.Instance.isOpenUI = false;
        InputManager.Instance.OpenUI--;
        if (this.UIResources.ContainsKey(type)) //判断UI是否存在
        {
            UIElement info = this.UIResources[type];
            if (info.Cache) //是否启用隐藏
            {
                info.Instance.SetActive(false);  //隐藏
            }
            else
            {
                GameObject.Destroy(info.Instance);  //销毁
                info.Instance = null;
            }
        }
    }

    /// <summary>
    /// 清空
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void Close<T>()
    {
        this.Close(typeof(T));
    }

}
