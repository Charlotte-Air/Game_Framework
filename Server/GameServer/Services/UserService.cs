using Network;
using Common;
using System.Linq;
using Charlotte.Proto;
using GameServer.Entities;
using GameServer.Managers;

namespace GameServer.Services
{
    class UserService : Singleton<UserService>
    {
        public UserService() 
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserLoginRequest>(this.OnLogin);                                   //用户登入
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserRegisterRequest>(this.OnRegister);                           //用户注册
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserCreateCharacterRequest>(this.OnCreateCharacter);   //创建角色
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserGameEnterRequest>(this.OnGameEnter);                   //角色进入游戏
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserGameLeaveRequest>(this.OnGameLeave);                 //角色离开游戏
        }

        public void Init()
        {
            
        }

        /// <summary>
        /// 服务端处理用户登入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        void OnLogin(NetConnection<NetSession> sender, UserLoginRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("UserLoginRequest-> User [{0}]  Pass [{1}]", request.User, request.Passward);
            sender.Session.Response.userLogin = new UserLoginResponse();
            TUser user = DBService.Instance.Entities.Users.Where(u => u.Username == request.User).FirstOrDefault();
            if (user == null) //判断用户是否存在
            {
                sender.Session.Response.userLogin.Result = Result.Failed;
                sender.Session.Response.userLogin.Errorcode = (uint)ErrorCode.ErrorUserNoExist;
            }
            else if (user.Password != request.Passward) //判断用户密码是否正确
            {
                sender.Session.Response.userLogin.Result = Result.Failed;
                sender.Session.Response.userLogin.Errorcode = (uint)ErrorCode.ErrorPassward;
            }
            else
            {
                var userSn = SessionManager.Instance.GetSession(user.Player.ID);
                if (userSn != null) //效验用户是否在线
                {
                    sender.Session.Response.userLogin.Result = Result.Failed;
                    sender.Session.Response.userLogin.Errorcode = (uint)ErrorCode.ErrorUserOnline;
                }
                else
                {
                    sender.Session.User = user; //读取用户数据库信息
                    sender.Session.Response.userLogin.Result = Result.Success;
                    sender.Session.Response.userLogin.Errorcode = (int)ErrorCode.Succed;
                    sender.Session.Response.userLogin.Userinfo = new NUserInfo();
                    sender.Session.Response.userLogin.Userinfo.Id = (int)user.ID;
                    sender.Session.Response.userLogin.Userinfo.Player = new NPlayerInfo();
                    sender.Session.Response.userLogin.Userinfo.Player.Id = user.Player.ID;
                    foreach (var dbCharacter in user.Player.Characters) //将DB已有DATA填充到协议发送至客户端
                    {
                        NCharacterInfo info = new NCharacterInfo();
                        info.Id = dbCharacter.ID;
                        info.Name = dbCharacter.Name;
                        info.Type = CharacterType.Player;
                        info.Class = (CharacterClass)dbCharacter.Class;
                        sender.Session.Response.userLogin.Userinfo.Player.Characters.Add(info);
                    }
                }
            }
            sender.SendResponse(); //向客户端发送响应
        }

        /// <summary>
        /// 服务端处理用户注册
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        void OnRegister(NetConnection<NetSession> sender, UserRegisterRequest request)
        {
            Log.InfoFormat("UserRegisterRequest-> User [{0}] Pass [{1}]", request.User, request.Passward);
            sender.Session.Response.userRegister = new UserRegisterResponse();
            TUser user = DBService.Instance.Entities.Users.Where(u => u.Username == request.User).FirstOrDefault();
            if (user != null) //判断用户是否注册过
            {
                sender.Session.Response.userRegister.Result = Result.Failed;
                sender.Session.Response.userRegister.Errorcode = (uint)ErrorCode.ErrorUserExist;
            }
            else //注册
            {
                TPlayer player = DBService.Instance.Entities.Players.Add(new TPlayer());
                DBService.Instance.Entities.Users.Add(new TUser() { Username = request.User, Password = request.Passward, Player = player });
                DBService.Instance.Entities.SaveChanges();
                sender.Session.Response.userRegister.Result = Result.Success;
                sender.Session.Response.userRegister.Errorcode = (uint)ErrorCode.Succed;
            }
            sender.SendResponse(); //向客户端发送响应
        }

        /// <summary>
        /// 服务端处理创建角色
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        private void OnCreateCharacter(NetConnection<NetSession> sender, UserCreateCharacterRequest request)
        {
            Log.InfoFormat("UserCreateCharacterRequest-> Name [{0}] Class [{1}]", request.Name, request.Class);
            TCharacter character = new TCharacter() //角色DB创建
            {
                Name = request.Name,
                Class = (int)request.Class,
                TID = (int)request.Class,
                Level = 1,                 //初始等级
                MapID = 1,              //初始地图
                MapPosX = 10476, //初始出生位置X
                MapPosY = 6306,   //初始出生位置Y
                MapPosZ = 786,    //初始出生位置Z
                Gold = 10000,       //初始金币
                Equips = new byte[28], //装备初始
            };
            var bag = new TCharacterBag(); //背包DB创建
            bag.Owner = character;      //所有者赋值
            bag.Items = new byte[0];   //初始化背包为0字节
            bag.Unlocked = 20;            //背包默认格子
            character.Bag = DBService.Instance.Entities.TCharacterBags.Add(bag); //绑定角色背包数据
            character = DBService.Instance.Entities.Characters.Add(character);
            ///-----------------------注册角色初始道具-------------------------------
            Character user = sender.Session.Character;
            //character.Items.Add(user.ItemManager.AddItem(character, 1, 20));
            //character.Items.Add(user.ItemManager.AddItem(character, 2, 20));
            sender.Session.User.Player.Characters.Add(character);
            DBService.Instance.Entities.SaveChanges();
            ///-------------------------------------------------------------------------
            sender.Session.Response.createChar = new UserCreateCharacterResponse();
            sender.Session.Response.createChar.Result = Result.Success;
            sender.Session.Response.createChar.Errorcode = (uint)ErrorCode.Succed;
            foreach (var c in sender.Session.User.Player.Characters) //当前已经有的角色添加在列表进行刷新角色
            {
                NCharacterInfo info = new NCharacterInfo();
                info.Id = 0;
                info.Name = c.Name;
                info.Type = CharacterType.Player;
                info.Class = (CharacterClass)c.Class;
                info.ConfigId = c.TID;
                sender.Session.Response.createChar.Characters.Add(info);
            }
            sender.SendResponse(); //向客户端发送
        }

        /// <summary>
        /// 服务端处理游戏角色进入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        void OnGameEnter(NetConnection<NetSession> sender, UserGameEnterRequest request)
        {
            TCharacter dbchar = sender.Session.User.Player.Characters.ElementAt(request.characterIdx); //获得角色的信息
            Log.InfoFormat("UserGameEnterRequest-> Character [{0}:{1}] MapID [{2}]", dbchar.ID, dbchar.Name, dbchar.MapID);
            var CharacterSe = SessionManager.Instance.GetSession(request.characterId);
            if (CharacterSe != null) //效验是否在线同角色无法进入
            {
                sender.Session.Response.gameEnter = new UserGameEnterResponse();
                sender.Session.Response.gameEnter.Result = Result.Failed;
                sender.Session.Response.gameEnter.Errorcode = (uint)ErrorCode.ErrorUserOnlineTitle;
                sender.SendResponse();
            }
            else
            {
                Character character = CharacterManager.Instance.AddCharacter(dbchar);
                SessionManager.Instance.AddSession(character.Id, sender);
                sender.Session.Response.gameEnter = new UserGameEnterResponse();
                sender.Session.Response.gameEnter.Result = Result.Success;
                sender.Session.Response.gameEnter.Errorcode = (uint)ErrorCode.Succed;
                sender.Session.Response.gameEnter.Character = character.Info; //进入成功，发送初始角色信息
                sender.Session.Character = character; //角色信息赋值
                sender.Session.PostResponser = character;  //初始化后处理器
                sender.SendResponse(); //向客户端发送
                MapManager.Instance[dbchar.MapID].CharacterEnter(sender, character);
            }
        }

        /// <summary>
        /// 服务端处理游戏角色离开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        void OnGameLeave(NetConnection<NetSession> sender, UserGameLeaveRequest request)
        {
            Character character = sender.Session.Character; //获得角色的信息
            Log.InfoFormat("UserGameLeaveRequest-> Character [{0}:{1}] MapID [{2}]", character.Id, character.Info.Name, character.Info.mapId);
            this.CharacterLeave(character);
            sender.Session.Response.gameLeave = new UserGameLeaveResponse();
            sender.Session.Response.gameLeave.Result = Result.Success;
            sender.Session.Response.gameLeave.Errorcode = (uint)ErrorCode.Succed;
            sender.SendResponse(); //向客户端发送
        }

        /// <summary>
        /// 角色离开
        /// </summary>
        /// <param name="character"></param>
        public void CharacterLeave(Character character)
        {
            Log.InfoFormat("CharacterLeave-> Character [{0}:{1}]",character.Id,character.Info.Name);
            SessionManager.Instance.RemovSession(character.Id);
            CharacterManager.Instance.RemoveCharacter(character.Id);
            character.Clear();
            MapManager.Instance[character.Info.mapId[0]].CharacterLeave(character);
        }

    }
}
