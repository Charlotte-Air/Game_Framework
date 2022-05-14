namespace Network
{
    /// <summary>
    /// ��Ϣ�ַ���[ȷ�Ϻ�ǰ��˽���Э��֮����Ҫ�����������Ҫ����Э�顢��Ӻ�����յ�����˷��ص���Ϣ]
    /// </summary>
    public class MessageDispatch<T> : Common.Singleton<MessageDispatch<T>>
    {
        /// <summary>
        /// ������Ӧ
        /// </summary>
        /// <param name="sender">������</param>
        /// <param name="message">��Ϣ</param>
        public void Dispatch(T sender, Charlotte.Proto.NetMessageResponse message)
        {
            if (message.mapEntitySync != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.mapEntitySync); }
            if (message.mapCharacterEnter != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.mapCharacterEnter); }
            if (message.mapCharacterLeave != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.mapCharacterLeave); }
            if (message.stateNotify != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.stateNotify); }
            if (message.userRegister != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.userRegister); }
            if (message.userLogin != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.userLogin); }
            if (message.createChar != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.createChar); }
            if (message.gameEnter != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.gameEnter); }
            if (message.gameLeave != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.gameLeave); }
            if (message.itemBuy != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.itemBuy); }
            if (message.itemEquip != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.itemEquip); }
            if (message.questAccept != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.questAccept); }
            if (message.questSubmit != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.questSubmit); }
            if (message.friendAddReq != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.friendAddReq); }
            if (message.friendAddRes != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.friendAddRes); }
            if (message.friendList != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.friendList); }
            if (message.friendRemove != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.friendRemove); }
            if (message.teamInviteReq != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.teamInviteReq); }
            if (message.teamInviteRes != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.teamInviteRes); }
            if (message.teamInfo != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.teamInfo); }
            if (message.teamLeave != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.teamLeave); }
            if (message.guildCreate != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.guildCreate); }
            if (message.guildJoinReq != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.guildJoinReq); }
            if (message.guildJoinRes != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.guildJoinRes); }
            if (message.Guild != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.Guild); }
            if (message.guildList != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.guildList); }
            if (message.guildLeave != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.guildLeave); }
            if (message.guildAdmin != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.guildAdmin); }
            if (message.Chat != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.Chat); }
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="sender">������</param>
        /// <param name="message">��Ϣ</param>
        public void Dispatch(T sender, Charlotte.Proto.NetMessageRequest message)
        {
            if (message.mapEntitySync != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.mapEntitySync); }
            if (message.mapCharacterEnter != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.mapCharacterEnter); }
            if (message.mapTeleport != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.mapTeleport); }
            if (message.userRegister != null) { MessageDistributer<T>.Instance.RaiseEvent(sender,message.userRegister); }
            if (message.userLogin != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.userLogin); }
            if (message.createChar != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.createChar); }
            if (message.gameEnter != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.gameEnter); }
            if (message.gameLeave != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.gameLeave); }
            if (message.itemBuy != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.itemBuy); }
            if (message.itemEquip != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.itemEquip); }
            if (message.questAccept != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.questAccept); }
            if (message.questSubmit != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.questSubmit); }
            if (message.friendAddReq != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.friendAddReq); }
            if (message.friendAddRes != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.friendAddRes); }
            if (message.friendList != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.friendList); }
            if (message.friendRemove != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.friendRemove); }
            if (message.teamInviteReq != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.teamInviteReq); }
            if (message.teamInviteRes != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.teamInviteRes); }
            if (message.teamInfo != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.teamInfo); }
            if (message.teamLeave != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.teamLeave); }
            if (message.guildCreate != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.guildCreate); }
            if (message.guildJoinReq != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.guildJoinReq); }
            if (message.guildJoinRes != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.guildJoinRes); }
            if (message.Guild != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.Guild); }
            if (message.guildList != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.guildList); }
            if (message.guildLeave != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.guildLeave); }
            if (message.guildAdmin != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.guildAdmin); }
            if (message.Chat != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.Chat); }
        }
    }
}