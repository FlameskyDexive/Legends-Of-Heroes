namespace ET
{
    namespace EventType
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
    }
}