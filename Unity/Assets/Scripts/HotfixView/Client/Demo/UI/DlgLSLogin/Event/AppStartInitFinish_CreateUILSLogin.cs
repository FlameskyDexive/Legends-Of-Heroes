namespace ET.Client
{
    [Event(SceneType.LockStep)]
    public class AppStartInitFinish_CreateUILSLogin: AEvent<Scene, AppStartInitFinish>
    {
        protected override async ETTask Run(Scene root, AppStartInitFinish args)
        {
            root.GetComponent<UIComponent>().ShowWindow(WindowID.WindowID_LSLogin);
            await ETTask.CompletedTask;
        }
    }
}