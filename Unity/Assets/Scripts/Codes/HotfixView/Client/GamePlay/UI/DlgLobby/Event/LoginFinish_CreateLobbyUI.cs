namespace ET.Client
{
    [Event(SceneType.Client)]
    public class LoginFinish_CreateLobbyUI: AEvent<Scene, EventType.LoginFinish>
    {
        protected override async ETTask Run(Scene scene, EventType.LoginFinish args)
        {
            scene.GetComponent<UIComponent>().HideWindow(WindowID.WindowID_Login);
            Log.Info($"show lobby");
            await scene.GetComponent<UIComponent>().ShowWindowAsync(WindowID.WindowID_Lobby);
        }
    }
}