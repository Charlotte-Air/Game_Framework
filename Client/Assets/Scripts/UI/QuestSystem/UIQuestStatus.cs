using Managers;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 任务状态
/// </summary>
public class UIQuestStatus : MonoBehaviour
{
    /// <summary>
    /// 状态图标
    /// </summary>
    public Image[] statusImages;
    private NpcQuestStatus questStatus;

    /// <summary>
    /// 设置任务状态
    /// </summary>
    public void SetQuestStatus(NpcQuestStatus status)
    {
        this.questStatus = status;
        for (int i = 0; i < 4; i++)
        {
            if (this.statusImages[i] != null)
                this.statusImages[i].gameObject.SetActive(i == (int) status);
        }
    }
}
