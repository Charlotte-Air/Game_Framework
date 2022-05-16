using UnityEngine;

/// <summary>
/// UI����Ԫ��
/// </summary>
public class UIWorldElement : MonoBehaviour
{
    /// <summary>
    /// Ԫ�ؽڵ�
    /// </summary>
    public Transform owner;

    /// <summary>
    /// Ԫ�ؾ���ֵ
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
