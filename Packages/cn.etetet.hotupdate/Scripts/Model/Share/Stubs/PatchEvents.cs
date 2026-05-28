namespace ET
{
    // 以下事件类为 DlgHotUpdate 移植所需的最小桩，等 cn.etetet.yooassets 补丁事件接入后由桩切换为正式定义。

    public struct OnPatchDownloadProgress
    {
        public int TotalDownloadCount;
        public int CurrentDownloadCount;
        public long TotalDownloadSizeBytes;
        public long CurrentDownloadSizeBytes;
    }

    public struct OnPatchDownlodFailed
    {
        public string FileName;
        public string Error;
    }
}
