namespace ET.Client
{
    [MessageHandler(SceneType.Demo)]
    public class Match2G_StateSyncNotifyMatchSuccessHandler : MessageHandler<Scene, Match2G_StateSyncNotifyMatchSuccess>
    {
        protected override async ETTask Run(Scene root, Match2G_StateSyncNotifyMatchSuccess message)
        {
            await SceneChangeHelper.SceneChangeTo(root, "Map3", message.ActorId.InstanceId);
        }
    }
}