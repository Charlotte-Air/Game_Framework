using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 导航寻路
/// </summary>
public class NavPathRender : MonoSingleton<NavPathRender>
{
    /// <summary>
    /// 线渲染器
    /// </summary>
    private LineRenderer pathRenderer;

    /// <summary>
    /// 寻路导航
    /// </summary>
    private NavMeshPath path;

    protected override void OnStart()
    {
        pathRenderer = this.GetComponent<LineRenderer>();
        pathRenderer.enabled = false;
    }

    /// <summary>
    /// 设置导航
    /// </summary>
    /// <param name="path">导航</param>
    /// <param name="target">目标</param>
    public void SetPath(NavMeshPath path,Vector3 target)
    {
        this.path = path;
        if (this.path == null)
        {
            pathRenderer.enabled = false;
            pathRenderer.positionCount = 0;
        }
        else
        {
            pathRenderer.enabled = true;
            pathRenderer.positionCount = path.corners.Length + 1; //生成点数
            pathRenderer.SetPositions(path.corners);
            pathRenderer.SetPosition(pathRenderer.positionCount - 1, target);
            for (int i = 0; i < pathRenderer.positionCount - 1; i++)
            {
                pathRenderer.SetPosition(i, pathRenderer.GetPosition(i) + Vector3.up * 0.2f); //给每个点设置偏移量
            }
        }
    }
}
