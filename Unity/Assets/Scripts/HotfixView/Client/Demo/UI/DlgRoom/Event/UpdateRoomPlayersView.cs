namespace ET.Client
{
    [Event(SceneType.Client)]
    public class UpdateRoomPlayersView : AEvent<Scene, EventType.UpdateRoomPlayers>
    {
        protected override async ETTask Run(Scene scene, EventType.UpdateRoomPlayers args)
        {
            scene.GetComponent<UIComponent>().HideWindow(WindowID.WindowID_Login);
            scene.GetComponent<RoomComponent>()?.UpdateRoomPlayersInfo(args.roomPlayersProto);
            scene.GetComponent<UIComponent>()?.GetDlgLogic<DlgRoom>()?.RefreshRoomInfo();
            await ETTask.CompletedTask;
        }
    }
}