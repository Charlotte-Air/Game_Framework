using Network;
using Common;
using Charlotte.Proto;
using GameServer.Entities;

namespace GameServer.Services
{
    class QuestService :  Singleton<QuestService>
    {
        public QuestService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<QuestAcceptRequest>(this.OnQuestAccept);   //任务接受
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<QuestSubmitRequest>(this.OnQuestSubmit);   //任务提交
        }

        public void Init()
        {

        }

        /// <summary>
        /// 服务端处理接受任务请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        private void OnQuestAccept(NetConnection<NetSession> sender, QuestAcceptRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("QuestAcceptRequest-> Character [{0}:{1}]  QuestID [{2}]",character.Id,character.Name,request.QuestId);
            sender.Session.Response.questAccept = new QuestAcceptResponse();
            Result result = character.QuestManager.AcceptQuest(sender, request.QuestId);
            sender.Session.Response.questAccept.Result = result;
            sender.SendResponse();
        }

        /// <summary>
        /// 服务端处理提交任务请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        private void OnQuestSubmit(NetConnection<NetSession> sender, QuestSubmitRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("QuestSubmitRequest-> Character [{0}:{1}] QuestID [{2}]", character.Id,character.Name,request.QuestId);
            sender.Session.Response.questSubmit = new QuestSubmitResponse();
            Result result = character.QuestManager.SubmitQuest(sender, request.QuestId);
            sender.Session.Response.questSubmit.Result = result;
            sender.SendResponse();
        }
    }
}
