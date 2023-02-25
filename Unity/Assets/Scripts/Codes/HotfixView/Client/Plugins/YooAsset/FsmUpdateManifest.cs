namespace ET.Client
{
    [FsmNode]
    public class FsmUpdateManifest: AFsmNodeHandler
    {
        public override async ETTask OnEnter(FsmComponent fsmComponent)
        {
            int errorCode = await ResComponent.Instance.UpdateManifestAsync();
            
            if (errorCode != ErrorCode.ERR_Success)
            {
                Log.Error("ResourceComponent.UpdateManifest 出错！{0}".Fmt(errorCode));
                return;
            }
            
            fsmComponent.Transition(nameof(FsmCreateDownloader));
        }
    }
}