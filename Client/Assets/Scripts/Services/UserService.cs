using System;
using Models;
using Network;
using Common;
using Managers;
using UnityEngine;
using Charlotte.Proto;

namespace Services
{
    class UserService : Singleton<UserService>, IDisposable
    {
        public UnityEngine.Events.UnityAction<Result, string> OnLogin;
        public UnityEngine.Events.UnityAction<Result, string> OnRegister;
        public UnityEngine.Events.UnityAction<Result, string> OnCharacterCreate;
        NetMessage pendingMessage = null;
        bool connected = false;
        bool isQuitGame = false;

        public UserService() 
        {
            NetClient.Instance.OnConnect += OnGameServerConnect; 
            NetClient.Instance.OnDisconnect += OnGameServerDisconnect;
            MessageDistributer.Instance.Subscribe<UserLoginResponse>(this.OnUserLogin);                                      //用户登入响应
            MessageDistributer.Instance.Subscribe<UserRegisterResponse>(this.OnUserRegister);                              //用户注册响应
            MessageDistributer.Instance.Subscribe<UserCreateCharacterResponse>(this.OnUserCreateCharacter);     //创建角色响应
            MessageDistributer.Instance.Subscribe<UserGameEnterResponse>(this.OnGameEnter);                            //角色进入响应
            MessageDistributer.Instance.Subscribe<UserGameLeaveResponse>(this.OnGameLeave);                          //角色离开响应
        }

        public void Dispose()
        {
            NetClient.Instance.OnConnect -= OnGameServerConnect;
            NetClient.Instance.OnDisconnect -= OnGameServerDisconnect;
            MessageDistributer.Instance.Unsubscribe<UserLoginResponse>(this.OnUserLogin);
            MessageDistributer.Instance.Unsubscribe<UserRegisterResponse>(this.OnUserRegister);
            MessageDistributer.Instance.Unsubscribe<UserCreateCharacterResponse>(this.OnUserCreateCharacter);
            MessageDistributer.Instance.Unsubscribe<UserGameEnterResponse>(this.OnGameEnter);
            MessageDistributer.Instance.Unsubscribe<UserGameLeaveResponse>(this.OnGameLeave);
        }

        public void Init()
        {

        }
        
        /// <summary>
        /// 连接服务器
        /// </summary>
        public void ConnectToServer()
        {
            Debug.Log("ConnectToServer()->Start ");
            //NetClient.Instance.CryptKey = this.SessionId;
            NetClient.Instance.Init("127.0.0.1", 8000); //服务器配置
            NetClient.Instance.Connect(); //连接服务器
        }

        /// <summary>
        /// 链接通知
        /// </summary>
        void OnGameServerConnect(int result, string reason)
        {
            Log.InfoFormat("LoadingMesager-> GameServerConnect-> {0} Reason:{1}", result, reason);
            if (NetClient.Instance.Connected) //捕获链接事件
            {
                this.connected = true;
                if(this.pendingMessage!=null) //判断是否有链接前是否有消息
                {
                    NetClient.Instance.SendMessage(this.pendingMessage); //有的话补发一下
                    this.pendingMessage = null;
                }
            }
            else
            {
                if (!this.DisconnectNotify(result, reason))
                {
                    MessageBox.Show(string.Format("网络错误，无法连接到服务器！\n RESULT:{0} ERROR:{1}", result, reason), "错误", MessageBoxType.Error);
                }
            }
        }

        /// <summary>
        /// 断开链接通知
        /// </summary>
        public void OnGameServerDisconnect(int result, string reason)
        {
            this.DisconnectNotify(result, reason);
            return;
        }

        bool DisconnectNotify(int result,string reason)
        {
            if (this.pendingMessage != null)
            {
                if(this.pendingMessage.Request.userLogin!=null)
                {
                    if (this.OnLogin != null)
                    {
                        this.OnLogin(Result.Failed, string.Format("服务器断开！\n RESULT:{0} ERROR:{1}", result, reason));
                    }
                }
                else if (this.pendingMessage.Request.userRegister!=null)
                {
                    if (this.OnRegister != null)
                    {
                        this.OnRegister(Result.Failed, string.Format("服务器断开！\n RESULT:{0} ERROR:{1}", result, reason));
                    }
                }
                else
                {
                    if (this.OnCharacterCreate != null)
                    {
                        this.OnCharacterCreate(Result.Failed, string.Format("服务器断开！\n RESULT:{0} ERROR:{1}", result, reason));
                    }
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 发送用户登入
        /// </summary>
        /// <param name="user">用户名</param>
        /// <param name="psw">密码</param>
        public void SendLogin(string user,string psw)
        {
            Debug.LogFormat("UserLoginRequest-> User :{0} Psw:{1}", user, psw);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.userLogin = new UserLoginRequest();
            message.Request.userLogin.User = user;
            message.Request.userLogin.Passward = psw;
            if(this.connected && NetClient.Instance.Connected)
            {
                this.pendingMessage = null;
                NetClient.Instance.SendMessage(message);
            }
            else
            {
                this.pendingMessage = message;
                this.ConnectToServer();
            }
        }

        /// <summary>
        /// 回调用户登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="response"></param>
        void OnUserLogin(object sender,UserLoginResponse response)
        {
            Debug.LogFormat("Login-> {0} [{1}]", response.Result, response.Errorcode);
            if (response.Result == Result.Success)
            {
                Models.User.Instance.SetupUserInfo(response.Userinfo);//登陆成功逻辑
            }
            if(this.OnLogin != null) //判断是否订阅
            {
                this.OnLogin(response.Result,response.Errorcode.ToString());
            }
        }

        /// <summary>
        /// 发送用户注册
        /// </summary>
        /// <param name="user">用户名</param>
        /// <param name="psw">密码</param>
        public void SendRegister(string user, string psw)
        {
            Debug.LogFormat("UserRegisterRequest-> User :{0} Psw:{1}", user, psw);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.userRegister = new UserRegisterRequest();
            message.Request.userRegister.User = user;
            message.Request.userRegister.Passward = psw;
            if (this.connected && NetClient.Instance.Connected) //判断链接是否连接
            {
                this.pendingMessage = null;
                NetClient.Instance.SendMessage(message);
            }
            else //如果没连上进入消息队列
            {
                this.pendingMessage = message;
                this.ConnectToServer(); 
            }
        }

        /// <summary>
        /// 回调用户注册
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="response"></param>
        void OnUserRegister(object sender, UserRegisterResponse response)
        {
            Debug.LogFormat("UserRegister-> {0} [{1}]", response.Result, response.Errorcode);
            if (this.OnRegister != null) //判断是否订阅
            {
                this.OnRegister(response.Result, response.Errorcode.ToString());
            }
        }

        /// <summary>
        /// 发送角色创建
        /// </summary>
        /// <param name="name">角色名</param>
        /// <param name="cls">职业</param>
        public void SendCharacterCreate(string name, CharacterClass cls)
        {
            Debug.LogFormat("UserCreateCharacterRequest-> Name :{0} Class:{1}", name, cls);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest(); //发送请求
            message.Request.createChar = new UserCreateCharacterRequest(); //创建角色
            message.Request.createChar.Name = name;
            message.Request.createChar.Class = cls;
            if (this.connected && NetClient.Instance.Connected)  //判断链接是否连接
            {
                this.pendingMessage = null;
                NetClient.Instance.SendMessage(message);
            }
            else //如果没连上进入消息队列
            {
                this.pendingMessage = message;
                this.ConnectToServer();
            }
        }

        /// <summary>
        /// 回调角色创建
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="response"></param>
        void OnUserCreateCharacter(object sender,UserCreateCharacterResponse response)
        {
            Debug.LogFormat("UserCreateCharacter-> {0} [{1}]", response.Result, response.Errorcode);
            if (response.Result == Result.Success)
            {
                Models.User.Instance.Info.Player.Characters.Clear();
                Models.User.Instance.Info.Player.Characters.AddRange(response.Characters);
            }
            if (this.OnCharacterCreate != null) //判断是否订阅
            {
                this.OnCharacterCreate(response.Result, response.Errorcode.ToString());
            }
        }

        /// <summary>
        /// 角色进入游戏
        /// </summary>
        /// <param name="characterIdx">职业索引值</param>
        public void SendGameEnter(int characterIdx)
        {
            Debug.LogFormat("UserGameEnterRequest-> CharacterIdx[{0}]:", characterIdx);
            ChatManager.Instance.Init(); //初始化聊天管理器
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.gameEnter = new UserGameEnterRequest();
            message.Request.gameEnter.characterIdx = characterIdx;
            NetClient.Instance.SendMessage(message);
        }

        /// <summary>
        /// 回调角色进入游戏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="response"></param>
        void OnGameEnter(object sender,UserGameEnterResponse response)
        {
            Debug.LogFormat("GameEnter-> {0} [{1}]", response.Result, response.Errorcode);
            if (response.Result == Result.Success)
            {
                if (response.Character != null)
                {
                    User.Instance.CurrentCharacter = response.Character;
                    ItemManager.Instance.Init(response.Character.Items);       //初始化道具管理器
                    BagManager.Instance.Init(response.Character.Bag);           //初始化背包管理器
                    EquipManager.Instance.Init(response.Character.Equips);   //初始化装备管理器
                    QuestManager.Instance.Init(response.Character.Quests);  //初始化任务管理器
                    FriendManager.Instance.Init(response.Character.Friends); //初始化好友管理器
                }
            }
            else
                MessageBox.Show(response.Errorcode.ToString(), "错误", MessageBoxType.Error);
        }

        /// <summary>
        /// 角色离开游戏
        /// </summary>
        /// <param name="isQuitGame"></param>
        public void SendGameLeave(bool isQuitGame=false)
        {
            this.isQuitGame = isQuitGame;
            Debug.Log("->UserGameLeaveRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.gameLeave = new UserGameLeaveRequest();
            NetClient.Instance.SendMessage(message);
        }

        /// <summary>
        /// 回调角色离开游戏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="response"></param>
        void OnGameLeave(object sender, UserGameLeaveResponse response)
        {
            Debug.LogFormat("GameLeave-> {0} [{1}]", response.Result, response.Errorcode);
            MapService.Instance.CurrentMapId = 0;      //地图ID清空
            SceneManager.Instance.SceneLeave();        //场景管理器数据清空
            InputManager.Instance.CharacterLeave();    //控制器数据清空
            User.Instance.CurrentCharacter = null;        //角色数据清空
            ChatManager.Instance.ChatClear();              //聊天数据清空
            if (this.isQuitGame)
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }
        }
    }
}
