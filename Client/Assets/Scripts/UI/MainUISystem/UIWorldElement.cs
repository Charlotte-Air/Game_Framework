using UnityEngine;

/// <summary>
/// UI世界元素
/// </summary>
public class UIWorldElement : MonoBehaviour
{
    /// <summary>
    /// 元素节点
    /// </summary>
    public Transform owner;

    /// <summary>
    /// 元素距离值
    /// </summary>
    public float height;

    void Update()
    {
        if (owner != null)
        {
            this.transform.position = owner.position + Vector3.up * height;
        }
        //if (MainPlayerCamera.Instance.player != null)
        //{
        //    this.transform.forward = MainPlayerCamera.Instance.player.transform.forward;
        //}
    }
}
