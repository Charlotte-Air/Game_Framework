using Managers;
using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// 主玩家摄像机
/// </summary>
public class MainPlayerCamera : MonoSingleton<MainPlayerCamera>
{
    [BoxGroup("游戏对象")]
    public GameObject player;

    [BoxGroup("主摄像机")]
    new public Camera camera;
    [BoxGroup("X轴值")]
    public float CamerX = 0.0f;
    [BoxGroup("Y轴值")]
    public float CamerY = 0.0f;

    [BoxGroup("X轴旋转速度")]
    public float X_Speed = 1.5f;
    [BoxGroup("X轴最小角度")]
    public float X_MinLimity = -360;
    [BoxGroup("X轴最大角度")]
    public float X_MaxLimity = 360;


    [BoxGroup("Y轴旋转速度")]
    public float Y_Speed = 1.5f;
    [BoxGroup("Y轴最小角度")]
    public float Y_MinLimity = -20;
    [BoxGroup("Y轴最大角度")]
    public float Y_MaxLimity = 20;

    [BoxGroup("摄像机距离")]
    public float CameMouse = 1.1f;
    [BoxGroup("摄像机速度")]
    public float MouseSpeed = 1f;
    [BoxGroup("视角最小值")]
    public float MouseMin = 1.1f;
    [BoxGroup("视角最大值")]
    public float MouseMax = 3f;

    [BoxGroup("插值启用")]
    public bool isNeedDamping=false;
    [BoxGroup("速度值")]
    public float CameraSpeed = 2.5f;

    private void LateUpdate()
    {
        if (player == null)
            return;
        if (InputManager.Instance.isOpenUI && InputManager.Instance.isNav == false)
            return;
        if (InputManager.Instance.isOpenUI == false && InputManager.Instance.OpenUI > 0)
            return;
        if (InputManager.Instance.lockTheScreen)
            return;
        if (InputManager.Instance.IsInputMode)
            return;

        CamerX += Input.GetAxis("Mouse X") * X_Speed;
        CamerY -= Input.GetAxis("Mouse Y") * Y_Speed;
        CameMouse -= Input.GetAxis("Mouse ScrollWheel") * MouseSpeed;
        CamerX = ClampAngle(CamerX, X_MinLimity, X_MaxLimity);
        CamerY = ClampAngle(CamerY, Y_MinLimity, Y_MaxLimity);
        CameMouse = ClampAngle(CameMouse, MouseMin, MouseMax);

        Quaternion Rotation = Quaternion.Euler(CamerY, CamerX, 0);
        Vector3 Position = Rotation * new Vector3(0.0f, 2.0f, -CameMouse) + this.player.transform.position;
        if (isNeedDamping)
        {
            transform.rotation=Quaternion.Slerp(transform.rotation,Rotation,Time.deltaTime*CameraSpeed);    //球形插值
            transform.position = Vector3.Lerp(transform.position, Position, Time.deltaTime * CameraSpeed); //线性插值
        }
        else
        {
            this.transform.rotation = Rotation;
            this.transform.position = Position;
        }
    }

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360) angle += 360;
        if (angle > 360) angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }
}
