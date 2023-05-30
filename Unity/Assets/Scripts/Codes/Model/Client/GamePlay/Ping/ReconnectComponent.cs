namespace ET.Client
{
    [ComponentOf(typeof(Scene))]
    public class ReconnectComponent : Entity, IAwake, IUpdate, IDestroy
    {
        public int maxAutoReconnectTimes = 4;
        
        public int reconnectTimes;
        /// <summary>
        /// 间隔5s自动重连一次
        /// </summary>
        public int reconnectInterval = 10000;

        public long timer;
        public long heartTimer;
        public int heartOffCount = 3;

        public bool showMsgBox = false;
    }
}