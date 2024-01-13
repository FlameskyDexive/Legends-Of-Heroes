namespace ET.Client
{
    namespace WaitType
    {
        public struct Wait_Room2C_StateSyncStart : IWaitType
        {
            public int Error { get; set; }

            public Room2C_StateSyncStart Message;
        }
    }
}