namespace ET.Client
{
    // 以下事件类为 DlgHotUpdate 移植所需的最小桩，等 cn.etetet.yooassets 补丁事件接入后由桩切换为正式定义。

    [EnableClass]
    public class OnPatchDownloadProgress
    {
        public int TotalDownloadCount;
        public int CurrentDownloadCount;
        public long TotalDownloadSizeBytes;
        public long CurrentDownloadSizeBytes;
    }

    [EnableClass]
    public class OnPatchDownlodFailed
    {
        public string FileName;
        public string Error;
    }
}
