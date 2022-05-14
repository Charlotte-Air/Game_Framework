using System;
using System.Collections.Generic;

namespace Common.Network
{
    public class MessageManager:Singleton<MessageManager>
    {
        private class EventBroadcast
        {
            public Action<object> action;
            public bool state;
        }

        List<MessageType> tempMsg = new List<MessageType>();

        Dictionary<MessageType, List<EventBroadcast>> messageBroadcasts = new Dictionary<MessageType, List<EventBroadcast>>();

        /// <summary>
        /// 通知消息
        /// </summary>
        /// <param name="eType"></param>
        /// <param name="param"></param>
        public void NotifyMessage(MessageType eType, object param = null)
        {
            List<EventBroadcast> actionList = null;

            if (messageBroadcasts.ContainsKey(eType))
            {
                actionList = messageBroadcasts[eType];
            }

            if (actionList == null)
            {
                return;
            }

            var msgEnumerator = messageBroadcasts.GetEnumerator();
            while (msgEnumerator.MoveNext())
            {
                for (int i = msgEnumerator.Current.Value.Count - 1; i >= 0; i--)
                {
                    if (msgEnumerator.Current.Value[i].state)
                    {
                        msgEnumerator.Current.Value.RemoveAt(i);
                    }
                }
                if (msgEnumerator.Current.Value.Count == 0)
                {
                    tempMsg.Add(msgEnumerator.Current.Key);
                }
            }

            for (int i = tempMsg.Count - 1; i > 0; i--)
            {
                messageBroadcasts.Remove(tempMsg[i]);
                tempMsg.RemoveAt(i);
            }

            for (int i = actionList.Count - 1; i >= 0; i--)
            {
                if (!actionList[i].state)
                {
                    actionList[i].action.Invoke(param);
                }
            }
        }

        /// <summary>
        /// 订阅
        /// </summary>
        /// <param name="msgType"></param>
        /// <param name="action"></param>
        public void Subscribe(MessageType msgType, Action<object> action)
        {
            if (action == null)
            {
                return;
            }

            //tempBroadcastEvent
            List<EventBroadcast> actionList = null;
            EventBroadcast tempBroadcast = new EventBroadcast();
            tempBroadcast.action = action;
            tempBroadcast.state = false;

            if (messageBroadcasts.ContainsKey(msgType)) //GetBroadcastEventList
            {
                actionList = messageBroadcasts[msgType];
            }

            if (actionList == null) //Get BroadcastEventList is null new BroadcastEventList tempBroadcastEvntList Add.
            {
                actionList = new List<EventBroadcast>();
                actionList.Add(tempBroadcast);
                messageBroadcasts.Add(msgType, actionList);
            }
            else
            {
                for (int i = 0; i < actionList.Count; i++)
                {
                    if (actionList[i].action.Target == action.Target)
                    {
                        actionList[i].state = false;
                        return;
                    }
                }
                actionList.Add(tempBroadcast);
            }
        }

        /// <summary>
        /// 取消订阅
        /// </summary>
        /// <param name="target"></param>
        public void Unsubscribe(object target)
        {
            var mapMsgEnumerator = messageBroadcasts.GetEnumerator();
            while (mapMsgEnumerator.MoveNext())
            {
                List<EventBroadcast> acList = mapMsgEnumerator.Current.Value;
                for (var i = acList.Count - 1; i >= 0; i--)
                {
                    if (acList[i].action.Target == target)
                    {
                        acList[i].state = true;
                    }
                }
            }
        }

        /// <summary>
        /// 取消订阅
        /// </summary>
        /// <param name="msgType"></param>
        public void Unsubscribe(MessageType msgType)
        {
            foreach (var msg in messageBroadcasts)
            {
                if (msgType == msg.Key)
                {
                    for (var i = msg.Value.Count - 1; i >= 0; i--)
                    {
                        msg.Value[i].state = true;
                    }
                }
            }
        }

        /// <summary>
        /// 取消订阅
        /// </summary>
        /// <param name="msgType"></param>
        /// <param name="target"></param>
        public void Unsubscribe(MessageType msgType, object target)
        {
            List<MessageType> deleteList = new List<MessageType>();
            foreach (var msg in messageBroadcasts)
            {
                if (msgType == msg.Key)
                {
                    List<EventBroadcast> acList = msg.Value;
                    for (var i = acList.Count - 1; i >= 0; i--)
                    {
                        if (acList[i].action.Target == target)
                        {
                            acList[i].state = true;
                            break;
                        }
                    }
                }
            }
        }
    }
}
