namespace ET.Client
{
    [FsmNode]
    public class FsmDonwloadWebFiles: AFsmNodeHandler
    {
        public override async ETTask OnEnter(FsmComponent fsmComponent)
        {
            int errorCode = await ResComponent.Instance.DonwloadWebFilesAsync(OnDownloadProgress, OnDownloadError);
            
            if (errorCode != ErrorCode.ERR_Success)
            {
                Log.Error("ResourceComponent.FsmDonwloadWebFiles 出错！{0}".Fmt(errorCode));
                return;
            }
            
            fsmComponent.Transition(nameof(FsmPatchDone));
        }
        
        private static void OnDownloadError(string fileName, string error)
        {
            
        }

        private static void OnDownloadProgress(int totalDownloadCount, int currentDownloadCount, long totalDownloadBytes, long currentDownloadBytes)
        {
            
        }
    }
}