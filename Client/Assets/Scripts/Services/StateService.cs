using System;
using Models;
using Network;
using UnityEngine;
using Charlotte.Proto;
using System.Collections.Generic;

namespace Services
{
    class StateService : Singleton<StateService>,IDisposable
    {
        public delegate bool StatusNoifyHandler(NStatus status);
        /// <summary>
        /// 单个事件集合（避免重复注册事件)
        /// </summary>
        HashSet<StatusNoifyHandler> hsndles = new HashSet<StatusNoifyHandler>();
        /// <summary>
        /// 状态事件集合
        /// </summary>
        Dictionary<StatusType, StatusNoifyHandler> StatusEvents = new Dictionary<StatusType, StatusNoifyHandler>();
        public StateService()
        {
            MessageDistributer.Instance.Subscribe<StateNotify>(this.OnStatusNotify);  //状态事件
        }
        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<StateNotify>(this.OnStatusNotify);
        }
        public void Init()
        {

        }

        /// <summary>
        /// 注册状态通知事件
        /// </summary>
        public void RegisterStatusNofity(StatusType function, StatusNoifyHandler action)
        {
            if(hsndles.Contains(action))
                return;
            if (!StatusEvents.ContainsKey(function))
                StatusEvents[function] = action;
            else
                StatusEvents[function] += action;
            hsndles.Add(action);
        }

        /// <summary>
        /// 状态事件回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="notify"></param>
        private void OnStatusNotify(object sender, StateNotify notify)
        {
            foreach (NStatus status in notify.Status) //遍历状态协议
            {
                Notify(status);
            }
        }

        /// <summary>
        /// 事件处理
        /// </summary>
        /// <param name="status"></param>
        private void Notify(NStatus status)
        {
            Debug.LogFormat("StatusNotify-> [{0}] [{1}] [{2}:{3}]", status.Type, status.Action, status.Id, status.Value);
            if (status.Type == StatusType.Money) 
            {
                if (status.Action == StatusAction.Add)
                    User.Instance.AddGold(status.Value);
                else if (status.Action == StatusAction.Delete)
                    User.Instance.AddGold(-status.Value);
            }
            StatusNoifyHandler handler;
            if (StatusEvents.TryGetValue(status.Type, out handler))  //发通知
            {
                handler(status);
            }
        }
    }
}
