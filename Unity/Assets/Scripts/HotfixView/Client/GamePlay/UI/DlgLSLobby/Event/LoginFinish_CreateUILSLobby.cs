namespace ET.Client
{
    [Event(SceneType.LockStep)]
    public class LoginFinish_CreateUILSLobby: AEvent<Scene, LoginFinish>
    {
        protected override async ETTask Run(Scene scene, LoginFinish args)
        {
            scene.GetComponent<UIComponent>().ShowWindow(WindowID.WindowID_LSLobby);
            await ETTask.CompletedTask;
        }
    }
}