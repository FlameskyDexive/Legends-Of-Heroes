namespace ET.Client
{
    [Event(SceneType.Demo)]
    public class LoginFinish_CreateLobbyUI: AEvent<Scene, LoginFinish>
    {
        protected override async ETTask Run(Scene scene, LoginFinish args)
        {
            await scene.GetComponent<UIComponent>().ShowWindowAsync(WindowID.WindowID_Lobby);
            await ETTask.CompletedTask;
        }
    }
}