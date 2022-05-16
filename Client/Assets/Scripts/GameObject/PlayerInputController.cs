using Entities;
using Services;
using Managers;
using UnityEngine;
using UnityEngine.AI;
using Charlotte.Proto;
using System.Collections;

/// <summary>
/// 玩家输入控制器
/// </summary>
public class PlayerInputController : MonoBehaviour
{
    /// <summary>
    /// 玩家刚体
    /// </summary>
    public Rigidbody rb;

    /// <summary>
    /// 玩家角色
    /// </summary>
    public Character character;

    /// <summary>
    /// 旋转速度
    /// </summary>
    public float rotateSpeed = 2.0f;

    /// <summary>
    /// 旋转角度
    /// </summary>
    public float turnAngle = 10;

    /// <summary>
    /// 玩家速度
    /// </summary>
    public int speed;

    /// <summary>
    /// 实体控制器
    /// </summary>
    public EntityController entityController;

    CharacterState state;
    public NavMeshAgent agent;
    private Vector3 lastPos;
    public bool onAir = false;
    private bool autoNav = false;

    void Start ()
    {
        state = CharacterState.Idle;
        if(this.character == null)
        {
            DataManager.Instance.Load();
            NCharacterInfo cinfo = new NCharacterInfo();
            cinfo.Id = 1;
            cinfo.Name = "Test";
            cinfo.EntityId = 1;
            cinfo.Entity = new NEntity();
            cinfo.Entity.Position = new NVector3();
            cinfo.Entity.Direction = new NVector3();
            cinfo.Entity.Direction.X = 0;
            cinfo.Entity.Direction.Y = 0;
            cinfo.Entity.Direction.Z = 0;
            this.character = new Character(cinfo);
            if (entityController != null) entityController.entity = this.character;
        }
        if (agent == null)
        {
            agent = this.gameObject.AddComponent<NavMeshAgent>();
            agent.stoppingDistance = 0.3f;
        }
    }

    /// <summary>
    /// 开始导航
    /// </summary>
    public void StartNav(Vector3 target)
    {
        InputManager.Instance.isNav = true;
        InputManager.Instance.isNpcInteraction = false;
        StartCoroutine(BeginNav(target));
    }
    IEnumerator BeginNav(Vector3 target)
    {
        agent.SetDestination(target);
        yield return null;
        autoNav = true;
        if (state != CharacterState.Move)
        {
            state = CharacterState.Move;
            this.character.MoveForward();
            this.SendEntityEvent(EntityEvent.MoveFwd);
            agent.speed = this.character.speed / 100f;
        }
    }

    /// <summary>
    /// 停止导航
    /// </summary>
    public void StopNav()
    {
        autoNav = false;
        InputManager.Instance.isNav = false;
        agent.ResetPath();
        if (state != CharacterState.Idle)
        {
            state = CharacterState.Idle;
            this.rb.velocity = Vector3.zero;
            this.character.Stop();
            this.SendEntityEvent(EntityEvent.Idle);
        }
        NavPathRender.Instance.SetPath(null, Vector3.zero);
    }

    /// <summary>
    /// 导航移动中
    /// </summary>
    public void NavMove()
    {
        if (agent.pathPending) //判断导航是否完成
            return;
        if (agent.pathStatus == NavMeshPathStatus.PathInvalid)
        {
            StopNav();
            return;
        }
        if (agent.pathStatus != NavMeshPathStatus.PathComplete) //判断导航是否完成
            return;
        if (Mathf.Abs(Input.GetAxis("Vertical")) > 0.1 || Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1) //判断玩家是否控制
        {
            StopNav();
            return;
        }
        NavPathRender.Instance.SetPath(agent.path, agent.destination);
        if (agent.isStopped || agent.remainingDistance < 1.5) //判断与目标距离
        {
            StopNav();
            InputManager.Instance.isNpcInteraction = true;
            return;
        }
    }

    void FixedUpdate()
    {
        if (character == null) return;  //判断角色是否有实体
        if (autoNav)
        {
            NavMove();
            return;
        }
        if (InputManager.Instance.isOpenUI && InputManager.Instance.isNav == false)
        {
            this.character.Stop();
            this.rb.velocity = Vector3.zero;
            entityController.OnEntityEvent(EntityEvent.Idle);
            return;
        }
        //if (!InputManager.Instance.isNav)
        //{
        //    this.character.Stop();
        //    this.rb.velocity = Vector3.zero;
        //    entityController.OnEntityEvent(EntityEvent.OpenUI);
        //    return;
        //}
        if (InputManager.Instance.IsInputMode)
        {
            this.character.Stop();
            this.rb.velocity = Vector3.zero;
            entityController.OnEntityEvent(EntityEvent.Idle);
            return;
        }
        float v = Input.GetAxis("Vertical"); //玩家移动
        if (v > 0.01)
        {
            if (state != CharacterState.Move) //玩家当前状态不是移动状态就设置为Move状态，并且向前移动
            {
                state = CharacterState.Move;  //状态机变化
                this.character.MoveForward();                               //玩家移动
                this.SendEntityEvent(EntityEvent.MoveFwd);        //事件通知
            }
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                this.character.MoveRush();                                      //玩家移动
                this.SendEntityEvent(EntityEvent.Rush);                 //事件通知
            }
            this.rb.velocity = this.rb.velocity.y * Vector3.up + GameObjectTool.LogicToWorld(character.direction) * (this.character.speed + 9.81f) / 100f; //速度处理
        }
        else if (v < -0.01)
        {
            if (state != CharacterState.Move) //玩家当前状态不是移动状态就设置为Move状态，并且向后移动
            {
                state = CharacterState.Move;
                this.character.MoveBack();
                this.SendEntityEvent(EntityEvent.MoveBack);
            }
            this.rb.velocity = this.rb.velocity.y * Vector3.up + GameObjectTool.LogicToWorld(character.direction) * (this.character.speed + 9.81f) / 100f;
        }
        else
        {
            if (state != CharacterState.Idle) //玩家当前状态不是停止就设置为Idle状态，并且停止
            {
                state = CharacterState.Idle;
                this.rb.velocity = Vector3.zero;
                this.character.Stop();
                this.SendEntityEvent(EntityEvent.Idle);
            }
        }
        float h = Input.GetAxis("Horizontal"); //玩家旋转
        if (h < -0.1 || h > 0.1)
        {
            this.transform.Rotate(0, h * rotateSpeed, 0);
            Vector3 dirVector3 = GameObjectTool.LogicToWorld(character.direction);
            Quaternion rotQuaternion = new Quaternion();
            rotQuaternion.SetFromToRotation(dirVector3, this.transform.forward);
            if (rotQuaternion.eulerAngles.y > this.turnAngle && rotQuaternion.eulerAngles.y < (360 - this.turnAngle))
            {
                character.SetDirection(GameObjectTool.WorldToLogic(this.transform.forward));
                rb.transform.forward = this.transform.forward;
                this.SendEntityEvent(EntityEvent.None);
            }
        }
        if (Input.GetKey(KeyCode.Z))  //玩家技能1
        {
            if (state != CharacterState.Idle) //玩家当前状态不是停止就设置为Idle状态，并且停止
            {
                this.rb.velocity = Vector3.zero;
                this.character.Stop();
                entityController.OnEntityEvent(EntityEvent.Idle);
                entityController.OnEntityEvent(EntityEvent.SkillB);
                this.SendEntityEvent(EntityEvent.SkillA);  //事件通知
            }
        }
        if (Input.GetKey(KeyCode.X))  //玩家技能2
        {
            if (state != CharacterState.Idle) //玩家当前状态不是停止就设置为Idle状态，并且停止
            {
                this.rb.velocity = Vector3.zero;
                this.character.Stop();
                entityController.OnEntityEvent(EntityEvent.Idle);
                entityController.OnEntityEvent(EntityEvent.SkillB);
                this.SendEntityEvent(EntityEvent.SkillB);  //事件通知
            }
        }
        if (Input.GetKey(KeyCode.C))  //玩家技能3
        {
            if (state != CharacterState.Idle) //玩家当前状态不是停止就设置为Idle状态，并且停止
            {
                this.rb.velocity = Vector3.zero;
                this.character.Stop();
                entityController.OnEntityEvent(EntityEvent.Idle);
                entityController.OnEntityEvent(EntityEvent.SkillC);
                this.SendEntityEvent(EntityEvent.SkillC);  //事件通知
            }
        }

        //---------------------------------------------------------------------------------------------------------------------------------------

        Vector3 offset = this.rb.transform.position - lastPos;
        this.speed = (int)(offset.magnitude * 100f / Time.deltaTime);
        this.lastPos = this.rb.transform.position;
        //Debug.LogFormat("LateUpdate velocity {0} : {1}", this.rb.velocity.magnitude, this.speed);
        Vector3Int goLogicPos = GameObjectTool.WorldToLogic(this.rb.transform.position);
        float logicOffset = (goLogicPos - this.character.position).magnitude;
        if (logicOffset > 100)
        {
            this.character.SetPosition(GameObjectTool.WorldToLogic(this.rb.transform.position));
            this.SendEntityEvent(EntityEvent.None);
        }
        this.transform.position = this.rb.transform.position;

        Vector3 dir = GameObjectTool.LogicToWorld(character.direction);
        Quaternion rot = new Quaternion();
        rot.SetFromToRotation(dir, this.transform.forward);
        if (rot.eulerAngles.y > this.turnAngle && rot.eulerAngles.y < (360 - this.turnAngle))
        {
            character.SetDirection(GameObjectTool.WorldToLogic(this.transform.forward));
            this.SendEntityEvent(EntityEvent.None);
        }
        //Debug.LogFormat("velocity {0}", this.rb.velocity.magnitude)s
    }

    //void LateUpdate()   
    //{
    //    if (this.character == null) return;

    //    Vector3 offset = this.rb.transform.position - lastPos;
    //    this.speed = (int)(offset.magnitude * 100f / Time.deltaTime);
    //    //Debug.LogFormat("LateUpdate velocity {0} : {1}", this.rb.velocity.magnitude, this.speed);
    //    this.lastPos = this.rb.transform.position;

    //    Vector3Int goLogicPos = GameObjectTool.WorldToLogic(this.rb.transform.position);
    //    float logicOffset = (goLogicPos - this.character.position).magnitude;
    //    if (logicOffset > 100)
    //    {
    //        this.character.SetPosition(GameObjectTool.WorldToLogic(this.rb.transform.position));
    //        this.SendEntityEvent(EntityEvent.None);
    //    }
    //    this.transform.position = this.rb.transform.position;

    //    Vector3 dir = GameObjectTool.LogicToWorld(character.direction);
    //    Quaternion rot = new Quaternion();
    //    rot.SetFromToRotation(dir,this.transform.forward);

    //    if (rot.eulerAngles.y > this.turnAngle && rot.eulerAngles.y < (360 - this.turnAngle))
    //    {
    //        character.SetDirection(GameObjectTool.WorldToLogic(this.transform.forward));
    //        this.SendEntityEvent(EntityEvent.None);
    //    }
    //}

    /// <summary>
    /// 发送实体事件
    /// </summary>
    public void SendEntityEvent(EntityEvent entityEvent)
    {
        if (entityController != null)
            entityController.OnEntityEvent(entityEvent);
        MapService.Instance.SendMapEntitySync(entityEvent,this.character.EntityData);
    }
}
