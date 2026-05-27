namespace ET.Client
{
    [Event(SceneType.Client)]
    public class OnPatchDownloadProgressEvent : AEvent<Scene, OnPatchDownloadProgress>
    {
        protected override async ETTask Run(Scene scene, OnPatchDownloadProgress a)
        {
            scene.GetComponent<UIComponent>()?.GetDlgLogic<DlgHotUpdate>()?
                .OnPatchDownloadProgress(a.TotalDownloadCount, a.CurrentDownloadCount, a.TotalDownloadSizeBytes, a.CurrentDownloadSizeBytes);
            await ETTask.CompletedTask;
        }
    }
}
