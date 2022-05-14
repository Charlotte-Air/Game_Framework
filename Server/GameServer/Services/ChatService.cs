using Network;
using Common;
using Charlotte.Proto;
using GameServer.Entities;
using GameServer.Managers;

namespace GameServer.Services
{
    class ChatService:Singleton<ChatService>
    {
        public ChatService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<ChatRequest>(this.OnChat); //聊天处理
        }

        public void Init()
        {
            ChatManager.Instance.Init();
        }

        /// <summary>
        /// 服务端处理聊天事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        private void OnChat(NetConnection<NetSession> sender, ChatRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("Chat-> Character [{0}:{1}]  Channel [{2} : Message{3}]",character.Id,character.Name,request.Message.Channel,request.Message.Message);
            if (request.Message.Channel == ChatChannel.Private) //私聊处理
            {
                var chatTo = SessionManager.Instance.GetSession(request.Message.ToId); //取目标是否在线
                if (chatTo == null)
                {
                    sender.Session.Response.Chat = new ChatResponse();
                    sender.Session.Response.Chat.Result = Result.Failed;
                    sender.Session.Response.Chat.Errorcode = (uint)ErrorCode.ErrorChatUserNoOnline;
                    sender.Session.Response.Chat.privateMessages.Add(request.Message);
                    sender.SendResponse();
                }
                else
                {
                    if (chatTo.Session.Response.Chat == null)
                        chatTo.Session.Response.Chat = new ChatResponse();
                    request.Message.FromId = character.Id;
                    request.Message.FromName = character.Name;
                    chatTo.Session.Response.Chat.Result = Result.Success;
                    chatTo.Session.Response.Chat.privateMessages.Add(request.Message);
                    chatTo.SendResponse();
                    if (sender.Session.Response.Chat == null)
                        sender.Session.Response.Chat = new ChatResponse();
                    sender.Session.Response.Chat.Result = Result.Success;
                    sender.Session.Response.Chat.privateMessages.Add(request.Message);
                    sender.SendResponse();
                }
            }
            else  //其他聊天处理
            {
                sender.Session.Response.Chat = new ChatResponse();
                sender.Session.Response.Chat.Result = Result.Success;
                ChatManager.Instance.AddMessage(character,request.Message);
                sender.SendResponse();
            }
        }

    }
}
