using UnityEngine;

namespace Managers
{   /// <summary>
    /// 输入管理器
    /// </summary>
    public class InputManager : MonoSingleton<InputManager>
    {
        /// <summary>
        /// 打开UI数量
        /// </summary>
        public int OpenUI = 0;
        /// <summary>
        /// 打开UI
        /// </summary>
        public bool isOpenUI = false;
        /// <summary>
        /// 聊天中
        /// </summary>
        public bool IsInputMode = false;
        /// <summary>
        /// 锁定屏幕
        /// </summary>
        public bool lockTheScreen = false;
        /// <summary>
        /// 导航中
        /// </summary>
        public bool isNav = false;
        /// <summary>
        /// 加载中
        /// </summary>
        public bool isLoadEnd = false;
        /// <summary>
        /// NPC交互中
        /// </summary>
        public bool isNpcInteraction = false;
        /// <summary>
        /// 界面显示
        /// </summary>
        private bool isMain = true;
        /// <summary>
        /// 界面显示
        /// </summary>
        public bool ISMain
        {
            get { return isMain; }
            set { isMain = value; }
        }
        void LateUpdate()
        {
            if (Input.GetKeyUp(KeyCode.P))  //主UI显示与隐藏
            {
                if (isMain)
                    UIMain.Instance.OnHide();
                else
                    UIMain.Instance.OnShow();
            }
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                if (!lockTheScreen)
                    lockTheScreen = true;
                else
                    lockTheScreen = false;
            }
        }

        /// <summary>
        /// 角色离开时
        /// </summary>
        public void CharacterLeave()
        {
            this.OpenUI = 0;
            this.isOpenUI = false;
            this.IsInputMode = false;
            this.lockTheScreen = false;
            this.isNav = false;
            this.isLoadEnd = false;
            this.isNpcInteraction = false;
            this.isMain = true;
        }
    }
}
