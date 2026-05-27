namespace ET.Client
{
    [Event(SceneType.Client)]
    public class OnPatchDownlodFailedEvent : AEvent<Scene, OnPatchDownlodFailed>
    {
        protected override async ETTask Run(Scene scene, OnPatchDownlodFailed a)
        {
            Log.Error($"下载资源失败: {a.FileName} {a.Error}");
            await ETTask.CompletedTask;
        }
    }
}
