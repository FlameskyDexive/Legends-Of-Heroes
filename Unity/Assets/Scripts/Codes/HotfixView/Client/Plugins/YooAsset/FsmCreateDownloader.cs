namespace ET.Client
{
    [FsmNode]
    public class FsmCreateDownloader: AFsmNodeHandler
    {
        public override async ETTask OnEnter(FsmComponent fsmComponent)
        {
            int errorCode = ResComponent.Instance.CreateDownloader();
            
            if (errorCode != ErrorCode.ERR_Success)
            {
                Log.Error("ResourceComponent.FsmCreateDownloader 出错！{0}".Fmt(errorCode));
                return;
            }
            
            if (ResComponent.Instance.Downloader != null)
            {
                Log.Info("Count: {0}, Bytes: {1}", ResComponent.Instance.Downloader.TotalDownloadCount, ResComponent.Instance.Downloader.TotalDownloadBytes);
                fsmComponent.Transition(nameof(FsmDonwloadWebFiles));
            }
            else
            {
                fsmComponent.Transition(nameof(FsmPatchDone));
            }
            
            await ETTask.CompletedTask;
        }
    }
}