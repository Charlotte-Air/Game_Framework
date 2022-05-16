using Models;
using Services;
using UnityEngine;
using UnityEngine.UI;
using Charlotte.Proto;
using System.Collections.Generic;

public class UICharacterSelect : MonoBehaviour 
{
    /// <summary>
    /// 创建职业面板
    /// </summary>
    public GameObject panelCreate;

    /// <summary>
    /// 角色列表面板
    /// </summary>
    public GameObject btnCreateCancel;

    /// <summary>
    /// 角色姓名输入框
    /// </summary>
    public InputField charName;

    /// <summary>
    /// 职业标题
    /// </summary>
    public Image[] titles;

    /// <summary>
    /// 职业描述
    /// </summary>
    public Text descs;

    /// <summary>
    /// 职业名称
    /// </summary>
    public Text[] names;

    /// <summary>
    /// 职业滚动区视图
    /// </summary>
    public UICharacterView characterView;

    /// <summary>
    /// 创建角色
    /// </summary>
    public GameObject panelSelect;

    public List<GameObject> uiChars = new List<GameObject>();

    /// <summary>
    /// 角色滚动区
    /// </summary>
    public Transform uiCharList;

    /// <summary>
    /// 滚动区角色信息
    /// </summary>
    public GameObject uiCharInfo;

    /// <summary>
    /// 职业索引值
    /// </summary>
    private int selectCharacterIdx = -1;

    /// <summary>
    /// 角色职业
    /// </summary>
    CharacterClass charClass;

    void Start()
    {
        InitCharacterSelect(true);
        UserService.Instance.OnCharacterCreate = OnCharacterCreate;
    }

    public void InitCharacterCreate()
    {
        panelCreate.SetActive(true);
        panelSelect.SetActive(false);
        OnSelectClass(1);
    }

    /// <summary>
    /// 创建角色
    /// </summary>
    public void OnClickCreate()
    {
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Button1);
        if (string.IsNullOrEmpty(this.charName.text))
        {
            MessageBox.Show("请输入角色名称");
            return;
        }
        UserService.Instance.SendCharacterCreate(this.charName.text, this.charClass);
    }

    /// <summary>
    /// 创建角色时职业索引信息事件
    /// </summary>
    /// <param name="charClass"></param>
    public void OnSelectClass(int charClass)
    {
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Button2);
        this.charClass = (CharacterClass)charClass;
        characterView.CurrectCharacter = charClass - 1;
        for (int i = 0; i < 3; i++)
        {
            titles[i].gameObject.SetActive(i == charClass - 1);
            names[i].text = DataManager.Instance.Characters[i + 1].Name;
        }
        descs.text = DataManager.Instance.Characters[charClass].Description;
    }

    /// <summary>
    /// 创建新角色回调事件
    /// </summary>
    /// <param name="result"></param>
    /// <param name="message"></param>
    void OnCharacterCreate(Result result, string message)
    {
        if (result == Result.Success)
        {
            SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Button2);
            InitCharacterSelect(true);
        }
        else
            MessageBox.Show(message, "错误", MessageBoxType.Error);
    }

    /// <summary>
    /// 初始化角色列表
    /// </summary>
    /// <param name="init"></param>
    public void InitCharacterSelect(bool init)
    {
        panelCreate.SetActive(false); //创建职业面板隐藏
        panelSelect.SetActive(true);  //角色选择面板显示
        if (init)
        {
            foreach (var old in uiChars)
            {
                Destroy(old);
            }
            uiChars.Clear();
            for (int i = 0; i < User.Instance.Info.Player.Characters.Count; i++)//遍历所有角色
            {
                GameObject go = Instantiate(uiCharInfo, this.uiCharList);
                UICharInfo chrInfo = go.GetComponent<UICharInfo>();
                chrInfo.info = User.Instance.Info.Player.Characters[i];
                Button button = go.GetComponent<Button>();
                int idx = i;
                button.onClick.AddListener(() => 
                {
                    OnSelectCharacter(idx);
                });
                uiChars.Add(go);
                go.SetActive(true);
            }
        }
    }

    /// <summary>
    /// 用户选择角色索引事件
    /// </summary>
    /// <param name="idx"></param>
    public void OnSelectCharacter(int idx)
    {
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Button2);
        this.selectCharacterIdx = idx; //idx当前选中索引值
        var cha = User.Instance.Info.Player.Characters[idx];
        Debug.LogFormat("Select Char:[{0}:{1}:{2}]", cha.Id, cha.Name, cha.Class);
        characterView.CurrectCharacter = ((int)cha.Class - 1);

        for (int i = 0; i < User.Instance.Info.Player.Characters.Count; i++) //角色按钮状态
        {
            UICharInfo ci = this.uiChars[i].GetComponent<UICharInfo>();
            ci.Selected = idx == i; //当前列表中选中索引 是不是等于 当前索引值
        }
    }

    public void OnClickPlay()
    {
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Button1);
        if (selectCharacterIdx >= 0)
        {
            UserService.Instance.SendGameEnter(selectCharacterIdx);
        }
        else
            MessageBox.Show("请选择角色~如没有角色请创建~");
    }

    public void OnClickPlaySound()
    {
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Button1);
    }
}
