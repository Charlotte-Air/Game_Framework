using Models;
using Services;
using UnityEngine;
using Common.Data;

/// <summary>
/// 传送点对象
/// </summary>
public class TeleporterObject : MonoBehaviour
{
    /// <summary>
    /// 传送点索引值
    /// </summary>
    public int ID;
    Mesh mesh = null;

    /// <summary>
    /// 触碰传送点
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter(Collider other)
    {
        PlayerInputController playerController = other.GetComponent<PlayerInputController>();
        if (playerController != null)
        {
            TeleporterDefine td = DataManager.Instance.Teleporters[this.ID];
            if (td == null)
            {
                Debug.LogFormat("TeleporterObject: Character [ {0} ] Enter Teleporter [ {1} ] , But TeleporterDefine not existed", playerController.character.Info.Name, this.ID);
                return;
            }
            if (other.gameObject.name != User.Instance.CurrentCharacterObject.name)
                return;
            if (td.LinkTo > 0)
            {
                if (DataManager.Instance.Teleporters.ContainsKey(td.LinkTo))//检测地图ID的有效性
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene("LoadScene"); //loading界面场景
                    MapService.Instance.SendMapTeleport(this.ID);
                }
                else
                    Debug.LogFormat("Teleporter ID: { 0 } LinkID {1} error!", td.ID, td.LinkTo);
            }
        }
    }


#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (this.mesh != null)
        {
            Gizmos.DrawWireMesh(this.mesh, this.transform.position + Vector3.up * this.transform.localScale.y*.5f,this.transform.rotation,this.transform.localScale);
        }
        UnityEditor.Handles.color = Color.red;
        UnityEditor.Handles.ArrowHandleCap(0, this.transform.position, this.transform.rotation, 1f, EventType.Repaint);
    }
#endif
}
