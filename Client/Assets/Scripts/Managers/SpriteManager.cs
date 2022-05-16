using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// 精灵管理器
/// </summary>
public class SpriteManager : MonoSingleton<SpriteManager>
{
    /// <summary>
    /// 角色职业图标组
    /// </summary>
    [PreviewField]
    public Sprite[] classIcons;
}