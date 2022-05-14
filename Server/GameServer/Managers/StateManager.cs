using Charlotte.Proto;
using GameServer.Entities;
using System.Collections.Generic;

namespace GameServer.Managers
{   
    public enum NoticeType
    {
        DEFAULT = 0,
        ITEM_ADD,
        ITEM_REMOVE,
        GOLD_ADD,
    }

    /// <summary>
    /// 状态管理器
    /// </summary>
    class StateManager
    {
        Character Owner;
        Queue<NStatus> States { get; set; }
        public bool HasStatus { get { return this.States.Count > 0; } } 

        public StateManager(Character owner)
        {
            this.Owner = owner;
            this.States = new Queue<NStatus>();
        }

        /// <summary>
        /// 状态通知
        /// </summary>
        /// <param name="type">状态类型</param>
        /// <param name="id">状态ID</param>
        /// <param name="value">状态值</param>
        /// <param name="action">状态事件类型</param>
        public void StateNotice(StatusType type, int id, int value, StatusAction action)
        {
            this.States.Enqueue(new NStatus()
            {
                Type = type,
                Id = id,
                Value = value,
                Action = action
            });
        }

        /// <summary>
        /// 后处理
        /// </summary>
        /// <param name="message"></param>
        public void PostProcess(NetMessageResponse message)
        {
            if (message.stateNotify == null)
                message.stateNotify = new StateNotify();
            foreach (var state in States)
            {
                message.stateNotify.Status.Add(state);
            }
            this.States.Clear();
        }
    }
}
