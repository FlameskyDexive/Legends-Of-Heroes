namespace ET.Client
{
    [Event(SceneType.Demo)]
    public class RefreshMatchViewHandler : AEvent<Scene, StateSyncRefreshMatch>
    {
        protected override async ETTask Run(Scene scene, StateSyncRefreshMatch args)
        {
            scene.GetComponent<UIComponent>()?.GetDlgLogic<DlgRoom>()?.RefreshRoomInfo(args.RoomInfo);
            await ETTask.CompletedTask;
        }
    }
}