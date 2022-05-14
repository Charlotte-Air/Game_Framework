using System;
using Common;
using System.Threading;
using System.Collections.Generic;

namespace Network
{
    /// <summary>
    /// 消息分发器接口
    /// </summary>
    public class MessageDistributer : MessageDistributer<object>
    {

    }

    /// <summary>
    /// 消息分发器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MessageDistributer<T> : Singleton<MessageDistributer<T>>
    {
        /// <summary>
        /// 消息参数
        /// </summary>
        class MessageArgs
        {
            public T sender; //发送者
            public Charlotte.Proto.NetMessage message; //消息data
        }

        private AutoResetEvent threadEvent = new AutoResetEvent(true); //线程事件
        public delegate void MessageHandler<Tm>(T sender, Tm message);  //订阅委托
        private Queue<MessageArgs> messageQueue = new Queue<MessageArgs>(); //消息分发队列
        private Dictionary<string, System.Delegate> messageHandlers = new Dictionary<string, System.Delegate>(); //消息处理集合

        private int ThreadCount = 0;               //线程数
        private int ActiveThreadCount = 0;     //使用中线程数
        private bool Running = false;              //线程运行状态
        private bool ThrowException = false;  //线程异常状态
        public bool SetThrowException { set => ThrowException = value; }

        public MessageDistributer() {   }

        /// <summary>
        /// 订阅
        /// </summary>
        /// <typeparam name="Tm">消息类型</typeparam>
        /// <param name="messageHandler">消息处理者</param>
        public void Subscribe<Tm>(MessageHandler<Tm> messageHandler)
        {
            if (!messageHandlers.ContainsKey(typeof(Tm).Name))
            {
                messageHandlers[typeof(Tm).Name] = null;
            }
            messageHandlers[typeof(Tm).Name] = (MessageHandler<Tm>)messageHandlers[typeof(Tm).Name] + messageHandler;
        }

        /// <summary>
        /// 取消订阅
        /// </summary>
        /// <typeparam name="Tm">消息类型</typeparam>
        /// <param name="messageHandler">消息处理者</param>
        public void Unsubscribe<Tm>(MessageHandler<Tm> messageHandler)
        {
            if (!messageHandlers.ContainsKey(typeof(Tm).Name))
            {
                messageHandlers[typeof(Tm).Name] = null;
            }
            messageHandlers[typeof(Tm).Name] = (MessageHandler<Tm>)messageHandlers[typeof(Tm).Name] - messageHandler;
        }
        
        /// <summary>
        /// 引发事件
        /// </summary>
        /// <typeparam name="Tm"></typeparam>
        /// <param name="sender">发送者</param>
        /// <param name="msg"></param>
        public void RaiseEvent<Tm>(T sender,Tm msg)
        {
            if (messageHandlers.ContainsKey(msg.GetType().Name))
            {
                MessageHandler<Tm> handler = (MessageHandler<Tm>)messageHandlers[msg.GetType().Name];
                if (handler != null)
                {
                    try
                    {
                        handler(sender, msg);
                    }
                    catch (System.Exception ex)
                    {
                        Log.ErrorFormat("Message handler exception:{0}, {1}, {2}, {3}", ex.InnerException, ex.Message, ex.Source, ex.StackTrace);
                        if (ThrowException)
                            throw ex;
                    }
                }
                else
                {
                    Log.Warning("No handler subscribed for {0}" + msg.ToString());
                }
            }
        }

        /// <summary>
        /// 启动处理器 [多线程模式]
        /// </summary>
        /// <param name="ThreadNum">线程数</param>
        public void Start(int ThreadNum)
        {
            this.ThreadCount = ThreadNum;
            if (this.ThreadCount < 1)
                this.ThreadCount = 1;
            else if (this.ThreadCount > 1000)
                this.ThreadCount = 1000;
            Running = true;
            for (int i = 0; i < this.ThreadCount; i++)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(MessageDistribute));
            }
            while (ActiveThreadCount < this.ThreadCount)
            {
                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// 停止处理器 [多线程模式]
        /// </summary>
        public void Stop()
        {
            Running = false;
            this.messageQueue.Clear();
            while (ActiveThreadCount > 0)
            {
                threadEvent.Set();
            }
            Thread.Sleep(100);
        }

        /// <summary>
        /// 清除
        /// </summary>
        public void Clear() => this.messageQueue.Clear();

        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="message"></param>
        public void ReceiveMessage(T sender, Charlotte.Proto.NetMessage message)
        {
            this.messageQueue.Enqueue(new MessageArgs() { sender = sender, message = message });
            threadEvent.Set();
        }

        /// <summary>
        /// 分发消息
        /// </summary>
        public void Distribute()
        {
            if (this.messageQueue.Count > 0)
            {
                while (this.messageQueue.Count > 0) //一次性分发队列中的所有消息
                {
                    MessageArgs package = this.messageQueue.Dequeue();
                    if (package.message.Request != null)
                        MessageDispatch<T>.Instance.Dispatch(package.sender, package.message.Request);
                    if (package.message.Response != null)
                        MessageDispatch<T>.Instance.Dispatch(package.sender, package.message.Response);
                }
            }
        }

        /// <summary>
        /// 消息分发 [多线程模式]
        /// </summary>
        /// <param name="stateInfo">状态信息</param>
        private void MessageDistribute(Object stateInfo)
        {
            Log.Warning("MessageDistribute thread start");
            try
            {
                ActiveThreadCount = Interlocked.Increment(ref ActiveThreadCount);
                while (Running)
                {
                    if (this.messageQueue.Count  == 0)
                    {
                        threadEvent.WaitOne();
                        //Log.WarningFormat("[{0}]MessageDistribute Thread[{1}] Continue:", DateTime.Now, Thread.CurrentThread.ManagedThreadId);
                        continue;
                    }
                    MessageArgs package = this.messageQueue.Dequeue();
                    if (package.message.Request != null)
                        MessageDispatch<T>.Instance.Dispatch(package.sender, package.message.Request);
                    if (package.message.Response != null)
                        MessageDispatch<T>.Instance.Dispatch(package.sender, package.message.Response);
                }
            }
            catch
            {

            }
            finally
            {
                ActiveThreadCount = Interlocked.Decrement(ref ActiveThreadCount);
                Log.Warning("MessageDistribute thread end");
            }
        }
    }
}