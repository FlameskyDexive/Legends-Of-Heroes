namespace ET.Client
{
    [Event(SceneType.Demo)]
    public class UpdateRoomPlayersView : AEvent<Scene, UpdateRoomPlayers>
    {
        protected override async ETTask Run(Scene scene, UpdateRoomPlayers args)
        {
            scene.GetComponent<UIComponent>().HideWindow(WindowID.WindowID_Login);
            // scene.GetComponent<RoomComponent>()?.UpdateRoomPlayersInfo(args.roomPlayersProto);
            scene.GetComponent<UIComponent>()?.GetDlgLogic<DlgRoom>()?.RefreshRoomInfo();
            await ETTask.CompletedTask;
        }
    }
}