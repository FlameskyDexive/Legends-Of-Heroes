namespace ET
{
    public class LoginFinish_CreateLobbyUI: AEventAsync<EventType.LoginFinish>
    {
        protected override async ETTask Run(EventType.LoginFinish args)
        {
            args.ZoneScene.GetComponent<UIComponent>().HideWindow(WindowID.WindowID_Login);
            await args.ZoneScene.GetComponent<UIComponent>().ShowWindowAsync(WindowID.WindowID_Lobby);
        }
    }
}