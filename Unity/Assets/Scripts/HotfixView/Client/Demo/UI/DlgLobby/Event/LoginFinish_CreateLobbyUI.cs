namespace ET.Client
{
    [Event(SceneType.Demo)]
    public class LoginFinish_CreateLobbyUI: AEvent<Scene, LoginFinish>
    {
        protected override async ETTask Run(Scene scene, LoginFinish args)
        {
            scene.GetComponent<UIComponent>().HideWindow(WindowID.WindowID_Login);
            scene.Fiber.Log.Info($"show lobby");
            await scene.GetComponent<UIComponent>().ShowWindowAsync(WindowID.WindowID_Lobby);
        }
    }
}