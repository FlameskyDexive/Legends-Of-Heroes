namespace ET.Client
{
    public struct OnPatchDownloadProgress
    {
        public int CurrentDownloadCount;

        public int TotalDownloadCount;

        public long CurrentDownloadSizeBytes;

        public long TotalDownloadSizeBytes;
    }

    public struct OnPatchDownlodFailed
    {
        public string FileName;

        public string Error;
    }
    
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