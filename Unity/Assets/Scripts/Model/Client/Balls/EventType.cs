namespace ET.Client
{
    public struct StateSyncSceneChangeStart
    {
        public StateSyncRoom Room;
    }
    
    public struct StateSyncSceneInitFinish
    {
    }

    public struct StateSyncRefreshMatch
    {
        public RoomInfo RoomInfo;
    }
    
}