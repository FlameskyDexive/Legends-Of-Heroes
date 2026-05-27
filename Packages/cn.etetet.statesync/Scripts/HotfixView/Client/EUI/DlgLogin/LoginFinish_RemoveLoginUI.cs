namespace ET.Client
{
    [Event(SceneType.Client)]
    public class LoginFinish_RemoveLoginUI : AEvent<Scene, LoginFinish>
    {
        protected override async ETTask Run(Scene scene, LoginFinish args)
        {
            scene.GetComponent<UIComponent>().CloseWindow(WindowID.WindowID_Login);
            await ETTask.CompletedTask;
        }
    }
}
