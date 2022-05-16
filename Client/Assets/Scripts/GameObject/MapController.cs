using Managers;
using UnityEngine;

/// <summary>
/// 地图控制器
/// </summary>
public class MapController : MonoBehaviour
{
	/// <summary>
	/// 边界盒
	/// </summary>
    public Collider minimapBoundingBox;

    void Awake ()
    {
        MinimapManager.Instance.UpdateMinimap(minimapBoundingBox);
	}
}
