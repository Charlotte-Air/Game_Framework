using Network;
using Common;
using Charlotte.Proto;
using GameServer.Models;
using GameServer.Managers;

namespace GameServer.Entities
{
    /// <summary>
    /// 角色类
    /// </summary>
    class Character : CharacterBase, IPostResponser
    {
        public Chat Chat;
        public Guild Guild;
        public Team Team;
        public TCharacter Data;
        public ItemManager ItemManager;
        public QuestManager QuestManager;
        public StateManager StateManager;
        public FriendManager FriendManager;
        public double TeamUpdateTS; //时间戳
        public double GuildUpdateTS; //时间戳

        public Character(CharacterType type,TCharacter dbChararcter) :
        base(new Core.Vector3Int(dbChararcter.MapPosX, dbChararcter.MapPosY, dbChararcter.MapPosZ),new Core.Vector3Int(100,0,0))
        {
            this.Data = dbChararcter;
            this.Id = dbChararcter.ID;
            this.Info = new NCharacterInfo()
            {
                Type = type,
                Id = dbChararcter.ID,
                EntityId = this.entityId,
                ConfigId = dbChararcter.TID,
                Name = dbChararcter.Name,
                Level = dbChararcter.Level,
                Gold = dbChararcter.Gold,
                Class = (CharacterClass)dbChararcter.Class,
                mapId = new byte[1] { dbChararcter.MapID },
                Entity = this.EntityData,
                Equips = this.Data.Equips,
                Bag = new NBagInfo()
                {
                    Unlocked = this.Data.Bag.Unlocked,
                    Items = this.Data.Bag.Items,
                },
            };
            this.ItemManager = new ItemManager(this);
            this.ItemManager.GetItemInfos(this.Info.Items);
            this.QuestManager = new QuestManager(this);
            this.QuestManager.GetQuestInfos(this.Info.Quests);
            this.StateManager = new StateManager(this);
            this.FriendManager = new FriendManager(this);
            this.FriendManager.GetFriendInfos(this.Info.Friends);
            this.Chat = new Chat(this);
            this.Define = DataManager.Instance.Characters[this.Info.ConfigId];
            this.Guild = GuildManager.Instance.GetGuild(this.Data.GuildId);
        }

        public long Gold
        {
            get => this.Data.Gold;
            set
            {
                if (this.Data.Gold != value)
                {
                    var gold = (int)(value - this.Data.Gold);
                    this.StateManager.StateNotice(StatusType.Money, 0, gold > 0 ? gold : -gold, gold > 0 ? StatusAction.Add : StatusAction.Delete);
                    this.Data.Gold = value;
                }
            }
        }

        /// <summary>
        /// 后处理器
        /// summary>
        public void PostProcess(NetMessageResponse message)
        {
            if (this.StateManager.HasStatus) //状态管理器后处理
            {
                this.StateManager.PostProcess(message);
                Log.InfoFormat("Status PostProcess-> Character [{0}:{1}]", this.Id, this.Info.Name);
            }
            this.FriendManager.PostProcess(message); //好友后处理
            Log.InfoFormat("Chat PostProcess-> Character [{0}:{1}]", this.Id, this.Info.Name);
            this.Chat.PostProcess(message); //聊天后处理
            if (this.Team != null && TeamUpdateTS < this.Team.timestamp)
            {
                TeamUpdateTS = Team.timestamp;
                this.Team.PostProcess(message);  //组队后处理
                Log.InfoFormat("Team PostProcess-> Character [{0}:{1}] Timestamp [{2} < {3}]", this.Id, this.Info.Name, TeamUpdateTS, this.Team.timestamp);
            }
            if (this.Guild != null) //公会后处理
            {
                if (this.Info.Guild == null)
                {
                    this.Info.Guild = this.Guild.GuildInfo(this);
                    if (message.mapCharacterEnter != null)
                        GuildUpdateTS = Guild.timestamp;
                }
                if (GuildUpdateTS < this.Guild.timestamp && message.mapCharacterEnter == null)
                {
                    this.GuildUpdateTS = Guild.timestamp;
                    this.Guild.PostProcess(this, message);
                    Log.InfoFormat("Guild PostProcess-> Character [{0}:{1}] Timestamp [{2} < {3}]", this.Id, this.Info.Name, GuildUpdateTS, this.Guild.timestamp);
                }
            }
        }

        public void Clear() => this.FriendManager.offlineNotify();

        public NCharacterInfo GetBasicInfo()
        {
            return new NCharacterInfo()
            {
                Id = this.Id,
                Name = this.Info.Name,
                Class = this.Info.Class,
                Level = this.Info.Level
            };
        }
    }
}
