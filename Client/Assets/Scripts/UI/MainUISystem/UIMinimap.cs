using Models;
using Managers;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

/// <summary>
/// UI小地图
/// </summary>
public class UIMinimap : MonoBehaviour
{
    /// <summary>
    /// 地图边界盒
    /// </summary>
    public Collider minimapBoundingBox;

    /// <summary>
    /// 地图
    /// </summary>
	public Image minimap;

    /// <summary>
    /// 图标
    /// </summary>
	public Image arrow;

    /// <summary>
    /// 地图名称
    /// </summary>
    public Text mapName;
    private Transform playerTransform;

    [Header("收缩地图框")]
    public RectTransform Content1;
    [Header("收缩按钮框")]
    public RectTransform Content2;
    [Header("弹出框")]
    public RectTransform Content3;
    [Tooltip("图片")]
    public Image image;
    [Tooltip("启动")]
    private bool isOpen = true;

    void Start ()
    {
        this.UpdateMap();
    }

    /// <summary>
    /// 更新地图
    /// </summary>
    public void UpdateMap()
    {
        this.mapName.text = User.Instance.CurrentMapData.Name;//动态加载地图名
        this.minimap.overrideSprite = MinimapManager.Instance.LoadCurrentMinimap(); //动态加载地图资源

        this.minimap.SetNativeSize();
        this.minimapBoundingBox = MinimapManager.Instance.minimapBoundingBox;
    }

    void Update ()
    {
        if (playerTransform == null)
        {
            if (User.Instance.CurrentCharacterObject != null)
            {
                playerTransform = User.Instance.CurrentCharacterObject.transform; //取得当前对象的位置
            }
        }
        if (minimapBoundingBox == null || playerTransform == null)  //检查组件
            return;
        else
            MinimapManager.Instance.minimap = this;
        //世界坐标转换真实坐标
        float realWidth = minimapBoundingBox.bounds.size.x; //取得边界盒宽度
        float realHeight = minimapBoundingBox.bounds.size.z; //取得边界盒高度
        //角色在小地图中相对坐标转换
        float relaX = playerTransform.position.x - minimapBoundingBox.bounds.min.x;
        float relaY = playerTransform.position.z - minimapBoundingBox.bounds.min.z;
        //相对坐标转换中心点坐标
        float pivotX = relaX / realWidth;
        float pivotY = relaY / realHeight;
        //小地图随中心点移动
        this.minimap.rectTransform.pivot = new Vector2(pivotX, pivotY);
        this.arrow.transform.eulerAngles = new Vector3(0, 0, -playerTransform.eulerAngles.y);
    }

    public void Click()
    {
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Hide);
        if (isOpen)
        {
            isOpen = false;
            //image.sprite = Resources.Load<Sprite>("paimon_off");
            Content3.DOAnchorPosX(84, 0.33f);
            Content1.DOAnchorPos3D( new Vector3(105, -25, 0),0.33f);
            Content2.DOAnchorPosY(65,0.33f);
        }
        else
        {
            isOpen = true;
            //image.sprite = Resources.Load<Sprite>("paimon_on");
            Content3.DOAnchorPosX(145, 0.33f);
            Content1.DOAnchorPos3D(new Vector3(-110, -140, 0), 0.33f);
            Content2.DOAnchorPosY(-50, 0.33f);
        }
    }
}
