using Models;
using Managers;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

/// <summary>
/// UIС��ͼ
/// </summary>
public class UIMinimap : MonoBehaviour
{
    /// <summary>
    /// ��ͼ�߽��
    /// </summary>
    public Collider minimapBoundingBox;

    /// <summary>
    /// ��ͼ
    /// </summary>
	public Image minimap;

    /// <summary>
    /// ͼ��
    /// </summary>
	public Image arrow;

    /// <summary>
    /// ��ͼ����
    /// </summary>
    public Text mapName;
    private Transform playerTransform;

    [Header("������ͼ��")]
    public RectTransform Content1;
    [Header("������ť��")]
    public RectTransform Content2;
    [Header("������")]
    public RectTransform Content3;
    [Tooltip("ͼƬ")]
    public Image image;
    [Tooltip("����")]
    private bool isOpen = true;

    void Start ()
    {
        this.UpdateMap();
    }

    /// <summary>
    /// ���µ�ͼ
    /// </summary>
    public void UpdateMap()
    {
        this.mapName.text = User.Instance.CurrentMapData.Name;//��̬���ص�ͼ��
        this.minimap.overrideSprite = MinimapManager.Instance.LoadCurrentMinimap(); //��̬���ص�ͼ��Դ

        this.minimap.SetNativeSize();
        this.minimapBoundingBox = MinimapManager.Instance.minimapBoundingBox;
    }

    void Update ()
    {
        if (playerTransform == null)
        {
            if (User.Instance.CurrentCharacterObject != null)
            {
                playerTransform = User.Instance.CurrentCharacterObject.transform; //ȡ�õ�ǰ�����λ��
            }
        }
        if (minimapBoundingBox == null || playerTransform == null)  //������
            return;
        else
            MinimapManager.Instance.minimap = this;
        //��������ת����ʵ����
        float realWidth = minimapBoundingBox.bounds.size.x; //ȡ�ñ߽�п��
        float realHeight = minimapBoundingBox.bounds.size.z; //ȡ�ñ߽�и߶�
        //��ɫ��С��ͼ���������ת��
        float relaX = playerTransform.position.x - minimapBoundingBox.bounds.min.x;
        float relaY = playerTransform.position.z - minimapBoundingBox.bounds.min.z;
        //�������ת�����ĵ�����
        float pivotX = relaX / realWidth;
        float pivotY = relaY / realHeight;
        //С��ͼ�����ĵ��ƶ�
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
