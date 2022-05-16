using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// UI窗口拖拽
/// </summary>
public class UIDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    /// <summary>
    /// 精准拖拽为true ,鼠标一直在UI中心可以为false
    /// </summary>
    public bool isPrecision = true;
    /// <summary>
    /// 存储图片中心点与鼠标点击点的偏移量
    /// </summary>
    private Vector3 offect;
    /// <summary>
    /// 存储当前拖拽图片的RectTransform组件
    /// </summary>
    private RectTransform m_rt;
    void Start()
    {
        m_rt = this.transform.GetComponent<RectTransform>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isPrecision) //如果是精确拖拽则进行计算偏移量操作
        {
            Vector3 tWorldPos;  //存储点击时的鼠标坐标
            RectTransformUtility.ScreenPointToWorldPointInRectangle(m_rt, eventData.position, eventData.pressEventCamera, out tWorldPos); //UI屏幕坐标转换为世界坐标
            offect = transform.position - tWorldPos; //计算偏移量
        }
        else 
        {
            offect = Vector3.zero; //否则,默认偏移量为0
        }
        //m_rt.position = Input.mousePosition + offect;
        SetDraggedPosition(eventData);
    }

    /// <summary>
    /// 拖拽过程中
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        //m_rt.position = Input.mousePosition + offect;
        SetDraggedPosition(eventData);
    }

    /// <summary>
    /// 结束拖拽
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {
        //m_rt.position = Input.mousePosition + offect;
        SetDraggedPosition(eventData);
    }
    private void SetDraggedPosition(PointerEventData eventData)
    {
        Vector3 globalMousePos; //存储当前鼠标所在位置
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_rt, eventData.position, eventData.pressEventCamera, out globalMousePos)) //UI屏幕坐标转换为世界坐标
        {
            m_rt.position = globalMousePos + offect; //设置位置及偏移量
        }
    }

}