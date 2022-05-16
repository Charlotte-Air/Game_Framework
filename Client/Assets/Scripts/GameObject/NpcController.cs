using Models;
using Managers;
using UnityEngine;
using Common.Data;
using Charlotte.Proto;
using System.Collections;


/// <summary>
/// NPC控制器
/// </summary>
public class NpcController : MonoBehaviour
{
    /// <summary>
    /// NPC状态机
    /// </summary>
    Animator anim;

    /// <summary>
    /// NPC表缓存
    /// </summary>
    NpcDefine npc;

    /// <summary>
    /// NPC任务状态
    /// </summary>
    NpcQuestStatus queststatus;

    /// <summary>
    /// NPC ID
    /// </summary>
    public int npcID;

    Color orignCollider;

    /// <summary>
    /// 交互中
    /// </summary>
    private bool inInteractive = false;

    new SkinnedMeshRenderer renderer;

    void Start ()
    {
        renderer = this.gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        anim = this.gameObject.GetComponent<Animator>();
        orignCollider = renderer.sharedMaterial.color;
        npc = NPCManager.Instance.GetNpcDefine(this.npcID);
        NPCManager.Instance.UpdateNpcPosition(this.npcID,this.transform.position);
        this.StartCoroutine(Actions());
        RefreshNpcStatus();
        QuestManager.Instance.onQuestStatusChanged += OnQuestStatusChanged;
    }

    /// <summary>
    /// 点击交互事件回调
    /// </summary>
    /// <param name="result"></param>
    public void OnClickNpcis(Result result)
    {
        if (result == Result.Success)
            this.Interactive();
    }

    /// <summary>
    /// 任务进度状态
    /// </summary>
    /// <param name="quest"></param>
    void OnQuestStatusChanged(Quest quest)
    {
        this.RefreshNpcStatus();   
    }

    /// <summary>
    /// 更新NPC任务状态
    /// </summary>
    void RefreshNpcStatus()
    {
        queststatus = QuestManager.Instance.GetQuestStatusByNpc(this.npcID);
        UIWorldElementManager.Instance.AddNpcQuestStatus(this.transform,queststatus);
    }

    void OnDestroy()
    {
        QuestManager.Instance.onQuestStatusChanged -= OnQuestStatusChanged;
        if (UIWorldElementManager.Instance != null)
            UIWorldElementManager.Instance.RemoveNpcQuestStatus(this.transform);
    }

    /// <summary>
    /// 活动时
    /// </summary>
    IEnumerator Actions()
    {
        while (true)
        {
            if (inInteractive)
                yield return new WaitForSeconds(2f);
            else
            {
                yield return new WaitForSeconds(Random.Range(5f, 10f));
                this.Relax();
            }
        }
    }

    /// <summary>
    /// 空闲时
    /// </summary>
    void Relax()
    {
        if (this.npcID != 3)
        {
            anim.SetTrigger("Relax");
        }
    }

    /// <summary>
    /// 交互时
    /// </summary>
    IEnumerator DoInteractive()
    {
       // yield return FaceToPlayer();
        while (true)
        {
            if (InputManager.Instance.isNpcInteraction)
                break;
            else
                yield return new WaitForSeconds(0.05f);
        }
        if (NPCManager.Instance.Interactive(npc)) //发送事件
        {
            if (this.npcID != 3 || this.npcID != 4)
            {
                anim.SetTrigger("Talk");
            }
        }
        yield return new WaitForSeconds(1f);
        inInteractive = false;
    }

    /// <summary>
    /// 背对时
    /// </summary>
    //IEnumerator FaceToPlayer()
    //{
    //    Vector3 faceTo = (User.Instance.CurrentCharacterObject.transform.position - this.transform.position).normalized; //目标位置-自己的位置
    //    while (Mathf.Abs(Vector3.Angle(this.gameObject.transform.forward,faceTo))>5) //碰到当前的角度和目标的角度
    //    {
    //        this.gameObject.transform.forward= Vector3.Lerp(this.gameObject.transform.forward,faceTo,Time.deltaTime*5f);
    //        yield return null;
    //    }
    //}


    /// <summary>
    /// 交互
    /// </summary>
    void Interactive()
    {
        if (!inInteractive)
        {
            inInteractive = true;
            StartCoroutine(DoInteractive());
        }
    }

    /// <summary>
    /// 点击交互
    /// </summary>
    void OnMouseDown()
    {
        User.Instance.CurrentCharacterObject.StartNav(this.transform.position);
        Interactive();
    }
    private void OnMouseOver()
    {
        Highlight(true);
    }
    private void OnMouseEnter()
    {
        Highlight(true);
    }
    private void OnMouseExit()
    {
        Highlight(false);
    }

    /// <summary>
    /// 高亮
    /// </summary>
    /// <param name="highlight">是否启用</param>
    void Highlight(bool highlight)
    {
        if (highlight)
        {
            if (renderer.sharedMaterial.color != Color.white)
                renderer.sharedMaterial.color = Color.white;
        }
        else
        {
            if (renderer.sharedMaterial.color != orignCollider)
                renderer.sharedMaterial.color = orignCollider;
        }
    }
}
