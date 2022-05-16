using System;
using Models;
using Network;
using Managers;
using UnityEngine;
using Charlotte.Proto;

namespace Services
{
    class QuestService : Singleton<QuestService>, IDisposable
    {
        public QuestService()
        {
            MessageDistributer.Instance.Subscribe<QuestAcceptResponse>(this.OnQuestAccept); //接收任务
            MessageDistributer.Instance.Subscribe<QuestSubmitResponse>(this.OnQuestSubmit); //提交任务
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<QuestAcceptResponse>(this.OnQuestAccept);
            MessageDistributer.Instance.Unsubscribe<QuestSubmitResponse>(this.OnQuestSubmit);
        }

        /// <summary>
        /// 发送接收任务请求
        /// </summary>
        /// <param name="quest">Quest</param>
        /// <returns></returns>
        public bool SendQuestAccept(Quest quest)
        {
            Debug.Log("SendQuestAccept");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.questAccept = new QuestAcceptRequest();
            message.Request.questAccept.QuestId = quest.Define.ID;
            NetClient.Instance.SendMessage(message);
            return true;
        }

        /// <summary>
        /// 接收任务回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnQuestAccept(object sender, QuestAcceptResponse message)
        {
            Debug.LogFormat("QuestAccept-> {0},ERR:{1}", message.Result,message.Errorcode);
            if (message.Result == Result.Success)
                QuestManager.Instance.OnQuestAccepted(message.Quest);
            else
                MessageBox.Show("任务接受失败", "错误", MessageBoxType.Error);
        }

        /// <summary>
        /// 发送完成任务请求
        /// </summary>
        /// <param name="quest">Quest</param>
        /// <returns></returns>
        public bool SendQuestSubmit(Quest quest)
        {
            Debug.Log("SendQuestSubmit");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.questSubmit = new QuestSubmitRequest();
            message.Request.questSubmit.QuestId = quest.Define.ID;
            NetClient.Instance.SendMessage(message);
            return true;
        }

        /// <summary>
        /// 完成任务回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnQuestSubmit(object sender, QuestSubmitResponse message)
        {
            Debug.LogFormat("QuestSubmit-> {0},ERR:{1}", message.Result,message.Errorcode);
            if (message.Result == Result.Success)
                QuestManager.Instance.OnQuestSubmited(message.Quest);
            else
                MessageBox.Show("任务完成失败", "错误", MessageBoxType.Error);
        }

    }
}
