using Entities;
using Manager;
using UnityEngine;
using Charlotte.Proto;

/// <summary>
/// 实体控制器
/// </summary>
public class EntityController : MonoBehaviour,IEntityNotify
{
    /// <summary>
    /// 状态机
    /// </summary>
    public Animator anim;

    /// <summary>
    /// 刚体
    /// </summary>
    public Rigidbody rb;

    /// <summary>
    /// 当前状态机的状态
    /// </summary>
    private AnimatorStateInfo currentBaseState;

    /// <summary>
    /// 实体对象
    /// </summary>
    public Entity entity;

    /// <summary>
    /// 位置
    /// </summary>
    public UnityEngine.Vector3 position;

    /// <summary>
    /// 方向
    /// </summary>
    public UnityEngine.Vector3 direction;

    /// <summary>
    /// 旋转
    /// </summary>
    Quaternion rotation;

    /// <summary>
    /// 位置
    /// </summary>
    public UnityEngine.Vector3 lastPosition;

    /// <summary>
    /// 旋转
    /// </summary>
    Quaternion lastRotation;

    /// <summary>
    /// 速度
    /// </summary>
    public float speed;

    /// <summary>
    /// 状态机速度
    /// </summary>
    public float animSpeed = 1.5f;

    /// <summary>
    /// 弹跳力
    /// </summary>
    public float jumpPower = 3.0f;

    public bool isPlayer = false;

    void Start () 
    {
        if (entity != null) //判断当前角色是否是玩家
        {
            EntityManager.Instance.RegisterEntityChangeNotify(entity.entityId,this);
            this.UpdateTransform();
        }
        if (!this.isPlayer)
        {
            rb.useGravity = false; //不是玩家角色重力功能关闭
        }
    }

    /// <summary>
    /// 实体Transform更新
    /// </summary>
    void UpdateTransform()
    {   //逻辑坐标系转换世界坐标系
        this.position = GameObjectTool.LogicToWorld(entity.position);
        this.direction = GameObjectTool.LogicToWorld(entity.direction);
        //更新方向数据
        this.rb.MovePosition(this.position);
        this.transform.forward = this.direction;
        this.lastPosition = this.position;
        this.lastRotation = this.rotation;
    }
	
    void OnDestroy()
    {
        if (entity != null)
        {
            Debug.LogFormat("{0} OnDestroy ID:{1} POS:{2} DIR:{3} SPD:{4} ", this.name, entity.entityId, entity.position, entity.direction, entity.speed);
        }
        if (UIWorldElementManager.Instance != null) //判断是否存在
        {
            UIWorldElementManager.Instance.RemoveCharacterNameBar(this.transform);
        }
    }

    void FixedUpdate()
    {
        if (this.entity == null)
        {
            return;
        }
        this.entity.OnUpdate(Time.fixedDeltaTime);
        if (!this.isPlayer)
        {
            this.UpdateTransform();
        }
    }

    /// <summary>
    /// 实体清空
    /// </summary>
    public void OnEntityRemoved()
    {
        if (UIWorldElementManager.Instance != null)
        {
            UIWorldElementManager.Instance.RemoveCharacterNameBar(this.transform);
        }
        Destroy(this.gameObject);
    }

    /// <summary>
    /// 实体事件
    /// </summary>
    public void OnEntityEvent(EntityEvent entityEvent)
    {
        switch(entityEvent)
        {
            case EntityEvent.Idle:
                anim.SetTrigger("Idle");
                anim.SetBool("Rush", false);
                anim.SetBool("Move", false);
                anim.SetBool("Standby", false);
                anim.SetBool("Rush", false);
                anim.SetBool("SkillA", false);
                anim.SetBool("SkillB", false);
                anim.SetBool("SkillC", false);
                anim.SetBool("OpenUI", false);
                break;
            case EntityEvent.MoveFwd:
                anim.SetBool("Standby", false);
                anim.SetBool("Rush", false);
                anim.SetBool("Move", true);
                anim.SetBool("SkillA", false);
                anim.SetBool("SkillB", false);
                anim.SetBool("SkillC", false);
                anim.SetBool("OpenUI", false);
                break;
            case EntityEvent.MoveBack:
                anim.SetBool("Rush", false);
                anim.SetBool("Move", false);
                anim.SetBool("Standby", true);
                anim.SetBool("OpenUI", false);
                break;
            case EntityEvent.Rush:
                anim.SetBool("Rush", true);
                anim.SetBool("SkillA", false);
                anim.SetBool("SkillB", false);
                anim.SetBool("SkillC", false);
                anim.SetBool("OpenUI", false);
                break;
            case EntityEvent.Attack:
                anim.SetBool("Attack",true);
                anim.SetBool("Move", false);
                anim.SetBool("Rush", false);
                anim.SetBool("Standby", false);
                anim.SetBool("OpenUI", false);
                break;
            case EntityEvent.SkillA:
                anim.SetBool("SkillA", true);
                anim.SetBool("Move", false);
                anim.SetBool("Rush", false);
                anim.SetBool("Standby", false);
                anim.SetBool("OpenUI", false);
                break;
            case EntityEvent.SkillB:
                anim.SetBool("SkillB", true);
                anim.SetBool("Move", false);
                anim.SetBool("Rush", false);
                anim.SetBool("Standby", false);
                anim.SetBool("OpenUI", false);
                break;
            case EntityEvent.SkillC:
                anim.SetBool("SkillC", true);
                anim.SetBool("Move", false);
                anim.SetBool("Rush", false);
                anim.SetBool("Standby", false);
                anim.SetBool("OpenUI", false);
                break;
            case EntityEvent.OpenUI:
                anim.SetBool("OpenUI", true);
                anim.SetBool("Move", false);
                anim.SetBool("Rush", false);
                anim.SetBool("Standby", false);
                break;
            case EntityEvent.Death:
                anim.SetBool("Death", true);
                anim.SetBool("Move", false);
                anim.SetBool("Rush", false);
                anim.SetBool("Standby", false);
                anim.SetBool("SkillA", false);
                anim.SetBool("SkillB", false);
                anim.SetBool("SkillC", false);
                anim.SetBool("OpenUI", false);
                break;
        }
    }

    /// <summary>
    /// 实体通知
    /// </summary>
    /// <param name="entity"></param>
    public void OnEntityChanged(Entity entity)
    {
        if(entity !=null)
            Debug.LogFormat("OnEntityChanged :ID:{0} POS:{1} DIR:{2} SPD:{3}", entity.entityId, entity.position, entity.direction, entity.speed);
    }
}
